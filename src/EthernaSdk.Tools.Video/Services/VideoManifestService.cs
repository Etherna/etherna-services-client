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
using Etherna.BeeNet.Hashing.Pipeline;
using Etherna.BeeNet.Hashing.Postage;
using Etherna.BeeNet.Hashing.Signer;
using Etherna.BeeNet.Manifest;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Services;
using Etherna.BeeNet.Stores;
using Etherna.Sdk.Tools.Video.Models;
using Etherna.Sdk.Tools.Video.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Etherna.Sdk.Tools.Video.Services
{
    public class VideoManifestService(
        IBeeClient beeClient,
        IChunkService chunkService)
        : IVideoManifestService
    {
        // Consts.
        private const string DetailsManifestFileName = "details";
        private const string ManifestContentType = "application/json";
        private const string PreviewManifestFileName = "preview";

        // Methods.
        public async Task<SwarmHash> CreateVideoManifestChunksAsync(
            VideoManifest manifest,
            string chunksDirectory,
            bool createDirectory = true,
            IPostageStampIssuer? postageStampIssuer = null)
        {
            ArgumentNullException.ThrowIfNull(manifest, nameof(manifest));
            
            // Serialize manifest.
            var previewManifest = manifest.SerializePreviewManifest();
            var detailsManifest = manifest.SerializeDetailsManifest();
            
            // Get byte arrays.
            var previewManifestByteArray = Encoding.UTF8.GetBytes(previewManifest);
            var detailsManifestByteArray = Encoding.UTF8.GetBytes(detailsManifest);
            
            // Create video manifests chunks.
            var previewManifestHash = await chunkService.WriteDataChunksAsync(
                previewManifestByteArray,
                chunksDirectory,
                postageStampIssuer,
                createDirectory).ConfigureAwait(false);
            var detailsManifestHash = await chunkService.WriteDataChunksAsync(
                detailsManifestByteArray,
                chunksDirectory,
                postageStampIssuer,
                createDirectory).ConfigureAwait(false);
            
            // Create mantaray root manifest.
            var chunkStore = new LocalDirectoryChunkStore(chunksDirectory, createDirectory);
            IPostageStamper postageStamper = postageStampIssuer is null
                ? new FakePostageStamper()
                : new PostageStamper(
                    new FakeSigner(),
                    postageStampIssuer,
                    new MemoryStampStore());
            var mantarayManifest = new MantarayManifest(
                () => HasherPipelineBuilder.BuildNewHasherPipeline(
                    chunkStore,
                    postageStamper,
                    RedundancyLevel.None,
                    false,
                    0,
                    null),
                false);
            
            //add default (preview)
            mantarayManifest.Add(
                MantarayManifest.RootPath,
                ManifestEntry.NewDirectory(
                    new Dictionary<string, string>
                    {
                        [ManifestEntry.WebsiteIndexDocPathKey] = PreviewManifestFileName
                    }));
            
            //add preview
            mantarayManifest.Add(
                PreviewManifestFileName,
                ManifestEntry.NewFile(
                    previewManifestHash,
                    new Dictionary<string, string>
                    {
                        [ManifestEntry.ContentTypeKey] = ManifestContentType,
                        [ManifestEntry.FilenameKey] = PreviewManifestFileName
                    }));
            
            //add details
            mantarayManifest.Add(
                DetailsManifestFileName,
                ManifestEntry.NewFile(
                    detailsManifestHash,
                    new Dictionary<string, string>
                    {
                        [ManifestEntry.ContentTypeKey] = ManifestContentType,
                        [ManifestEntry.FilenameKey] = DetailsManifestFileName
                    }));
            
            //add encoded video streams, only if uri is relative
            foreach (var videoSource in manifest.VideoSources.Where(
                         vs => vs.Uri.UriKind == UriKind.Relative))
            {
                mantarayManifest.Add(
                    videoSource.Uri.ToString(),
                    ManifestEntry.NewFile(
                        videoSource.Metadata.ContentSwarmHash,
                        new Dictionary<string, string>
                        {
                            [ManifestEntry.ContentTypeKey] = videoSource.Metadata.MimeContentType,
                            [ManifestEntry.FilenameKey] = videoSource.Metadata.FileName
                        }));

                //add optional additional files, if present (ex. Hls segments)
                foreach (var additionalFile in videoSource.Metadata.AdditionalFiles)
                {
                    mantarayManifest.Add(
                        additionalFile.Uri.ToString(),
                        ManifestEntry.NewFile(
                            additionalFile.File.SwarmHash,
                            new Dictionary<string, string>
                            {
                                [ManifestEntry.ContentTypeKey] = additionalFile.File.MimeContentType,
                                [ManifestEntry.FilenameKey] = additionalFile.File.FileName
                            }));
                }
            }
            
            //add encoded thumbnail files, only if uri is relative
            foreach (var thumbnailSource in manifest.Thumbnail.Sources.Where(
                         ts => ts.Uri.UriKind == UriKind.Relative))
            {
                mantarayManifest.Add(
                    thumbnailSource.Uri.ToString(),
                    ManifestEntry.NewFile(
                        thumbnailSource.Metadata.ContentSwarmHash,
                        new Dictionary<string, string>
                        {
                            [ManifestEntry.ContentTypeKey] = thumbnailSource.Metadata.MimeContentType,
                            [ManifestEntry.FilenameKey] = thumbnailSource.Metadata.FileName
                        }));
            }
            
            //add captions files, only if uri is relative
            foreach (var captionSource in manifest.CaptionSources.Where(
                         cs => cs.Uri.UriKind == UriKind.Relative))
            {
                mantarayManifest.Add(
                    captionSource.Uri.ToString(),
                    ManifestEntry.NewFile(
                        captionSource.Source.ContentSwarmHash,
                        new Dictionary<string, string>
                        {
                            [ManifestEntry.ContentTypeKey] = captionSource.Source.MimeContentType,
                            [ManifestEntry.FilenameKey] = captionSource.Source.FileName
                        }));
            }

            return (await mantarayManifest.GetHashAsync().ConfigureAwait(false)).Hash;
        }

        public async Task<PublishedVideoManifest> GetPublishedVideoManifestAsync(SwarmHash manifestHash)
        {
            using var rootManifestStream = (await beeClient.GetFileAsync(manifestHash).ConfigureAwait(false)).Stream;
            var rootManifestJsonElement = await JsonSerializer.DeserializeAsync<JsonElement>(rootManifestStream).ConfigureAwait(false);

            // Find version.
            var versionStr = rootManifestJsonElement.TryGetProperty("v", out var jsonVersion) ?
                jsonVersion.GetString()! :
                "1.0"; //first version didn't have an identifier
            var version = new Version(versionStr);

            // Deserialize document.
            var (videoManifest, errors) = version.Major switch
            {
                1 => await ManifestSerializer.TryDeserializeManifest1Async(rootManifestJsonElement, beeClient).ConfigureAwait(false),
                2 => await ManifestSerializer.TryDeserializeManifest2Async(manifestHash, rootManifestJsonElement, beeClient).ConfigureAwait(false),
                _ => (null, [new ValidationError(ValidationErrorType.JsonConvert, "Invalid version")])
            };

            return new PublishedVideoManifest(manifestHash, videoManifest, errors);
        }
    }
}