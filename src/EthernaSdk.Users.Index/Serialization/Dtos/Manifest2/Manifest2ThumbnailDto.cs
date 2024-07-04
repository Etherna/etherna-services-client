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

using System.Collections.Generic;

namespace Etherna.Sdk.Users.Index.Serialization.Dtos.Manifest2
{
    public sealed class Manifest2ThumbnailDto
    {
        // Constructors.
        public Manifest2ThumbnailDto(
            float aspectRatio,
            string blurhash,
            IEnumerable<Manifest2ThumbnailSourceDto> sources)
        {
            AspectRatio = aspectRatio;
            Blurhash = blurhash;
            Sources = sources;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Manifest2ThumbnailDto() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        
        // Properties.
        public float AspectRatio { get; private set; }
        public string Blurhash { get; private set; }
        public IEnumerable<Manifest2ThumbnailSourceDto> Sources { get; private set; }
    }
}
