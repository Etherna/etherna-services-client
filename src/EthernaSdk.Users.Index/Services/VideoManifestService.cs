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

using Etherna.BeeNet.Hashing.Pipeline;
using Etherna.BeeNet.Hashing.Postage;
using Etherna.BeeNet.Hashing.Store;
using Etherna.BeeNet.Manifest;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Services;
using Etherna.Sdk.Users.Index.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etherna.Sdk.Users.Index.Services
{
    public class VideoManifestService(ICalculatorService calculatorService)
        : IVideoManifestService
    {
        // Consts.
        private const string DetailsManifestFileName = "details";
        private const string ManifestContentType = "application/json";
        private const string PreviewManifestFileName = "preview";

        // Methods.
        public async Task<SwarmHash> CreateVideoManifestChunksAsync(
            VideoManifest manifest,
            string chunkDirectory,
            bool createDirectory = true)
        {
            ArgumentNullException.ThrowIfNull(manifest, nameof(manifest));
            
            // Serialize manifest.
            var previewManifest = manifest.SerializePreviewManifest();
            var detailsManifest = manifest.SerializeDetailsManifest();
            
            // Get byte arrays.
            var previewManifestByteArray = Encoding.UTF8.GetBytes(previewManifest);
            var detailsManifestByteArray = Encoding.UTF8.GetBytes(detailsManifest);
            
            // Create manifests chunks.
            var previewManifestHash = await calculatorService.WriteDataChunksAsync(
                previewManifestByteArray,
                chunkDirectory,
                createDirectory).ConfigureAwait(false);
            var detailsManifestHash = await calculatorService.WriteDataChunksAsync(
                detailsManifestByteArray,
                chunkDirectory,
                createDirectory).ConfigureAwait(false);
            
            // Create mantaray root manifest.
            var chunkStore = new LocalDirectoryChunkStore(chunkDirectory, createDirectory);
            var postageStamper = new FakePostageStamper();
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
                         vs => vs.ManifestUri.UriKind == UriKind.Relative))
            {
                var absoluteHash = videoSource.AbsoluteHash;
                if (absoluteHash is null)
                    throw new InvalidOperationException("Video source absolute hash must be set here");
                
                mantarayManifest.Add(
                    videoSource.ManifestUri.ToString(),
                    ManifestEntry.NewFile(
                        absoluteHash.Value,
                        new Dictionary<string, string>
                            {
                                [ManifestEntry.ContentTypeKey] = videoSource.MimeContentType,
                                [ManifestEntry.FilenameKey] = videoSource.FileName
                            }));
            }
            
            //add encoded thumbnail files, only if uri is relative
            foreach (var thumbnailSource in manifest.Thumbnail.Sources.Where(
                         ts => ts.ManifestUri.UriKind == UriKind.Relative))
            {
                var absoluteHash = thumbnailSource.AbsoluteHash;
                if (absoluteHash is null)
                    throw new InvalidOperationException("Image source absolute hash must be set here");
                
                mantarayManifest.Add(
                    thumbnailSource.ManifestUri.ToString(),
                    ManifestEntry.NewFile(
                        absoluteHash.Value,
                        new Dictionary<string, string>
                        {
                            [ManifestEntry.ContentTypeKey] = thumbnailSource.MimeContentType,
                            [ManifestEntry.FilenameKey] = thumbnailSource.FileName
                        }));
            }

            return (await mantarayManifest.GetHashAsync().ConfigureAwait(false)).Hash;
        }
    }
}