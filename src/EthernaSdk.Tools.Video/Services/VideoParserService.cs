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
using Etherna.Sdk.Index.GenClients;
using Etherna.Sdk.Tools.Video.Models;
using M3U8Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etherna.Sdk.Tools.Video.Services
{
    public class VideoParserService(IBeeClient beeClient)
        : IVideoParserService
    {
        public async Task<IndexedVideo> BuildIndexedVideoAsync(Video2Dto videoDto)
        {
            ArgumentNullException.ThrowIfNull(videoDto, nameof(videoDto));

            List<VideoManifestImageSource> thumbnailSources = [];
            List<VideoManifestVideoSource> videoSources = [];
            if (videoDto.LastValidManifest is not null)
            {
                //thumb sources
                foreach (var thumbnailSource in videoDto.LastValidManifest.Thumbnail?.Sources ?? [])
                    thumbnailSources.Add(
                        BuildVideoManifestImageSource(
                            thumbnailSource,
                            videoDto.LastValidManifest.Hash));
                
                //video sources
                foreach (var videoSource in videoDto.LastValidManifest.Sources)
                    videoSources.Add(
                        await BuildVideoManifestVideoSourceAsync(
                            videoSource,
                            videoDto.LastValidManifest.Hash).ConfigureAwait(false));
            }
            
            // Build published video manifest.
            PublishedVideoManifest? publishedVideoManifest = null;
            if (videoDto.LastValidManifest is not null)
            {
                //aspect ratio con be 0 from manifest v1. Migrate taking it from thumbnail
                var aspectRatio = videoDto.LastValidManifest.AspectRatio != 0
                    ? videoDto.LastValidManifest.AspectRatio
                    : videoDto.LastValidManifest.Thumbnail?.AspectRatio ?? 1;
                
                //create at was indicated in milliseconds with manifest v1
                DateTimeOffset createdAt;
                try
                {
                    createdAt = DateTimeOffset.FromUnixTimeSeconds(videoDto.LastValidManifest.CreatedAt);
                }
                catch (ArgumentOutOfRangeException)
                {
                    createdAt = DateTimeOffset.FromUnixTimeMilliseconds(videoDto.LastValidManifest.CreatedAt);
                }
                
                //same for updated at
                DateTimeOffset? updatedAt = null;
                if (videoDto.LastValidManifest.UpdatedAt is not null)
                {
                    try
                    {
                        updatedAt = DateTimeOffset.FromUnixTimeSeconds(videoDto.LastValidManifest.UpdatedAt.Value);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        updatedAt = DateTimeOffset.FromUnixTimeMilliseconds(videoDto.LastValidManifest.UpdatedAt.Value);
                    }
                }
                
                publishedVideoManifest = new PublishedVideoManifest(
                    hash: videoDto.LastValidManifest.Hash,
                    manifest: new(
                        aspectRatio: aspectRatio,
                        batchId: videoDto.LastValidManifest.BatchId ?? PostageBatchId.Zero,
                        createdAt: createdAt,
                        description: videoDto.LastValidManifest.Description ?? "",
                        duration: TimeSpan.FromSeconds(videoDto.LastValidManifest.Duration ?? 0),
                        title: videoDto.LastValidManifest.Title ?? "",
                        videoDto.OwnerAddress,
                        personalData: videoDto.LastValidManifest.PersonalData,
                        videoSources: videoSources,
                        thumbnail: new VideoManifestImage(
                            aspectRatio: videoDto.LastValidManifest.Thumbnail?.AspectRatio ?? 1,
                            blurhash: videoDto.LastValidManifest.Thumbnail?.Blurhash ?? "",
                            sources: thumbnailSources),
                        captionSources: [],
                        updatedAt: updatedAt));
            }
            
            return new IndexedVideo(
                id: videoDto.Id,
                creationDateTime: videoDto.CreationDateTime,
                currentVoteValue: videoDto.CurrentVoteValue.HasValue ?
                    Enum.Parse<VoteValue>(videoDto.CurrentVoteValue.Value.ToString()) : null,
                lastValidManifest: publishedVideoManifest,
                ownerAddress: videoDto.OwnerAddress,
                totDownvotes: videoDto.TotDownvotes,
                totUpvotes: videoDto.TotUpvotes);
        }

        public async Task<VideoManifestVideoSource> BuildVideoManifestVideoSourceAsync(
            VideoSourceDto videoSourceDto,
            string videoManifestHashStr)
        {
            ArgumentNullException.ThrowIfNull(videoSourceDto, nameof(videoSourceDto));
            
            if (!Enum.TryParse<VideoType>(videoSourceDto.Type, true, out var videoType))
                videoType = VideoType.Mp4;
            
            var videoSourceSwarmUri = new SwarmUri(videoSourceDto.Path, UriKind.RelativeOrAbsolute);
            var videoSourceSwarmAddress = videoSourceSwarmUri.ToSwarmAddress(videoManifestHashStr);
            
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
                        // Read segments info.
                        var segmentSwarmAddress = new SwarmAddress(
                            videoSourceSwarmAddress.Hash,
                            videoSourceSwarmAddress.Path.TrimEnd(SwarmAddress.Separator) + SwarmAddress.Separator +
                            segment.Uri);
                        
                        additionalFiles.Add(new VideoManifestVideoSourceAdditionalFile(
                            Path.GetFileName(segment.Uri),
                            (await beeClient.ResolveAddressToChunkReferenceAsync(segmentSwarmAddress).ConfigureAwait(false)).Hash));
                    }
                    
                    break;
                }
            }

            return VideoManifestVideoSource.BuildFromPublishedContent(
                swarmAddress: videoSourceSwarmAddress,
                sourceRelativePath: videoSourceDto.Path,
                videoType: videoType,
                quality: videoSourceDto.Quality,
                totalSourceSize: videoSourceDto.Size,
                additionalFiles: additionalFiles.ToArray());
        }

        public VideoManifestImageSource BuildVideoManifestImageSource(
            ImageSourceDto imageSourceDto,
            string videoManifestHashStr)
        {
            ArgumentNullException.ThrowIfNull(imageSourceDto, nameof(imageSourceDto));
            
            if (!Enum.TryParse<ImageType>(imageSourceDto.Type, true, out var imageType))
                imageType = ImageType.Jpeg; //only used jpeg at first

            var fileName = imageSourceDto.Path.Split(SwarmAddress.Separator).Last();
            if (!Path.HasExtension(fileName))
                fileName += imageType switch
                {
                    ImageType.Avif => ".avif",
                    ImageType.Jpeg => ".jpeg",
                    ImageType.Png => ".png",
                    ImageType.Webp => ".webp",
                    _ => throw new InvalidOperationException()
                };

            var swarmUri = new SwarmUri(imageSourceDto.Path, UriKind.RelativeOrAbsolute);

            return VideoManifestImageSource.BuildFromPublishedContent(
                fileName,
                imageType,
                swarmUri.UriKind == UriKind.Absolute
                    ? swarmUri.ToSwarmAddress()
                    : swarmUri.ToSwarmAddress(videoManifestHashStr),
                imageSourceDto.Width
            );
        }
    }
}