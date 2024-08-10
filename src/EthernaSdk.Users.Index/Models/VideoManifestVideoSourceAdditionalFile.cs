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
using System.IO;

namespace Etherna.Sdk.Users.Index.Models
{
    public class VideoManifestVideoSourceAdditionalFile(
        string fileName,
        SwarmHash swarmHash,
        SwarmUri swarmUri)
    {
        // Properties.
        /// <summary>
        /// The file name, used to set the download file name in mantaray
        /// </summary>
        public string FileName { get; } = fileName;

        /// <summary>
        /// The video mime content type, used to set content type with the mantaray manifest
        /// </summary>
        public string MimeContentType => Path.GetExtension(FileName) switch
        {
            ".ts" => "video/MP2T",
            _ => throw new NotSupportedException()
        };
        
        /// <summary>
        /// Absolute swarm hash. Used to link internal mantaray path to resource.
        /// </summary>
        public SwarmHash SwarmHash { get; } = swarmHash;

        public SwarmUri SwarmUri { get; } = swarmUri;
    }
}