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
using Etherna.Sdk.Users.Index.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.Sdk.Users.Index.Clients
{
    public class EthernaUserIndexClient : IEthernaUserIndexClient
    {
        // Fields.
        private readonly CommentsClient generatedCommentsClient;
        private readonly ModerationClient generatedModerationClient;
        private readonly SearchClient generatedSearchClient;
        private readonly SystemClient generatedSystemClient;
        private readonly UsersClient generatedUsersClient;
        private readonly VideosClient generatedVideosClient;

        // Constructor.
        public EthernaUserIndexClient(
            Uri baseUrl,
            HttpClient httpClient)
        {
            ArgumentNullException.ThrowIfNull(baseUrl, nameof(baseUrl));

            generatedCommentsClient = new CommentsClient(baseUrl.AbsoluteUri, httpClient);
            generatedModerationClient = new ModerationClient(baseUrl.AbsoluteUri, httpClient);
            generatedSearchClient = new SearchClient(baseUrl.AbsoluteUri, httpClient);
            generatedSystemClient = new SystemClient(baseUrl.AbsoluteUri, httpClient);
            generatedUsersClient = new UsersClient(baseUrl.AbsoluteUri, httpClient);
            generatedVideosClient = new VideosClient(baseUrl.AbsoluteUri, httpClient);
        }

        // Methods.
        public Task AdminForceNewValidationByManifestHashAsync(
            SwarmHash manifestHash,
            CancellationToken cancellationToken = default) =>
            generatedSystemClient.ManifestAsync(manifestHash.ToString(), cancellationToken);

        public Task AdminForceNewValidationByVideoIdAsync(
            string videoId,
            CancellationToken cancellationToken = default) =>
            generatedSystemClient.VideoAsync(videoId, cancellationToken);

        public Task AdminReindexAllVideosAsync(CancellationToken cancellationToken = default) =>
            generatedSearchClient.ReindexAsync(cancellationToken);

        public async Task<Comment> CreateCommentAsync(
            string videoId,
            string commentText,
            CancellationToken cancellationToken = default) =>
            new(await generatedVideosClient.Comments2PostAsync(videoId, commentText, cancellationToken).ConfigureAwait(false));

        public Task DeleteOwnedCommentAsync(string commentId, CancellationToken cancellationToken = default) =>
            generatedCommentsClient.CommentsAsync(commentId, cancellationToken);

        public async Task<IEnumerable<VideoValidationStatus>> GetBulkVideoValidationStatusByIdsAsync(
            IEnumerable<string> videoIds,
            CancellationToken cancellationToken = default) =>
            (await generatedVideosClient.BulkValidation2Async(videoIds, cancellationToken).ConfigureAwait(false)).Select(s => new VideoValidationStatus(s));

        public async Task<IEnumerable<VideoValidationStatus>> GetBulkVideoValidationStatusByManifestsAsync(
            IEnumerable<SwarmHash> manifestHashes,
            CancellationToken cancellationToken = default) =>
            (await generatedVideosClient.BulkValidationPut2Async(
                manifestHashes.Select(h => h.ToString()), cancellationToken).ConfigureAwait(false)).Select(s => new VideoValidationStatus(s));

        public async Task<PaginatedResult<VideoPreview>> GetLastPublishedVideosAsync(int? page = null, int? take = null, CancellationToken cancellationToken = default)
        {
            var result = await generatedVideosClient.Latest3Async(page, take, cancellationToken).ConfigureAwait(false);
            return new PaginatedResult<VideoPreview>(
                result.Elements.Select(v => new VideoPreview(v)),
                result.TotalElements,
                result.PageSize,
                result.CurrentPage,
                result.MaxPage);
        }

        public async Task<PaginatedResult<IndexUserInfo>> GetRegisteredUsersAsync(int? page = null, int? take = null, CancellationToken cancellationToken = default)
        {
            var result = await generatedUsersClient.List2Async(page, take, cancellationToken).ConfigureAwait(false);
            return new PaginatedResult<IndexUserInfo>(
                result.Elements.Select(u => new IndexUserInfo(u)),
                result.TotalElements,
                result.PageSize,
                result.CurrentPage,
                result.MaxPage);
        }

        public async Task<IndexParameters> GetIndexParametersAsync(CancellationToken cancellationToken = default) =>
            new(await generatedSystemClient.ParametersAsync(cancellationToken).ConfigureAwait(false));

        public async Task<IndexUserInfo> GetUserInfoByAddressAsync(string address, CancellationToken cancellationToken = default) =>
            new(await generatedUsersClient.UsersGetAsync(address, cancellationToken).ConfigureAwait(false));

        public async Task<Video> GetVideoByIdAsync(string videoId, CancellationToken cancellationToken = default) =>
            new(await generatedVideosClient.Find2Async(videoId, cancellationToken).ConfigureAwait(false));

        public async Task<Video> GetVideoByManifestAsync(SwarmHash manifestHash, CancellationToken cancellationToken = default) =>
            new(await generatedVideosClient.Manifest2Async(manifestHash.ToString(), cancellationToken).ConfigureAwait(false));

        public async Task<PaginatedResult<Comment>> GetVideoCommentsAsync(string videoId, int? page = null, int? take = null,
            CancellationToken cancellationToken = default)
        {
            var result = await generatedVideosClient.Comments3Async(videoId, page, take, cancellationToken).ConfigureAwait(false);
            return new PaginatedResult<Comment>(
                result.Elements.Select(c => new Comment(c)),
                result.TotalElements,
                result.PageSize,
                result.CurrentPage,
                result.MaxPage);
        }

        public async Task<PaginatedResult<Video>> GetVideosByOwnerAsync(string userAddress, int? page = null,
            int? take = null,
            CancellationToken cancellationToken = default)
        {
            var result = await generatedUsersClient.Videos3Async(userAddress, page, take, cancellationToken).ConfigureAwait(false);
            return new PaginatedResult<Video>(
                result.Elements.Select(v => new Video(v)),
                result.TotalElements,
                result.PageSize,
                result.CurrentPage,
                result.MaxPage);
        }

        public async Task<IEnumerable<VideoValidationStatus>> GetVideoValidationStatusByIdAsync(string videoId, CancellationToken cancellationToken = default) =>
            (await generatedVideosClient.Validation2Async(videoId, cancellationToken).ConfigureAwait(false)).Select(v => new VideoValidationStatus(v));

        public async Task<VideoValidationStatus> GetVideoValidationStatusByManifestAsync(SwarmHash manifestHash, CancellationToken cancellationToken = default) =>
            new VideoValidationStatus(await generatedVideosClient.ValidationGet2Async(manifestHash.ToString(), cancellationToken).ConfigureAwait(false));

        public Task ModerateCommentAsync(string commentId, CancellationToken cancellationToken = default) =>
            generatedModerationClient.CommentsAsync(commentId, cancellationToken);

        public Task ModerateVideoAsync(string videoId, CancellationToken cancellationToken = default) =>
            generatedModerationClient.VideosAsync(videoId, cancellationToken);

        public Task OwnerRemoveVideoAsync(string videoId, CancellationToken cancellationToken = default) =>
            generatedVideosClient.VideosDeleteAsync(videoId, cancellationToken);

        public Task<string> PublishNewVideoAsync(SwarmHash manifestHash, CancellationToken cancellationToken = default) =>
            generatedVideosClient.VideosPostAsync(new VideoCreateInput { ManifestHash = manifestHash.ToString() }, cancellationToken);

        public Task ReportUnsuitableVideoAsync(string videoId, SwarmHash manifestHash, string description, CancellationToken cancellationToken = default) =>
            generatedVideosClient.ReportsAsync(videoId, manifestHash.ToString(), description, cancellationToken);

        public Task<PaginatedResult<VideoPreview>> SearchVideosAsync(string? query = null, int? page = null, int? take = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateOwnedVideoCommentAsync(string commentId, string newCommentText, CancellationToken cancellationToken = default) =>
            generatedVideosClient.CommentsPutAsync(commentId, newCommentText, cancellationToken);

        public Task UpdateVideoManifestAsync(string videoId, SwarmHash newManifestHash, CancellationToken cancellationToken = default) =>
            generatedVideosClient.Update2Async(videoId, newManifestHash.ToString(), cancellationToken);

        public Task VotesVideoAsync(string id, VoteValue value, CancellationToken cancellationToken = default) =>
            generatedVideosClient.VotesAsync(id, Enum.Parse<Value>(value.ToString()), cancellationToken);
    }
}
