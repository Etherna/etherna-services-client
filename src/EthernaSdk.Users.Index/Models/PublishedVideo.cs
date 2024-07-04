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
using System;
using System.Linq;

namespace Etherna.Sdk.Users.Index.Models
{
    public class PublishedVideo
    {
        // Constructors.
        internal PublishedVideo(Video2Dto video)
        {
            Id = video.Id;
            CreationDateTime = video.CreationDateTime;
            if (video.CurrentVoteValue.HasValue)
                CurrentVoteValue = Enum.Parse<VoteValue>(video.CurrentVoteValue.Value.ToString());
            if (video.LastValidManifest is not null) //can be null, see: https://etherna.atlassian.net/browse/EID-229
                LastValidManifest = new PublishedVideoManifest(
                    hash: video.LastValidManifest.Hash,
                    manifest: new(
                        aspectRatio: video.LastValidManifest.AspectRatio,
                        batchId: video.LastValidManifest.BatchId is null ? (PostageBatchId?)null : video.LastValidManifest.BatchId,
                        createdAt: DateTimeOffset.FromUnixTimeSeconds(video.LastValidManifest.CreatedAt),
                        description: video.LastValidManifest.Description ?? "",
                        duration: TimeSpan.FromSeconds(video.LastValidManifest.Duration ?? 0),
                        title: video.LastValidManifest.Title ?? "",
                        video.OwnerAddress,
                        personalData: video.LastValidManifest.PersonalData,
                        sources: video.LastValidManifest.Sources.Select(s => new VideoManifestVideoSource(s)),
                        thumbnail: new VideoManifestImage(video.LastValidManifest.Thumbnail),
                        updatedAt: video.LastValidManifest.UpdatedAt is null ? null : DateTimeOffset.FromUnixTimeSeconds(video.LastValidManifest.UpdatedAt.Value)),
                    version: null);
            OwnerAddress = video.OwnerAddress;
            TotDownvotes = video.TotDownvotes;
            TotUpvotes = video.TotUpvotes;
        }
        
        // Properties.
        public string Id { get; }
        public DateTimeOffset CreationDateTime { get; }
        public VoteValue? CurrentVoteValue { get; }
        public PublishedVideoManifest? LastValidManifest { get; }
        public string OwnerAddress { get; }
        public long TotDownvotes { get; }
        public long TotUpvotes { get; }
    }
}