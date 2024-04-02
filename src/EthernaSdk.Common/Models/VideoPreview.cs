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

namespace Etherna.Sdk.Common.Models
{
    public class VideoPreview
    {
        // Constructors.
        internal VideoPreview(VideoPreviewDto videoPreview)
        {
            Id = videoPreview.Id;
            CreatedAt = videoPreview.CreatedAt;
            Duration = videoPreview.Duration;
            Hash = videoPreview.Hash;
            OwnerAddress = videoPreview.OwnerAddress;
            Thumbnail = new Image(videoPreview.Thumbnail);
            Title = videoPreview.Title;
            UpdatedAt = videoPreview.UpdatedAt;
        }
        
        // Properties.
        public string Id { get; }
        public long? CreatedAt { get; }
        public long? Duration { get; }
        public string? Hash { get; }
        public string OwnerAddress { get; }
        public Image Thumbnail { get; }
        public string? Title { get; }
        public long? UpdatedAt { get; }
    }
}