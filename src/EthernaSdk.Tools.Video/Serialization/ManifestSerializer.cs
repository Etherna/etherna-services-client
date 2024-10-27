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
using Etherna.Sdk.Tools.Video.Models;
using Etherna.Sdk.Tools.Video.Serialization.Dtos.Manifest1;
using Etherna.Sdk.Tools.Video.Serialization.Dtos.Manifest2;
using M3U8Parser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Etherna.Sdk.Tools.Video.Serialization
{
    public static class ManifestSerializer
    {
        // Fields.
        private static readonly VideoManifestImage defaultThumbnail = new(
            1.8f,
            "UcGkx38v?CKhoej[j[jtM|bHs:jZjaj[j@ay",
            [new VideoManifestImageSource("thumb.jpg", ImageType.Jpeg, 100, SwarmHash.Zero)]);
        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            PropertyNameCaseInsensitive = true,
        };
        
        // Static methods.
        public static async Task<(VideoManifest?, ValidationError[])> TryDeserializeManifest1Async(
            JsonElement manifestJsonElement,
            IBeeClient beeClient)
        {
            ArgumentNullException.ThrowIfNull(beeClient, nameof(beeClient));
            
            Dictionary<SwarmHash, SwarmChunk> chunksCache = [];
            
            // Get manifest.
            var manifestDto = manifestJsonElement.Deserialize<Manifest1Dto>(jsonSerializerOptions);
            if (manifestDto is null)
                return (null, [new ValidationError(ValidationErrorType.JsonConvert, "Empty json")]);
            
            // Validate manifest.
            var validationErrors = manifestDto.GetValidationErrors();
            if (validationErrors.Length > 0)
                return (null, validationErrors);

            // Build manifest.
            //video sources
            List<VideoManifestVideoSource> videoSources = [];
            foreach (var videoSourceDto in manifestDto.Sources)
            {
                var videoSourceChunkRef = await beeClient.ResolveAddressToChunkReferenceAsync(
                    videoSourceDto.Reference,
                    chunksCache).ConfigureAwait(false);
                videoSources.Add(new VideoManifestVideoSource(
                    videoSourceDto.Quality + ".mp4",
                    VideoType.Mp4,
                    videoSourceDto.Quality,
                    videoSourceDto.Size ?? 100,
                    [],
                    videoSourceChunkRef.Hash));
            }
            
            //thumbnail
            var thumbnail = defaultThumbnail;
            if (manifestDto.Thumbnail is not null)
            {
                List<VideoManifestImageSource> imgSources = [];
                foreach (var imgSourceDto in manifestDto.Thumbnail.Sources)
                {
                    var imgSourceChunkRef = await beeClient.ResolveAddressToChunkReferenceAsync(
                        imgSourceDto.Value,
                        chunksCache).ConfigureAwait(false);
                    imgSources.Add(new VideoManifestImageSource(
                        imgSourceDto.Key.TrimEnd('w') + ".jpg",
                        ImageType.Jpeg,
                        int.Parse(imgSourceDto.Key.TrimEnd('w'), CultureInfo.InvariantCulture),
                        imgSourceChunkRef.Hash));
                }

                thumbnail = new VideoManifestImage(
                    manifestDto.Thumbnail.AspectRatio,
                    manifestDto.Thumbnail.Blurhash,
                    imgSources);
            }
            
            //manifest
            return (new VideoManifest(
                manifestDto.Thumbnail?.AspectRatio ?? 1,
                manifestDto.BatchId is null ? (PostageBatchId?)null : PostageBatchId.FromString(manifestDto.BatchId),
                DateTimeOffset.FromUnixTimeMilliseconds(manifestDto.CreatedAt ?? 0),
                manifestDto.Description,
                TimeSpan.FromSeconds(manifestDto.Duration),
                manifestDto.Title,
                manifestDto.OwnerAddress,
                manifestDto.PersonalData,
                videoSources,
                thumbnail,
                [],
                manifestDto.UpdatedAt.HasValue ?
                    DateTimeOffset.FromUnixTimeMilliseconds(manifestDto.UpdatedAt.Value) :
                    null), []);
        }
        
        public static async Task<(VideoManifest?, ValidationError[])> TryDeserializeManifest2Async(
            SwarmHash manifestHash,
            JsonElement previewManifestJsonElement,
            IBeeClient beeClient)
        {
            ArgumentNullException.ThrowIfNull(beeClient, nameof(beeClient));
            
            Dictionary<SwarmHash, SwarmChunk> chunksCache = [];
            
            // Get preview manifest.
            var previewManifestDto = previewManifestJsonElement.Deserialize<Manifest2PreviewDto>(jsonSerializerOptions);
            if (previewManifestDto is null)
                return (null, [new ValidationError(ValidationErrorType.JsonConvert, "Empty json preview manifest")]);

            // Get details manifest.
            using var detailsManifestStream = (await beeClient.GetFileAsync($"{manifestHash}/details").ConfigureAwait(false)).Stream;
            var detailsManifestDto = await JsonSerializer.DeserializeAsync<Manifest2DetailsDto>(
                detailsManifestStream, jsonSerializerOptions).ConfigureAwait(false);
            if (detailsManifestDto is null)
                return (null, [new ValidationError(ValidationErrorType.JsonConvert, "Empty json details manifest")]);
            
            // Validate manifests.
            List<ValidationError> errors = [];
            errors.AddRange(previewManifestDto.GetValidationErrors());
            errors.AddRange(detailsManifestDto.GetValidationErrors());
            if (errors.Count != 0)
                return (null, errors.ToArray());
            
            // Parse additional data.
            //captions
            List<VideoManifestCaptionSource> captions = [];
            foreach (var captionDto in detailsManifestDto.Captions ?? [])
            {
                var captionSwarmUri = new SwarmUri(captionDto.Path, UriKind.RelativeOrAbsolute);
                var captionSwarmAddress = captionSwarmUri.ToSwarmAddress(manifestHash);
                var captionChunkReference = await beeClient.ResolveAddressToChunkReferenceAsync(
                    captionSwarmAddress,
                    chunksCache).ConfigureAwait(false);
                var captionFileName = captionDto.Path.Split(SwarmAddress.Separator).Last();

                captions.Add(new(
                    captionDto.Label,
                    captionDto.Lang,
                    captionFileName,
                    captionChunkReference.Hash));
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

                var thumbnailAddress = swarmUri.ToSwarmAddress(manifestHash);
                var thumbnailChunkRef = await beeClient.ResolveAddressToChunkReferenceAsync(
                    thumbnailAddress,
                    chunksCache).ConfigureAwait(false);
                
                var thumbnailSource = new VideoManifestImageSource(
                    fileName,
                    imageType,
                    thumbnailSourceDto.Width,
                    thumbnailChunkRef.Hash);
                
                thumbnailSources.Add(thumbnailSource);
            }
                
            //video sources
            List<VideoManifestVideoSource> videoSources = [];
            foreach (var videoSourceDto in detailsManifestDto.Sources)
            {
                var videoType = Enum.Parse<VideoType>(videoSourceDto.Type, true);
                
                var videoSourceSwarmUri = new SwarmUri(videoSourceDto.Path, UriKind.RelativeOrAbsolute);
                var videoSourceSwarmAddress = videoSourceSwarmUri.ToSwarmAddress(manifestHash);
                var videoSourceChunkRef = await beeClient.ResolveAddressToChunkReferenceAsync(
                    videoSourceSwarmAddress,
                    chunksCache).ConfigureAwait(false);
                
                var sourceDirectoryPath = VideoManifestVideoSource.GetManifestVideoSourceBaseDirectory(videoType);
                if (!videoSourceDto.Path.StartsWith(sourceDirectoryPath, StringComparison.Ordinal))
                    return (null, [new ValidationError(ValidationErrorType.InvalidVideoSource, "Invalid video source path")]);
                var sourceRelativePath = videoSourceDto.Path[sourceDirectoryPath.Length..];
                
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
                        
                        var playlistDirectoryPath =
                            (videoSourceSwarmAddress.Path[..videoSourceSwarmAddress.Path.LastIndexOf(SwarmAddress.Separator)] + SwarmAddress.Separator).TrimStart(SwarmAddress.Separator);
                            
                        //retrieve segments as additional files
                        foreach (var segment in playlist.MediaSegments.First().Segments)
                        {
                            var segmentPath = playlistDirectoryPath + segment.Uri;
                            var segmentRelativePath = segmentPath[sourceDirectoryPath.Length..];
                            
                            var segmentSwarmAddress = new SwarmAddress(
                                videoSourceSwarmAddress.Hash,
                                segmentPath);
                            
                            additionalFiles.Add(new VideoManifestVideoSourceAdditionalFile(
                                segmentRelativePath,
                                (await beeClient.ResolveAddressToChunkReferenceAsync(
                                    segmentSwarmAddress,
                                    chunksCache).ConfigureAwait(false)).Hash));
                        }
                        
                        break;
                    }
                }

                var videoSource = new VideoManifestVideoSource(
                    sourceRelativePath,
                    videoType,
                    videoSourceDto.Quality,
                    videoSourceDto.Size,
                    additionalFiles.ToArray(),
                    videoSourceChunkRef.Hash);

                videoSources.Add(videoSource);
            }

            // Build manifest.
            return (new VideoManifest(
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
                    null), []);
        }
    }
}