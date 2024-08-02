// Copyright 2022-present Etherna SA
// This file is part of Etherna Video Importer.
// 
// Etherna Video Importer is free software: you can redistribute it and/or modify it under the terms of the
// GNU Affero General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Etherna Video Importer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Etherna Video Importer.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.BeeNet.Models;
using Etherna.Sdk.Users.Index.Models;
using System.Diagnostics.CodeAnalysis;

namespace Etherna.Sdk.Users.Index.Serialization.Dtos.Manifest2
{
    [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase")]
    public sealed class Manifest2VideoSourceDto
    {
        // Constructors.
        public Manifest2VideoSourceDto(
            VideoType type,
            string? quality,
            SwarmUri path,
            long size)
        {
            Quality = quality;
            Path = path.ToString();
            Size = size;
            Type = type.ToString().ToLowerInvariant();
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Manifest2VideoSourceDto() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Properties.
        public string Type { get; private set; }
        public string? Quality { get; private set; }
        public string Path { get; private set; }
        public long Size { get; private set; }
    }
}
