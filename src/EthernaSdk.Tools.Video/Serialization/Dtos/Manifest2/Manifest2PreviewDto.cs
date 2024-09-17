// Copyright 2020-present Etherna SA
// This file is part of Etherna SDK .Net.
// 
// Etherna SDK .Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Etherna SDK .Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Etherna SDK .Net.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.BeeNet;
using Etherna.BeeNet.Models;
using Etherna.Sdk.Tools.Video.Exceptions;
using Etherna.Sdk.Tools.Video.Models;
using M3U8Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Etherna.Sdk.Tools.Video.Serialization.Dtos.Manifest2
{
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    internal sealed class Manifest2PreviewDto
    {
        // Consts.
        public const int TitleMaxLength = 200;
        
        // Fields.
        private static readonly VideoManifestImage defaultThumbnail = new(
            1.8f,
            "UcGkx38v?CKhoej[j[jtM|bHs:jZjaj[j@ay",
            [VideoManifestImageSource.BuildFromPublishedContent("thumb.jpg", ImageType.Jpeg, SwarmHash.Zero, 100)]);
        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            PropertyNameCaseInsensitive = true,
        };

        // Constructors.
        public Manifest2PreviewDto(string title,
            long createdAt,
            long? updatedAt,
            string ownerEthAddress,
            long duration,
            Manifest2ThumbnailDto? thumbnail)
        {
            Title = title;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            OwnerAddress = ownerEthAddress;
            Duration = duration;
            Thumbnail = thumbnail;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        [JsonConstructor]
        private Manifest2PreviewDto() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Properties.
        public string V => "2.1";
        public string Title { get; set; }
        public long CreatedAt { get; set; }
        public long? UpdatedAt { get; set; }
        public string OwnerAddress { get; set; }
        public long Duration { get; set; }
        public Manifest2ThumbnailDto? Thumbnail { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtraElements { get; set; }
        
        // Methods.
        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
        public ValidationError[] GetValidationErrors()
        {
            var errors = new List<ValidationError>();
            
            if (CreatedAt <= 0)
                errors.Add(new ValidationError(ValidationErrorType.MissingManifestCreationTime));
            
            if (Duration == 0)
                errors.Add(new ValidationError(ValidationErrorType.MissingDuration));

            if (string.IsNullOrWhiteSpace(Title))
                errors.Add(new ValidationError(ValidationErrorType.MissingTitle));
            else if (Title.Length > TitleMaxLength)
                errors.Add(new ValidationError(ValidationErrorType.InvalidTitle, "Title is too long"));
            
            if (Thumbnail is not null)
                errors.AddRange(Thumbnail.GetValidationErrors());
            
            return errors.ToArray();
        }
        
        // Static methods.
        public static async Task<VideoManifest> DeserializeVideoManifestAsync(
            SwarmHash manifestHash,
            JsonElement previewManifestJsonElement,
            IBeeClient beeClient)
        {
            ArgumentNullException.ThrowIfNull(beeClient, nameof(beeClient));
            
            // Get preview manifest.
            var previewManifestDto = previewManifestJsonElement.Deserialize<Manifest2PreviewDto>(jsonSerializerOptions)
                ?? throw new VideoManifestValidationException([new ValidationError(ValidationErrorType.JsonConvert, "Empty json preview manifest")]);

            // Get details manifest.
            using var detailsManifestStream = (await beeClient.GetFileAsync($"{manifestHash}/details").ConfigureAwait(false)).Stream;
            var detailsManifestDto = await JsonSerializer.DeserializeAsync<Manifest2DetailsDto>(
                detailsManifestStream, jsonSerializerOptions).ConfigureAwait(false) ??
                throw new VideoManifestValidationException([new ValidationError(ValidationErrorType.JsonConvert, "Empty json details manifest")]);
            
            // Validate manifests.
            List<ValidationError> errors = [];
            errors.AddRange(previewManifestDto.GetValidationErrors());
            errors.AddRange(detailsManifestDto.GetValidationErrors());
            if (errors.Count != 0)
                throw new VideoManifestValidationException(errors.ToArray());
            
            // Parse additional data.
            //captions
            List<VideoManifestCaptionSource> captions = [];
            foreach (var captionDto in detailsManifestDto.Captions ?? [])
            {
                var captionSwarmUri = new SwarmUri(captionDto.Path, UriKind.RelativeOrAbsolute);
                var captionSwarmAddress = captionSwarmUri.ToSwarmAddress(manifestHash);
                var captionChunkReference = await beeClient.ResolveAddressToChunkReferenceAsync(captionSwarmAddress).ConfigureAwait(false);
                var captionSwarmHash = captionChunkReference.Hash;
                var captionFileName = await beeClient.TryGetFileNameAsync(captionSwarmAddress).ConfigureAwait(false);

                captions.Add(new(
                    captionDto.Label,
                    captionDto.Lang,
                    captionFileName ?? captionDto.Lang,
                    captionSwarmHash));
            }
            
            //thumb sources
            List<VideoManifestImageSource> thumbnailSources = [];
            foreach (var thumbnailSourceDto in previewManifestDto.Thumbnail?.Sources ?? [])
            {
                var imageType = Enum.Parse<ImageType>(thumbnailSourceDto.Type, true);

                var fileName = thumbnailSourceDto.Path.Split(SwarmAddress.Separator).Last();
                if (!Path.HasExtension(fileName))
                    fileName += imageType switch
                    {
                        ImageType.Avif => ".avif",
                        ImageType.Jpeg => ".jpeg",
                        ImageType.Png => ".png",
                        ImageType.Webp => ".webp",
                        _ => throw new InvalidOperationException()
                    };

                var swarmUri = new SwarmUri(thumbnailSourceDto.Path, UriKind.RelativeOrAbsolute);

                var thumbnailSource =  VideoManifestImageSource.BuildFromPublishedContent(
                    fileName,
                    imageType,
                    swarmUri.UriKind == UriKind.Absolute
                        ? swarmUri.ToSwarmAddress()
                        : swarmUri.ToSwarmAddress(manifestHash),
                    thumbnailSourceDto.Width
                );
                
                thumbnailSources.Add(thumbnailSource);
            }
                
            //video sources
            List<VideoManifestVideoSource> videoSources = [];
            foreach (var videoSourceDto in detailsManifestDto.Sources)
            {
                var videoType = Enum.Parse<VideoType>(videoSourceDto.Type, true);
                
                var videoSourceSwarmUri = new SwarmUri(videoSourceDto.Path, UriKind.RelativeOrAbsolute);
                var videoSourceSwarmAddress = videoSourceSwarmUri.ToSwarmAddress(manifestHash);
                
                // Check for additional files.
                List<VideoManifestVideoSourceAdditionalFile> additionalFiles = [];
                switch (videoType)
                {
                    case VideoType.Hls:
                    {
                        //if is master playlist, skip it
                        if (videoSourceDto.Size == 0)
                            break;
                        
                        //if is a stream playlist, read it from swarm
                        var response = await beeClient.GetFileAsync(videoSourceSwarmAddress).ConfigureAwait(false);
                        using var memoryStream = new MemoryStream();
                        await response.Stream.CopyToAsync(memoryStream).ConfigureAwait(false);
                        memoryStream.Position = 0;
                
                        var byteArrayContent = memoryStream.ToArray();
                        await response.Stream.DisposeAsync().ConfigureAwait(false);

                        var playlistString = Encoding.UTF8.GetString(byteArrayContent);
                        var playlist = MediaPlaylist.LoadFromText(playlistString);
                        
                        //retrieve segments as additional files
                        foreach (var segment in playlist.MediaSegments.First().Segments)
                        {
                            var playlistDirectoryPath =
                                videoSourceSwarmAddress.Path[..videoSourceSwarmAddress.Path.LastIndexOf(SwarmAddress.Separator)] + SwarmAddress.Separator;
                            
                            // Read segments info.
                            var segmentSwarmAddress = new SwarmAddress(
                                videoSourceSwarmAddress.Hash,
                                playlistDirectoryPath + segment.Uri);
                            
                            additionalFiles.Add(new VideoManifestVideoSourceAdditionalFile(
                                Path.GetFileName(segment.Uri),
                                (await beeClient.ResolveAddressToChunkReferenceAsync(segmentSwarmAddress).ConfigureAwait(false)).Hash));
                        }
                        
                        break;
                    }
                }

                var videoSource = VideoManifestVideoSource.BuildFromPublishedContent(
                    swarmAddress: videoSourceSwarmAddress,
                    sourceRelativePath: videoSourceDto.Path,
                    videoType: videoType,
                    quality: videoSourceDto.Quality,
                    totalSourceSize: videoSourceDto.Size,
                    additionalFiles: additionalFiles.ToArray());

                videoSources.Add(videoSource);
            }

            // Build manifest.
            return new VideoManifest(
                detailsManifestDto.AspectRatio,
                detailsManifestDto.BatchId,
                DateTimeOffset.FromUnixTimeSeconds(previewManifestDto.CreatedAt),
                detailsManifestDto.Description,
                TimeSpan.FromSeconds(previewManifestDto.Duration),
                previewManifestDto.Title,
                previewManifestDto.OwnerAddress,
                detailsManifestDto.PersonalData,
                videoSources,
                previewManifestDto.Thumbnail is null ? defaultThumbnail :
                    new VideoManifestImage(
                        previewManifestDto.Thumbnail.AspectRatio,
                        previewManifestDto.Thumbnail.Blurhash,
                        thumbnailSources),
                captions,
                previewManifestDto.UpdatedAt.HasValue ?
                    DateTimeOffset.FromUnixTimeSeconds(previewManifestDto.UpdatedAt.Value) :
                    null);
        }
    }
}