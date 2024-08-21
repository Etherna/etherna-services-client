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

using Etherna.BeeNet;
using Etherna.BeeNet.Models;
using Etherna.Sdk.Gateway.GenClients;
using Etherna.Sdk.Users.Gateway.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ChainState = Etherna.Sdk.Users.Gateway.Models.ChainState;
using FileResponse = Etherna.BeeNet.Models.FileResponse;

namespace Etherna.Sdk.Users.Gateway.Clients
{
    public sealed class EthernaUserGatewayClient : IEthernaUserGatewayClient
    {
        // Fields.
        private readonly PostageClient generatedPostageClient;
        private readonly ResourcesClient generatedResourcesClient;
        private readonly SystemClient generatedSystemClient;
        private readonly UsersClient generatedUsersClient;

        // Constructor.
        public EthernaUserGatewayClient(
            Uri baseUrl,
            IBeeClient beeClient,
            HttpClient httpClient)
        {
            ArgumentNullException.ThrowIfNull(baseUrl, nameof(baseUrl));

            BeeClient = beeClient;
            generatedPostageClient = new(baseUrl.AbsoluteUri, httpClient);
            generatedResourcesClient = new(baseUrl.AbsoluteUri, httpClient);
            generatedSystemClient = new(baseUrl.AbsoluteUri, httpClient);
            generatedUsersClient = new(baseUrl.AbsoluteUri, httpClient);
        }
        
        // Properties.
        public IBeeClient BeeClient { get; }

        // Methods.
        public Task AdminSetFreeResourcePinningAsync(
            SwarmHash hash,
            DateTimeOffset? freePinEndOfLife = null,
            CancellationToken cancellationToken = default) =>
            generatedResourcesClient.FreeAsync(hash.ToString(), freePinEndOfLife, cancellationToken);
        
        public Task AnnounceUploadAsync(
            SwarmHash hash,
            PostageBatchId batchId,
            CancellationToken cancellationToken = default) =>
            generatedResourcesClient.AnnounceUploadAsync(hash.ToString(), batchId.ToString(), cancellationToken);

        public async Task<IDictionary<SwarmHash, bool>> AreResourcesDownloadFundedAsync(
            IEnumerable<SwarmHash> resourceHashes,
            CancellationToken cancellationToken = default) =>
            (await generatedResourcesClient.AreofferedAsync(
                resourceHashes.Select(h => h.ToString()),
                cancellationToken).ConfigureAwait(false))
            .ToDictionary(pair => new SwarmHash(pair.Key), pair => pair.Value);

        public Task<string> BuyPostageBatchAsync(
            BzzBalance amount,
            int depth,
            string? label = null,
            CancellationToken cancellationToken = default) =>
            generatedUsersClient.BatchesPostAsync(depth, amount.ToPlurLong(), label, cancellationToken);

        public Task<SwarmHash> CreateFeedAsync(
            string owner,
            string topic,
            PostageBatchId batchId,
            string? type = null,
            bool swarmPin = false,
            CancellationToken cancellationToken = default) =>
            BeeClient.CreateFeedAsync(owner, topic, batchId, type, swarmPin, cancellationToken);

        public Task<bool> DefundResourceDownloadAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default) =>
            generatedResourcesClient.OffersDeleteAsync(hash.ToString(), cancellationToken);

