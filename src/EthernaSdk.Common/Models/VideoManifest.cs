// Copyright 2020-present Etherna SA
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Etherna.Sdk.Common.GenClients.Index;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.Sdk.Common.Models
{
    public class VideoManifest
    {
        // Constructors.
        internal VideoManifest(VideoManifest2Dto videoManifest)
        {
            AspectRatio = videoManifest.AspectRatio;
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
        public string? BatchId { get; }
        public long CreatedAt { get; }
        public string? Description { get; }
        public long? Duration { get; }
        public string Hash { get; }
        public string? PersonalData { get; }
        public IEnumerable<VideoSource> Sources { get; }
        public Image Thumbnail { get; }
        public string? Title { get; }
        public long? UpdatedAt { get; }
    }
}