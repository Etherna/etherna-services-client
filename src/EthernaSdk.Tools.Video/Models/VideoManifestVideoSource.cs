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

using Etherna.BeeNet.Models;
using Nethereum.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Etherna.Sdk.Tools.Video.Models
{
    public class VideoManifestVideoSource
    {
        // Fields.
        private readonly VideoManifestVideoSourceAdditionalFile[] additionalFiles;

        // Constructors.
        private VideoManifestVideoSource(
            string sourceRelativePath,
            VideoType videoType,
            string? quality,
            long totalSourceSize,
            VideoManifestVideoSourceAdditionalFile[] additionalFiles)
        {
            this.additionalFiles = additionalFiles;
            Quality = quality;
            SourceRelativePath = sourceRelativePath;
            TotalSourceSize = totalSourceSize;
            VideoType = videoType;
        }
        
        // Builders.
        public static VideoManifestVideoSource BuildFromPublishedContent(
            SwarmAddress swarmAddress,
            string sourceRelativePath,
            VideoType videoType,
            string? quality,
            long totalSourceSize,
            VideoManifestVideoSourceAdditionalFile[] additionalFiles) =>
            new(sourceRelativePath, videoType, quality, totalSourceSize, additionalFiles)
        {
            SwarmAddress = swarmAddress
        };
        
        public static VideoManifestVideoSource BuildFromDirectContentHash(
            SwarmHash directContentHash,
            string sourceRelativePath,
            VideoType videoType,
            string? quality,
            long totalSourceSize,
            VideoManifestVideoSourceAdditionalFile[] additionalFiles) =>
            new(sourceRelativePath, videoType, quality, totalSourceSize, additionalFiles)
        {
            ContentSwarmHash = directContentHash
        };

        // Properties.
        /// <summary>
        /// Content direct swarm hash. Used to link internal mantaray path to resource.
        /// </summary>
        public SwarmHash? ContentSwarmHash { get; set; }
        
        /// <summary>
        /// The file name, used to set the download file name in mantaray
        /// </summary>
        public string FileName => Path.GetFileName(SourceRelativePath);

        /// <summary>
        /// The video mime content type, used to set content type with the mantaray manifest
        /// </summary>
        public string MimeContentType => VideoType switch
        {
            VideoType.Dash => "application/dash+xml",
            VideoType.Hls => "application/x-mpegURL",
            VideoType.Mp4 => "video/mp4",
            _ => throw new NotSupportedException()
        };

        /// <summary>
        /// The video stream quality
        /// </summary>
        public string? Quality { get; }
        
        /// <summary>
        /// Relative path inside the source directory
        /// </summary>
        public string SourceRelativePath { get; }
        
        public SwarmAddress SwarmAddress { get; private set; }

        /// <summary>
        /// The video stream byte size
        /// </summary>
        public long TotalSourceSize { get; }

        /// <summary>
        /// The video type, used to derive mime conten type
        /// </summary>
        public VideoType VideoType { get; }

        public IEnumerable<(SwarmUri Uri, VideoManifestVideoSourceAdditionalFile File)> AdditionalFiles =>
            additionalFiles.Select(f => (new SwarmUri(
                GetManifestVideoSourceBaseDirectory() + f.SourceRelativePath,
                UriKind.Relative), f));
        
        // Methods.
        public string GetManifestVideoSourceBaseDirectory() =>
            $"sources/{VideoType.ToStringInvariant().ToLowerInvariant()}/";
    }
}