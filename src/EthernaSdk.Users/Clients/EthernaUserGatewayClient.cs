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

using Etherna.Sdk.Common.GenClients.Gateway;
using Etherna.Sdk.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.Sdk.Users.Clients
{
    public class EthernaUserGatewayClient : IEthernaUserGatewayClient
    {
        // Fields.
        private readonly PostageClient generatedPostageClient;
        private readonly ResourcesClient generatedResourcesClient;
        private readonly SystemClient generatedSystemClient;
        private readonly UsersClient generatedUsersClient;

        // Constructor.
        public EthernaUserGatewayClient(
            Uri baseUrl,
            HttpClient httpClient)
        {
            ArgumentNullException.ThrowIfNull(baseUrl, nameof(baseUrl));
            
            generatedPostageClient = new(baseUrl.AbsoluteUri, httpClient);
            generatedResourcesClient = new(baseUrl.AbsoluteUri, httpClient);
            generatedSystemClient = new(baseUrl.AbsoluteUri, httpClient);
            generatedUsersClient = new(baseUrl.AbsoluteUri, httpClient);
        }

        // Properties.
        public Task AdminSetFreeResourcePinningAsync(
            string resourceHash,
            DateTimeOffset? freePinEndOfLife = null,
            CancellationToken cancellationToken = default) =>
            generatedResourcesClient.FreeAsync(resourceHash, freePinEndOfLife, cancellationToken);

        public Task<IDictionary<string, bool>> AreResourcesDownloadFundedAsync(
            IEnumerable<string> resourceHashes,
            CancellationToken cancellationToken = default) =>
            generatedResourcesClient.AreofferedAsync(resourceHashes, cancellationToken);

        public Task<string> CreateNewPostageBatchAsync(
            int depth,
            long amount,
            string? label = null,
            CancellationToken cancellationToken = default) =>
            generatedUsersClient.BatchesPostAsync(depth, amount, label, cancellationToken);

        public Task<bool> DefundResourceDownloadAsync(
            string resourceHash,
            CancellationToken cancellationToken = default) =>
            generatedResourcesClient.OffersDeleteAsync(resourceHash, cancellationToken);

        public Task<bool> DefundResourcePinningAsync(
            string resourceHash,
            CancellationToken cancellationToken = default) =>
            generatedResourcesClient.PinDeleteAsync(resourceHash, cancellationToken);

        public Task DilutePostageBatchAsync(
            string postageBatchId,
            int depth,
            CancellationToken cancellationToken = default) =>
            generatedUsersClient.DiluteAsync(postageBatchId, depth, cancellationToken);

        public Task FundResourceDownloadAsync(string resourceHash, CancellationToken cancellationToken = default) =>
            generatedResourcesClient.OffersPostAsync(resourceHash, cancellationToken);

        public Task FundResourcePinningAsync(string resourceHash, CancellationToken cancellationToken = default) =>
            generatedResourcesClient.PinPostAsync(resourceHash, cancellationToken);

        public async Task<ChainState> GetChainStateAsync(CancellationToken cancellationToken = default) =>
            new(await generatedSystemClient.ChainstateAsync(cancellationToken).ConfigureAwait(false));

        public async Task<UserInfo> GetCurrentUserAsync(CancellationToken cancellationToken = default) =>
            new(await generatedUsersClient.CurrentAsync(cancellationToken).ConfigureAwait(false));

        public async Task<UserCredit> GetCurrentUserCreditAsync(CancellationToken cancellationToken = default) =>
            new(await generatedUsersClient.CreditAsync(cancellationToken).ConfigureAwait(false));

        public Task<double> GetDownloadBytePriceAsync(CancellationToken cancellationToken = default) =>
            generatedSystemClient.BytepriceAsync(cancellationToken);

        public async Task<IEnumerable<string>> GetDownloadFundedResourcesByUserAsync(
            CancellationToken cancellationToken = default) =>
            await generatedUsersClient.OfferedResourcesAsync(cancellationToken).ConfigureAwait(false);

        public async Task<IEnumerable<string>> GetPinFundedResourcesAsync(
            CancellationToken cancellationToken = default) =>
            await generatedUsersClient.PinnedResourcesAsync(cancellationToken).ConfigureAwait(false);

        public async Task<PostageBatch> GetPostageBatchAsync(
            string batchId,
            CancellationToken cancellationToken = default) =>
            new(await generatedUsersClient.BatchesGetAsync(batchId, cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<PostageBatchRef>> GetOwnedPostageBatchesAsync(
            string? labelContainsFilter = null,
            CancellationToken cancellationToken = default) =>
            (await generatedUsersClient.BatchesGetSearchAsync(labelContainsFilter, cancellationToken).ConfigureAwait(false))
            .Select(pbr => new PostageBatchRef(pbr));

        public async Task<ResourcePinStatus> GetResourcePinStatusAsync(
            string resourceHash,
            CancellationToken cancellationToken = default) =>
            new(await generatedResourcesClient.PinGetAsync(resourceHash, cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<string>> GetUsersFundingResourceDownloadAsync(
            string resourceHash,
            CancellationToken cancellationToken = default) =>
            await generatedResourcesClient.OffersGetAsync(resourceHash, cancellationToken).ConfigureAwait(false);

        public async Task<IEnumerable<string>> GetUsersFundingResourcePinningAsync(
            string resourceHash,
            CancellationToken cancellationToken = default) =>
            await generatedResourcesClient.UsersAsync(resourceHash, cancellationToken).ConfigureAwait(false);

        public async Task<WelcomePack> GetWelcomePackInfoAsync(CancellationToken cancellationToken = default) =>
            new(await generatedUsersClient.WelcomeGetAsync(cancellationToken).ConfigureAwait(false));

        public Task RequireWelcomePackAsync(CancellationToken cancellationToken = default) =>
            generatedUsersClient.WelcomePostAsync(cancellationToken);

        public Task TopUpPostageBatchAsync(
            string postageBatchId,
            long amount,
            CancellationToken cancellationToken = default) =>
            generatedPostageClient.TopupAsync(postageBatchId, amount, cancellationToken);

        public async Task<string?> TryGetNewPostageBatchIdFromPostageRefAsync(
            string postageReferenceId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await generatedSystemClient.PostageBatchRefAsync(
                    postageReferenceId,
                    cancellationToken).ConfigureAwait(false);
            }
            catch (EthernaGatewayApiException e) when(e.StatusCode == 404)
            {
                return null;
            }
        }
    }
}
