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
using System.Collections.Generic;

namespace Etherna.Sdk.Users.Index.Models
{
    /// <summary>
    /// Video Manifest utility class, able to serialize/deserialize to/from json
    /// </summary>
    public class VideoManifest(
        float aspectRatio,
        PostageBatchId? batchId,
        DateTimeOffset createdAt,
        string description,
        TimeSpan duration,
        string title,
        string ownerAddress,
        string? personalData,
        IEnumerable<VideoManifestVideoSource> sources,
        VideoManifestImage? thumbnail,
        DateTimeOffset? updatedAt)
    {
        // Methods.
        public string SerializeManifestDetail()
        {
            throw new NotImplementedException();
        }

        public string SerializeManifestPreview()
        {
            throw new NotImplementedException();
        }
        
        // Static methods.
        public VideoManifest DeserializeManifest(string jsonManifest)
        {
            throw new NotImplementedException();
        }

        // Properties.
        public float AspectRatio { get; } = aspectRatio;
        public PostageBatchId? BatchId { get; } = batchId;
        public DateTimeOffset CreatedAt { get; } = createdAt;
        public string Description { get; } = description;
        public TimeSpan Duration { get; } = duration;
        public string Title { get; } = title;
        public string OwnerAddress { get; } = ownerAddress;
        public string? PersonalData { get; } = personalData;
        public IEnumerable<VideoManifestVideoSource> Sources { get; } = sources;
        public VideoManifestImage? Thumbnail { get; } = thumbnail;
        public DateTimeOffset? UpdatedAt { get; } = updatedAt;
    }
}