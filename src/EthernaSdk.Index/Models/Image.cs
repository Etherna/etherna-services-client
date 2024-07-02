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

using Etherna.Sdk.Index.GenClients;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.Sdk.Index.Models
{
    public class Image
    {
        // Constructors.
        internal Image(Image2Dto image)
        {
            AspectRatio = image.AspectRatio;
            Blurhash = image.Blurhash;
            Sources = image.Sources.Select(s => new ImageSource(s));
        }
        
        // Properties.
        public float AspectRatio { get; }
        public string Blurhash { get; }
        public IEnumerable<ImageSource> Sources { get; }
    }
}