        public Task<bool> DefundResourcePinningAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default) =>
            generatedResourcesClient.PinDeleteAsync(hash.ToString(), cancellationToken);

        public Task DilutePostageBatchAsync(
            PostageBatchId batchId,
            int depth,
            CancellationToken cancellationToken = default) =>
            generatedUsersClient.DiluteAsync(batchId.ToString(), depth, cancellationToken);

        public Task FundResourceDownloadAsync(SwarmHash hash, CancellationToken cancellationToken = default) =>
            generatedResourcesClient.OffersPostAsync(hash.ToString(), cancellationToken);

        public Task FundResourcePinningAsync(SwarmHash hash, CancellationToken cancellationToken = default) =>
            generatedResourcesClient.PinPostAsync(hash.ToString(), cancellationToken);

        public Task<Stream> GetBytesAsync(
            SwarmHash hash,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default) =>
            BeeClient.GetBytesAsync(hash, swarmCache, swarmRedundancyStrategy, swarmRedundancyFallbackMode, swarmChunkRetrievalTimeout, cancellationToken);

        public async Task<ChainState> GetChainStateAsync(CancellationToken cancellationToken = default) =>
            new(await generatedSystemClient.ChainstateAsync(cancellationToken).ConfigureAwait(false));

        public Task<Stream> GetChunkAsync(
            SwarmHash hash,
            bool? swarmCache = null,
            CancellationToken cancellationToken = default) =>
            BeeClient.GetChunkStreamAsync(hash, swarmCache, cancellationToken);

        public async Task<UserCredit> GetCurrentUserCreditAsync(CancellationToken cancellationToken = default) =>
            new(await generatedUsersClient.CreditAsync(cancellationToken).ConfigureAwait(false));

        public Task<double> GetDownloadBytePriceAsync(CancellationToken cancellationToken = default) =>
            generatedSystemClient.BytepriceAsync(cancellationToken);

        public async Task<IEnumerable<SwarmHash>> GetDownloadFundedResourcesByUserAsync(
            CancellationToken cancellationToken = default) =>
            (await generatedUsersClient.OfferedResourcesAsync(cancellationToken).ConfigureAwait(false))
            .Select(hash => new SwarmHash(hash));

        public async Task<SwarmHash> GetFeedAsync(
            string owner,
            string topic,
            int? at = null,
            int? after = null,
            string? type = null,
            CancellationToken cancellationToken = default) =>
            await BeeClient.GetFeedAsync(owner, topic, at, after, type, cancellationToken).ConfigureAwait(false);

        public Task<FileResponse> GetFileAsync(
            SwarmAddress address,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default) =>
            BeeClient.GetFileAsync(address, swarmCache, swarmRedundancyStrategy,
                swarmRedundancyFallbackMode, swarmChunkRetrievalTimeout, cancellationToken);

        public async Task<IEnumerable<SwarmHash>> GetPinFundedResourcesAsync(
            CancellationToken cancellationToken = default) =>
            (await generatedUsersClient.PinnedResourcesAsync(cancellationToken).ConfigureAwait(false))
            .Select(h => new SwarmHash(h));

        public async Task<PostageBatch> GetPostageBatchAsync(
            PostageBatchId batchId,
            CancellationToken cancellationToken = default)
        {
            var batchDto = await generatedUsersClient.BatchesGetAsync(
                batchId.ToString(), cancellationToken).ConfigureAwait(false);
            return new PostageBatch(
                batchDto.Id,
                BzzBalance.FromPlurLong(batchDto.Value ?? 0),
                batchDto.BlockNumber ?? 0,
                batchDto.Depth,
                batchDto.Exists ?? false,
                batchDto.ImmutableFlag ?? false,
                batchDto.Usable,
                batchDto.Label,
                null,
                TimeSpan.FromSeconds(batchDto.BatchTTL ?? 0),
                (uint)(batchDto.Utilization ?? 0));
        }

        public async Task<IEnumerable<PostageBatchRef>> GetOwnedPostageBatchesAsync(
            string? labelContainsFilter = null,
            CancellationToken cancellationToken = default) =>
            (await generatedUsersClient.BatchesGetSearchAsync(labelContainsFilter, cancellationToken).ConfigureAwait(false))
            .Select(pbr => new PostageBatchRef(pbr));

        public async Task<ResourcePinStatus> GetResourcePinStatusAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default) =>
            new(await generatedResourcesClient.PinGetAsync(hash.ToString(), cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<string>> GetUsersFundingResourceDownloadAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default) =>
            await generatedResourcesClient.OffersGetAsync(hash.ToString(), cancellationToken).ConfigureAwait(false);

        public async Task<IEnumerable<string>> GetUsersFundingResourcePinningAsync(
            SwarmHash hash,
            CancellationToken cancellationToken = default) =>
            await generatedResourcesClient.UsersAsync(hash.ToString(), cancellationToken).ConfigureAwait(false);

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
            BeeClient.SendPssAsync(topic, targets, batchId, recipient, cancellationToken);

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

        public Task<SwarmHash> UploadChunkAsync(
            PostageBatchId batchId,
            Stream chunkData,
            bool swarmPin = false,
            bool swarmDeferredUpload = true,
            CancellationToken cancellationToken = default) =>
            BeeClient.UploadChunkAsync(
                batchId,
                chunkData,
                swarmPin: swarmPin,
                swarmDeferredUpload: swarmDeferredUpload,
                cancellationToken: cancellationToken);

        public Task<SwarmHash> UploadBytesAsync(
            PostageBatchId batchId,
            Stream content,
            bool swarmPin = false,
            bool swarmDeferredUpload = true,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
            CancellationToken cancellationToken = default) =>
            BeeClient.UploadBytesAsync(
                batchId,
                content,
                swarmPin: swarmPin,
                swarmDeferredUpload: swarmDeferredUpload,
                swarmRedundancyLevel: swarmRedundancyLevel,
                cancellationToken: cancellationToken);
        
#if NET7_0_OR_GREATER
        public Task<SwarmHash> UploadDirectoryAsync(
            PostageBatchId batchId,
            string directoryPath,
            bool swarmPin = false,
            bool swarmDeferredUpload = true,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
            CancellationToken cancellationToken = default) =>
            BeeClient.UploadDirectoryAsync(
                batchId,
                directoryPath,
                swarmPin: swarmPin,
                swarmDeferredUpload: swarmDeferredUpload,
                swarmRedundancyLevel: swarmRedundancyLevel,
                cancellationToken: cancellationToken);
#endif

        public Task<SwarmHash> UploadFileAsync(
            PostageBatchId batchId,
            Stream content,
            string? name = null,
            string? contentType = null,
            bool swarmPin = false,
            bool swarmDeferredUpload = true,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
            CancellationToken cancellationToken = default) =>
            BeeClient.UploadFileAsync(
                batchId,
                content,
                name: name,
                contentType: contentType,
                swarmPin: swarmPin,
                swarmDeferredUpload: swarmDeferredUpload,
                swarmRedundancyLevel: swarmRedundancyLevel,
                cancellationToken: cancellationToken);

        public Task<SwarmHash> UploadSocAsync(
            string owner,
            string id,
            string signature,
            PostageBatchId batchId,
            Stream content,
            bool swarmPin = false,
            CancellationToken cancellationToken = default) =>
            BeeClient.UploadSocAsync(
                owner: owner,
                id: id,
                sig: signature,
                batchId: batchId,
                content,
                swarmPin: swarmPin,
                cancellationToken: cancellationToken);
    }
}
