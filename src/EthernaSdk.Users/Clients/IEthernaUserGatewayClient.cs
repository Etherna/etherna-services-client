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

using Etherna.Sdk.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EthernaGatewayApiException = Etherna.Sdk.Common.GenClients.Gateway.EthernaGatewayApiException;

namespace Etherna.Sdk.Users.Clients
{
    public interface IEthernaUserGatewayClient
    {
        /// <summary>
        /// Admins can set a free pin period for a resource
        /// </summary>
        /// <param name="resourceHash">The swarm resource hash</param>
        /// <param name="freePinEndOfLife">End of free period. Null for disable</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task AdminSetFreeResourcePinningAsync(string resourceHash, DateTimeOffset? freePinEndOfLife = null, CancellationToken cancellationToken = default);

        /// <param name="resourceHashes">The swarm resource hashes list</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<IDictionary<string, bool>> AreResourcesDownloadFundedAsync(IEnumerable<string> resourceHashes, CancellationToken cancellationToken = default);

        /// <param name="depth">New postage batch depth</param>
        /// <param name="amount">New postage batch amount</param>
        /// <param name="label">New postage batch label</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A temporary postage batch reference Id</returns>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<string> CreateNewPostageBatchAsync(int depth, long amount, string? label = null, CancellationToken cancellationToken = default);

        /// <param name="resourceHash">The swarm resource hash</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<bool> DefundResourceDownloadAsync(string resourceHash, CancellationToken cancellationToken = default);

        /// <param name="resourceHash">The swarm resource hash</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<bool> DefundResourcePinningAsync(string resourceHash, CancellationToken cancellationToken = default);

        /// <param name="postageBatchId">Postage batch Id</param>
        /// <param name="depth">New postage batch depth</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task DilutePostageBatchAsync(string postageBatchId, int depth, CancellationToken cancellationToken = default);

        /// <param name="resourceHash">The swarm resource hash</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task FundResourceDownloadAsync(string resourceHash, CancellationToken cancellationToken = default);

        /// <param name="resourceHash">The swarm resource hash</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task FundResourcePinningAsync(string resourceHash, CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<ChainState> GetChainStateAsync(CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<UserCredit> GetCurrentUserCreditAsync(CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<double> GetDownloadBytePriceAsync(CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<string>> GetDownloadFundedResourcesByUserAsync(CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<string>> GetPinFundedResourcesAsync(CancellationToken cancellationToken = default);

        /// <param name="batchId">Postage batch Id</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<PostageBatch> GetPostageBatchAsync(string batchId, CancellationToken cancellationToken = default);

        /// <param name="labelContainsFilter">Filter only postage batches with label containing this string. Optional</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<PostageBatchRef>> GetOwnedPostageBatchesAsync(string? labelContainsFilter = null, CancellationToken cancellationToken = default);

        /// <param name="resourceHash">The swarm resource hash</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<ResourcePinStatus> GetResourcePinStatusAsync(string resourceHash, CancellationToken cancellationToken = default);

        /// <param name="resourceHash">The swarm resource hash</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<string>> GetUsersFundingResourceDownloadAsync(string resourceHash, CancellationToken cancellationToken = default);

        /// <param name="resourceHash">The swarm resource hash</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<string>> GetUsersFundingResourcePinningAsync(string resourceHash, CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<WelcomePack> GetWelcomePackInfoAsync(CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task RequireWelcomePackAsync(CancellationToken cancellationToken = default);

        /// <param name="postageBatchId">The postage batch Id</param>
        /// <param name="amount">The amount to top up</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task TopUpPostageBatchAsync(string postageBatchId, long amount, CancellationToken cancellationToken = default);

        /// <param name="postageReferenceId">Postage batch reference Id</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>New postage batch Id</returns>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<string?> TryGetNewPostageBatchIdFromPostageRefAsync(string postageReferenceId, CancellationToken cancellationToken = default);
    }
}