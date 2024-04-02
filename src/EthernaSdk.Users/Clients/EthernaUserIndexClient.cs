//   Copyright 2020-present Etherna SA
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Etherna.Sdk.Common.GenClients.Index;
using Etherna.Sdk.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.Sdk.Users.Clients
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
            string manifestHash,
            CancellationToken cancellationToken = default) =>
            generatedSystemClient.ManifestAsync(manifestHash, cancellationToken);

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

        public Task<IEnumerable<VideoValidationStatus>> GetBulkVideoValidationStatusByManifestsAsync(IEnumerable<string> manifestHashes,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserInfo> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResult<VideoPreview>> GetLastPublishedVideosAsync(int? page = null, int? take = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResult<UserInfo>> GetRegisteredUsersAsync(int? page = null, int? take = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IndexParameters> GetIndexParametersAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserInfo> GetUserInfoByAddressAsync(string address, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Video> GetVideoByIdAsync(string videoId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Video> GetVideoByManifestAsync(string manifestHash, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResult<Comment>> GetVideoCommentsAsync(string videoId, int? page = null, int? take = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResult<Video>> GetVideosByOwnerAsync(string userAddress, int? page = null, int? take = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<VideoValidationStatus>> GetVideoValidationStatusByIdAsync(string videoId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<VideoValidationStatus> GetVideoValidationStatusByManifestAsync(string manifestHash, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ModerateCommentAsync(string commentId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ModerateVideoAsync(string videoId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task OwnerRemoveVideoAsync(string videoId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<string> PublishNewVideoAsync(string manifestHash, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ReportUnsuitableVideoAsync(string videoId, string manifestHash, string description,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedResult<VideoPreview>> SearchVideosAsync(string? query = null, int? page = null, int? take = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateOwnedVideoCommentAsync(string commentId, string newCommentText,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateVideoManifestAsync(string videoId, string newManifestHash, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task VotesVideoAsync(string id, VoteValue value, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
