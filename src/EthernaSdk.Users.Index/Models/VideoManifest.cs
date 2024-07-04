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
    public class VideoManifest
    {
        // Constructor.
        public VideoManifest(
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
            AspectRatio = aspectRatio;
            BatchId = batchId;
            CreatedAt = createdAt;
            Description = description;
            Duration = duration;
            Title = title;
            OwnerAddress = ownerAddress;
            PersonalData = personalData;
            Sources = sources;
            Thumbnail = thumbnail;
            UpdatedAt = updatedAt;
        }
        
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
        public float AspectRatio { get; }
        public PostageBatchId? BatchId { get; }
        public DateTimeOffset CreatedAt { get; }
        public string Description { get; }
        public TimeSpan Duration { get; }
        public string Title { get; }
        public string OwnerAddress { get; }
        public string? PersonalData { get; }
        public IEnumerable<VideoManifestVideoSource> Sources { get; }
        public VideoManifestImage? Thumbnail { get; }
        public DateTimeOffset? UpdatedAt { get; }
    }
}