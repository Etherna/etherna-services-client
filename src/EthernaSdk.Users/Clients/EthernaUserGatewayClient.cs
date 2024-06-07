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

using Etherna.BeeNet;
using Etherna.BeeNet.Models;
using Etherna.Sdk.Common.GenClients.Gateway;
using Etherna.Sdk.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ChainState = Etherna.Sdk.Common.Models.ChainState;
using FileResponse = Etherna.BeeNet.Models.FileResponse;

namespace Etherna.Sdk.Users.Clients
{
    public sealed class EthernaUserGatewayClient : IEthernaUserGatewayClient, IDisposable
    {
        // Fields.
        private readonly BeeClient beeClient;
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

            beeClient = new BeeClient(baseUrl.ToString(), customHttpClient: httpClient);
            generatedPostageClient = new(baseUrl.AbsoluteUri, httpClient);
            generatedResourcesClient = new(baseUrl.AbsoluteUri, httpClient);
            generatedSystemClient = new(baseUrl.AbsoluteUri, httpClient);
            generatedUsersClient = new(baseUrl.AbsoluteUri, httpClient);
        }

        // Dispose.
        public void Dispose()
        {
            beeClient.Dispose();
        }

        // Properties.
        public Task AdminSetFreeResourcePinningAsync(
            SwarmAddress address,
            DateTimeOffset? freePinEndOfLife = null,
            CancellationToken cancellationToken = default) =>
            generatedResourcesClient.FreeAsync(address.ToString(), freePinEndOfLife, cancellationToken);

        public async Task<IDictionary<SwarmAddress, bool>> AreResourcesDownloadFundedAsync(
            IEnumerable<SwarmAddress> resourceAddresses,
            CancellationToken cancellationToken = default) =>
            (await generatedResourcesClient.AreofferedAsync(
                resourceAddresses.Select(a => a.ToString()),
                cancellationToken).ConfigureAwait(false))
            .ToDictionary(pair => new SwarmAddress(pair.Key), pair => pair.Value);

        public Task<string> BuyPostageBatchAsync(
            long amount,
            int depth,
            string? label = null,
            CancellationToken cancellationToken = default) =>
            generatedUsersClient.BatchesPostAsync(depth, amount, label, cancellationToken);

        public Task<SwarmAddress> CreateFeedAsync(
            string owner,
            string topic,
            PostageBatchId batchId,
            string? type = null,
            bool swarmPin = false,
            CancellationToken cancellationToken = default) =>
            beeClient.CreateFeedAsync(owner, topic, batchId, type, swarmPin, cancellationToken);

        public Task<bool> DefundResourceDownloadAsync(
            SwarmAddress address,
            CancellationToken cancellationToken = default) =>
            generatedResourcesClient.OffersDeleteAsync(address.ToString(), cancellationToken);

        public Task<bool> DefundResourcePinningAsync(
            SwarmAddress address,
            CancellationToken cancellationToken = default) =>
            generatedResourcesClient.PinDeleteAsync(address.ToString(), cancellationToken);

        public Task DilutePostageBatchAsync(
            PostageBatchId batchId,
            int depth,
            CancellationToken cancellationToken = default) =>
            generatedUsersClient.DiluteAsync(batchId.ToString(), depth, cancellationToken);

        public Task FundResourceDownloadAsync(SwarmAddress address, CancellationToken cancellationToken = default) =>
            generatedResourcesClient.OffersPostAsync(address.ToString(), cancellationToken);

        public Task FundResourcePinningAsync(SwarmAddress address, CancellationToken cancellationToken = default) =>
            generatedResourcesClient.PinPostAsync(address.ToString(), cancellationToken);

        public Task<Stream> GetBytesAsync(
            SwarmAddress address,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default) =>
            beeClient.GetBytesAsync(address, swarmCache, swarmRedundancyStrategy, swarmRedundancyFallbackMode, swarmChunkRetrievalTimeout, cancellationToken);

        public async Task<ChainState> GetChainStateAsync(CancellationToken cancellationToken = default) =>
            new(await generatedSystemClient.ChainstateAsync(cancellationToken).ConfigureAwait(false));

        public Task<Stream> GetChunkAsync(
            SwarmAddress address,
            bool? swarmCache = null,
            CancellationToken cancellationToken = default) =>
            beeClient.GetChunkStreamAsync(address, swarmCache, cancellationToken);

        public async Task<UserCredit> GetCurrentUserCreditAsync(CancellationToken cancellationToken = default) =>
            new(await generatedUsersClient.CreditAsync(cancellationToken).ConfigureAwait(false));

        public Task<double> GetDownloadBytePriceAsync(CancellationToken cancellationToken = default) =>
            generatedSystemClient.BytepriceAsync(cancellationToken);

        public async Task<IEnumerable<SwarmAddress>> GetDownloadFundedResourcesByUserAsync(
            CancellationToken cancellationToken = default) =>
            (await generatedUsersClient.OfferedResourcesAsync(cancellationToken).ConfigureAwait(false))
            .Select(a => new SwarmAddress(a));

        public async Task<SwarmAddress> GetFeedAsync(
            string owner,
            string topic,
            int? at = null,
            int? after = null,
            string? type = null,
            CancellationToken cancellationToken = default) =>
            await beeClient.GetFeedAsync(owner, topic, at, after, type, cancellationToken).ConfigureAwait(false);

