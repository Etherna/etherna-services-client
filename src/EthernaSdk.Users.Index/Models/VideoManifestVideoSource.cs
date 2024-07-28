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
        public SwarmHash? AbsoluteHash { get; set; }
        public string FileName => ManifestUri.ToString().Split(SwarmAddress.Separator).Last();
        public SwarmUri ManifestUri { get; } = manifestUri;
        public string MimeContentType => Type switch
        {
            VideoSourceType.Dash => "application/dash+xml",
            VideoSourceType.Hls => "application/x-mpegURL",
            VideoSourceType.Mp4 => "video/mp4",
            _ => throw new NotSupportedException()
        };
        public string? Quality { get; } = quality;
        public long Size { get; } = size;
        public VideoSourceType Type { get; } = type;
    }
}