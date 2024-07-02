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
using System;

namespace Etherna.Sdk.Index.Models
{
    public class Video
    {
        // Constructors.
        internal Video(Video2Dto video)
        {
            Id = video.Id;
            CreationDateTime = video.CreationDateTime;
            if (video.CurrentVoteValue.HasValue)
                CurrentVoteValue = Enum.Parse<VoteValue>(video.CurrentVoteValue.Value.ToString());
            if (video.LastValidManifest is not null)
                LastValidManifest = new(video.LastValidManifest);
            OwnerAddress = video.OwnerAddress;
            TotDownvotes = video.TotDownvotes;
            TotUpvotes = video.TotUpvotes;
        }
        
        // Properties.
        public string Id { get; }
        public DateTimeOffset CreationDateTime { get; }
        public VoteValue? CurrentVoteValue { get; }
        public VideoManifest? LastValidManifest { get; }
        public string OwnerAddress { get; }
        public long TotDownvotes { get; }
        public long TotUpvotes { get; }
    }
}