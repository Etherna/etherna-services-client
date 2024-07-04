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

namespace Etherna.Sdk.Users.Index.Models
{
    public class VideoPreview
    {
        // Constructors.
        internal VideoPreview(VideoPreviewDto videoPreview)
        {
            if (videoPreview.Hash is not null)
                Hash = videoPreview.Hash;
            Id = videoPreview.Id;
            CreatedAt = videoPreview.CreatedAt;
            Duration = videoPreview.Duration;
            OwnerAddress = videoPreview.OwnerAddress;
            Thumbnail = new VideoManifestImage(videoPreview.Thumbnail);
            Title = videoPreview.Title;
            UpdatedAt = videoPreview.UpdatedAt;
        }
        
        // Properties.
        public string Id { get; }
        public long? CreatedAt { get; }
        public long? Duration { get; }
        public SwarmHash? Hash { get; }
        public string OwnerAddress { get; }
        public VideoManifestImage Thumbnail { get; }
        public string? Title { get; }
        public long? UpdatedAt { get; }
    }
}