        public Task<FileResponse> GetFileAsync(
            SwarmAddress address,
            string? path = null,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default) => path is null
            ? beeClient.GetFileAsync(address, swarmCache, swarmRedundancyStrategy,
                swarmRedundancyFallbackMode, swarmChunkRetrievalTimeout, cancellationToken)
            : beeClient.GetFileWithPathAsync(address, path, swarmRedundancyStrategy,
                swarmRedundancyFallbackMode, swarmChunkRetrievalTimeout, cancellationToken);

        public async Task<IEnumerable<SwarmAddress>> GetPinFundedResourcesAsync(
            CancellationToken cancellationToken = default) =>
            (await generatedUsersClient.PinnedResourcesAsync(cancellationToken).ConfigureAwait(false))
            .Select(a => new SwarmAddress(a));

        public async Task<PostageBatch> GetPostageBatchAsync(
            PostageBatchId batchId,
            CancellationToken cancellationToken = default)
        {
            var batchDto = await generatedUsersClient.BatchesGetAsync(
                batchId.ToString(), cancellationToken).ConfigureAwait(false);
            return new PostageBatch(
                batchDto.Id,
                batchDto.Value ?? 0,
                batchDto.BlockNumber ?? 0,
                batchDto.Depth,
                batchDto.Exists ?? false,
                batchDto.ImmutableFlag ?? false,
                batchDto.Usable,
                batchDto.Label,
                TimeSpan.FromSeconds(batchDto.BatchTTL ?? 0),
                (uint)(batchDto.Utilization ?? 0));
        }

        public async Task<IEnumerable<PostageBatchRef>> GetOwnedPostageBatchesAsync(
            string? labelContainsFilter = null,
            CancellationToken cancellationToken = default) =>
            (await generatedUsersClient.BatchesGetSearchAsync(labelContainsFilter, cancellationToken).ConfigureAwait(false))
            .Select(pbr => new PostageBatchRef(pbr));

        public async Task<ResourcePinStatus> GetResourcePinStatusAsync(
            SwarmAddress address,
            CancellationToken cancellationToken = default) =>
            new(await generatedResourcesClient.PinGetAsync(address.ToString(), cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<string>> GetUsersFundingResourceDownloadAsync(
            SwarmAddress address,
            CancellationToken cancellationToken = default) =>
            await generatedResourcesClient.OffersGetAsync(address.ToString(), cancellationToken).ConfigureAwait(false);

        public async Task<IEnumerable<string>> GetUsersFundingResourcePinningAsync(
            SwarmAddress address,
            CancellationToken cancellationToken = default) =>
            await generatedResourcesClient.UsersAsync(address.ToString(), cancellationToken).ConfigureAwait(false);

        public async Task<WelcomePack> GetWelcomePackInfoAsync(CancellationToken cancellationToken = default) =>
            new(await generatedUsersClient.WelcomeGetAsync(cancellationToken).ConfigureAwait(false));

        public Task RequireWelcomePackAsync(CancellationToken cancellationToken = default) =>
            generatedUsersClient.WelcomePostAsync(cancellationToken);

        public Task SendPssAsync(
            string topic,
            string targets,
            PostageBatchId batchId,
            string? recipient = null,
            CancellationToken cancellationToken = default) =>
            beeClient.SendPssAsync(topic, targets, batchId, recipient, cancellationToken);

        public Task TopUpPostageBatchAsync(
            PostageBatchId batchId,
            long amount,
            CancellationToken cancellationToken = default) =>
            generatedPostageClient.TopupAsync(batchId.ToString(), amount, cancellationToken);

        public async Task<PostageBatchId?> TryGetNewPostageBatchIdFromPostageRefAsync(
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

        public Task<SwarmAddress> UploadChunkAsync(
            PostageBatchId batchId,
            bool swarmPin = false,
            bool swarmDeferredUpload = true,
            Stream? body = null,
            CancellationToken cancellationToken = default) =>
            beeClient.UploadChunkAsync(
                batchId,
                swarmPin: swarmPin,
                swarmDeferredUpload: swarmDeferredUpload,
                body: body,
                cancellationToken: cancellationToken);

        public Task<SwarmAddress> UploadBytesAsync(
            PostageBatchId batchId,
            Stream content,
            bool swarmPin = false,
            bool swarmDeferredUpload = true,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
            CancellationToken cancellationToken = default) =>
            beeClient.UploadBytesAsync(
                batchId,
                content,
                swarmPin: swarmPin,
                swarmDeferredUpload: swarmDeferredUpload,
                swarmRedundancyLevel: swarmRedundancyLevel,
                cancellationToken: cancellationToken);

        public Task<SwarmAddress> UploadFileAsync(
            PostageBatchId batchId,
            Stream content,
            string? name = null,
            string? contentType = null,
            bool swarmPin = false,
            bool swarmDeferredUpload = true,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
            CancellationToken cancellationToken = default) =>
            beeClient.UploadFileAsync(
                batchId,
                content,
                name: name,
                contentType: contentType,
                swarmPin: swarmPin,
                swarmDeferredUpload: swarmDeferredUpload,
                swarmRedundancyLevel: swarmRedundancyLevel,
                cancellationToken: cancellationToken);

        public Task<SwarmAddress> UploadSocAsync(
            string owner,
            string id,
            string signature,
            PostageBatchId batchId,
            Stream content,
            bool swarmPin = false,
            CancellationToken cancellationToken = default) =>
            beeClient.UploadSocAsync(
                owner: owner,
                id: id,
                sig: signature,
                batchId: batchId,
                content,
                swarmPin: swarmPin,
                cancellationToken: cancellationToken);
    }
}
