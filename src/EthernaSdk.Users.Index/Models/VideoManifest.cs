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
using Etherna.Sdk.Index.GenClients;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.Sdk.Users.Index.Models
{
    public class VideoManifest
    {
        // Constructors.
        internal VideoManifest(VideoManifest2Dto videoManifest)
        {
            AspectRatio = videoManifest.AspectRatio;
            if (videoManifest.BatchId is not null)
                BatchId = videoManifest.BatchId;
            CreatedAt = videoManifest.CreatedAt;
            Description = videoManifest.Description;
            Duration = videoManifest.Duration;
            Hash = videoManifest.Hash;
            PersonalData = videoManifest.PersonalData;
            Sources = videoManifest.Sources.Select(s => new VideoSource(s));
            Thumbnail = new Image(videoManifest.Thumbnail);
            Title = videoManifest.Title;
            UpdatedAt = videoManifest.UpdatedAt;
        }
        
        // Properties.
        public float AspectRatio { get; }
        public PostageBatchId? BatchId { get; }
        public long CreatedAt { get; }
        public string? Description { get; }
        public long? Duration { get; }
        public SwarmHash Hash { get; }
        public string? PersonalData { get; }
        public IEnumerable<VideoSource> Sources { get; }
        public Image Thumbnail { get; }
        public string? Title { get; }
        public long? UpdatedAt { get; }
    }
}