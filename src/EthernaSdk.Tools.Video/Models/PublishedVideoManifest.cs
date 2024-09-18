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
using System.Collections.Generic;
using System.Linq;

namespace Etherna.Sdk.Tools.Video.Models
{
    public class PublishedVideoManifest(
        SwarmHash hash,
        VideoManifest? manifest,
        ValidationError[] validationErrors)
    {
        // Properties.
        public SwarmHash Hash { get; } = hash;
        public VideoManifest? Manifest { get; } = manifest;
        public IReadOnlyCollection<ValidationError> ValidationErrors { get; } = validationErrors;
        
        // Methods.
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not PublishedVideoManifest other) return false;
            return GetType() == other.GetType() &&
                   Hash.Equals(other.Hash) &&
                   EqualityComparer<VideoManifest?>.Default.Equals(Manifest, other.Manifest) &&
                   ValidationErrors.SequenceEqual(other.ValidationErrors);
        }

        public override int GetHashCode() =>
            Hash.GetHashCode() ^
            Manifest?.GetHashCode() ?? 0 ^
            ValidationErrors.GetHashCode();
    }
}