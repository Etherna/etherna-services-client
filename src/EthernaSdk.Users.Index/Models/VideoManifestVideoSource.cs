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
using System;
using System.Linq;

namespace Etherna.Sdk.Users.Index.Models
{
    public class VideoManifestVideoSource(
        SwarmUri manifestUri,
        VideoSourceType type,
        string? quality,
        long size)
    {
        // Properties.
        /// <summary>
        /// Absolute resource hash.
        /// Used to link internal mantaray path to resource, when Uri is relative
        /// </summary>
        public SwarmHash? AbsoluteHash { get; set; }
        
        /// <summary>
        /// The file name, used to set the download file name in mantaray
        /// </summary>
        public string FileName { get; set; } = manifestUri.ToString().Split(SwarmAddress.Separator).Last();

        /// <summary>
        /// The uri to use with the mantaray and video manifests
        /// </summary>
        public SwarmUri ManifestUri { get; } = manifestUri;

        /// <summary>
        /// The video mime content type, used to set content type with the mantaray manifest
        /// </summary>
        public string MimeContentType => Type switch
        {
            VideoSourceType.Dash => "application/dash+xml",
            VideoSourceType.Hls => "application/x-mpegURL",
            VideoSourceType.Mp4 => "video/mp4",
            _ => throw new NotSupportedException()
        };
        
        /// <summary>
        /// The video stream quality
        /// </summary>
        public string? Quality { get; } = quality;

        /// <summary>
        /// The video stream byte size
        /// </summary>
        public long Size { get; } = size;

        /// <summary>
        /// The video type, used to derive mime conten type
        /// </summary>
        public VideoSourceType Type { get; } = type;
    }
}