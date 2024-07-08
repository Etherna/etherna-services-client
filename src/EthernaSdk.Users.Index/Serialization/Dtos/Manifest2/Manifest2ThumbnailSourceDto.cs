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
using System.Diagnostics.CodeAnalysis;

namespace Etherna.Sdk.Users.Index.Serialization.Dtos.Manifest2
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum Manifest2ThumbnailSourceType
    {
        jpeg,
        png,
        webp,
        avif
    }
    
    public class Manifest2ThumbnailSourceDto
    {
        // Constructors.
        public Manifest2ThumbnailSourceDto(
            int width,
            Manifest2ThumbnailSourceType type,
            SwarmUri path)
        {
            Width = width;
            Type = type.ToString();
            Path = path.ToString();
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Manifest2ThumbnailSourceDto() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Properties.
        public int Width { get; private set; }
        public string Type { get; private set; }
        public string Path { get; private set; }
    }
}