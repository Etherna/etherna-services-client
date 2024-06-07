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

using Etherna.BeeNet.Models;
using Etherna.Sdk.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ChainState = Etherna.Sdk.Common.Models.ChainState;
using EthernaGatewayApiException = Etherna.Sdk.Common.GenClients.Gateway.EthernaGatewayApiException;

namespace Etherna.Sdk.Users.Clients
{
    public interface IEthernaUserGatewayClient
    {
        /// <summary>
        /// Admins can set a free pin period for a resource
        /// </summary>
        /// <param name="address">The swarm resource hash</param>
        /// <param name="freePinEndOfLife">End of free period. Null for disable</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task AdminSetFreeResourcePinningAsync(
            SwarmAddress address,
            DateTimeOffset? freePinEndOfLife = null,
            CancellationToken cancellationToken = default);

        /// <param name="resourceAddresses">The swarm resource hashes list</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<IDictionary<SwarmAddress, bool>> AreResourcesDownloadFundedAsync(
            IEnumerable<SwarmAddress> resourceAddresses,
            CancellationToken cancellationToken = default);

        /// <summary>Buy a new postage batch.</summary>
        /// <param name="amount">New postage batch amount</param>
        /// <param name="depth">New postage batch depth</param>
        /// <param name="label">New postage batch label</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A temporary postage batch reference Id</returns>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<string> BuyPostageBatchAsync(
            long amount,
            int depth,
            string? label = null,
            CancellationToken cancellationToken = default);

        /// <summary>Create an initial feed root manifest</summary>
        /// <param name="owner">Owner</param>
        /// <param name="topic">Topic</param>
        /// <param name="batchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="type">Feed indexing scheme (default: sequence)</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <returns>Reference hash</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<SwarmAddress> CreateFeedAsync(
            string owner,
            string topic,
            PostageBatchId batchId,
            string? type = null,
            bool swarmPin = false,
            CancellationToken cancellationToken = default);

        /// <param name="address">The swarm resource hash</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<bool> DefundResourceDownloadAsync(
            SwarmAddress address,
            CancellationToken cancellationToken = default);

        /// <param name="address">The swarm resource hash</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<bool> DefundResourcePinningAsync(
            SwarmAddress address,
            CancellationToken cancellationToken = default);

        /// <param name="batchId">Postage batch Id</param>
        /// <param name="depth">New postage batch depth</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task DilutePostageBatchAsync(
            PostageBatchId batchId,
            int depth,
            CancellationToken cancellationToken = default);

        /// <param name="address">The swarm resource hash</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task FundResourceDownloadAsync(
            SwarmAddress address,
            CancellationToken cancellationToken = default);

        /// <param name="address">The swarm resource hash</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task FundResourcePinningAsync(
            SwarmAddress address,
            CancellationToken cancellationToken = default);
        
        /// <summary>Get referenced data</summary>
        /// <param name="address">Swarm address reference to content</param>
        /// <param name="swarmCache">Determines if the download data should be cached on the node. By default the download will be cached</param>
        /// <param name="swarmRedundancyStrategy">Specify the retrieve strategy on redundant data. The numbers stand for NONE, DATA, PROX and RACE, respectively. Strategy NONE means no prefetching takes place. Strategy DATA means only data chunks are prefetched. Strategy PROX means only chunks that are close to the node are prefetched. Strategy RACE means all chunks are prefetched: n data chunks and k parity chunks. The first n chunks to arrive are used to reconstruct the file. Multiple strategies can be used in a fallback cascade if the swarm redundancy fallback mode is set to true. The default strategy is NONE, DATA, falling back to PROX, falling back to RACE</param>
        /// <param name="swarmRedundancyFallbackMode">Specify if the retrieve strategies (chunk prefetching on redundant data) are used in a fallback cascade. The default is true.</param>
        /// <param name="swarmChunkRetrievalTimeout">Specify the timeout for chunk retrieval. The default is 30 seconds.</param>
        /// <returns>Retrieved content specified by reference</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<Stream> GetBytesAsync(
            SwarmAddress address,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<ChainState> GetChainStateAsync(CancellationToken cancellationToken = default);

        /// <summary>Get Chunk</summary>
        /// <param name="address">Swarm address of chunk</param>
        /// <returns>Retrieved chunk content</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<Stream> GetChunkAsync(
            SwarmAddress address,
            bool? swarmCache = null,
            CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<UserCredit> GetCurrentUserCreditAsync(CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<double> GetDownloadBytePriceAsync(CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<SwarmAddress>> GetDownloadFundedResourcesByUserAsync(CancellationToken cancellationToken = default);

        /// <summary>Find feed update</summary>
        /// <param name="owner">Owner</param>
        /// <param name="topic">Topic</param>
        /// <param name="at">Timestamp of the update (default: now)</param>
        /// <param name="after">Start index (default: 0)</param>
        /// <param name="type">Feed indexing scheme (default: sequence)</param>
        /// <returns>Latest feed update</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<SwarmAddress> GetFeedAsync(
            string owner,
            string topic,
            int? at = null,
            int? after = null,
            string? type = null,
            CancellationToken cancellationToken = default);

        /// <summary>Get file or index document from a collection of files</summary>
        /// <param name="address">Swarm address of content</param>
        /// <param name="path">Path to the file in the collection.</param>
        /// <param name="swarmCache">Determines if the download data should be cached on the node. By default the download will be cached</param>
        /// <param name="swarmRedundancyStrategy">Specify the retrieve strategy on redundant data. The numbers stand for NONE, DATA, PROX and RACE, respectively. Strategy NONE means no prefetching takes place. Strategy DATA means only data chunks are prefetched. Strategy PROX means only chunks that are close to the node are prefetched. Strategy RACE means all chunks are prefetched: n data chunks and k parity chunks. The first n chunks to arrive are used to reconstruct the file. Multiple strategies can be used in a fallback cascade if the swarm redundancy fallback mode is set to true. The default strategy is NONE, DATA, falling back to PROX, falling back to RACE</param>
        /// <param name="swarmRedundancyFallbackMode">Specify if the retrieve strategies (chunk prefetching on redundant data) are used in a fallback cascade. The default is true.</param>
        /// <param name="swarmChunkRetrievalTimeout">Specify the timeout for chunk retrieval. The default is 30 seconds.</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<FileResponse> GetFileAsync(
            SwarmAddress address,
            string? path = null,
            bool? swarmCache = null,
            RedundancyStrategy? swarmRedundancyStrategy = null,
            bool? swarmRedundancyFallbackMode = null,
            string? swarmChunkRetrievalTimeout = null,
            CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<SwarmAddress>> GetPinFundedResourcesAsync(CancellationToken cancellationToken = default);

        /// <param name="batchId">Postage batch Id</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<PostageBatch> GetPostageBatchAsync(
            PostageBatchId batchId,
            CancellationToken cancellationToken = default);

        /// <param name="labelContainsFilter">Filter only postage batches with label containing this string. Optional</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<PostageBatchRef>> GetOwnedPostageBatchesAsync(
            string? labelContainsFilter = null,
            CancellationToken cancellationToken = default);

        /// <param name="address">The swarm resource hash</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<ResourcePinStatus> GetResourcePinStatusAsync(
            SwarmAddress address,
            CancellationToken cancellationToken = default);

        /// <param name="address">The swarm resource hash</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<string>> GetUsersFundingResourceDownloadAsync(
            SwarmAddress address,
            CancellationToken cancellationToken = default);

        /// <param name="address">The swarm resource hash</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<IEnumerable<string>> GetUsersFundingResourcePinningAsync(
            SwarmAddress address,
            CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<WelcomePack> GetWelcomePackInfoAsync(CancellationToken cancellationToken = default);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task RequireWelcomePackAsync(CancellationToken cancellationToken = default);

        /// <summary>Send to recipient or target with Postal Service for Swarm</summary>
        /// <param name="topic">Topic name</param>
        /// <param name="targets">Target message address prefix. If multiple targets are specified, only one would be matched.</param>
        /// <param name="batchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="recipient">Recipient publickey</param>
        /// <returns>Subscribed to topic</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task SendPssAsync(
            string topic,
            string targets,
            PostageBatchId batchId,
            string? recipient = null,
            CancellationToken cancellationToken = default);

        /// <param name="batchId">The postage batch Id</param>
        /// <param name="amount">The amount to top up</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task TopUpPostageBatchAsync(
            PostageBatchId batchId,
            long amount,
            CancellationToken cancellationToken = default);

        /// <param name="postageReferenceId">Postage batch reference Id</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>New postage batch Id</returns>
        /// <exception cref="EthernaGatewayApiException">A server side error occurred.</exception>
        Task<PostageBatchId?> TryGetNewPostageBatchIdFromPostageRefAsync(
            string postageReferenceId,
            CancellationToken cancellationToken = default);
        
        /// <summary>Upload Chunk</summary>
        /// <param name="batchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="swarmTag">Associate upload with an existing Tag UID</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarmDeferredUpload">Determines if the uploaded data should be sent to the network immediately or in a deferred fashion. By default the upload will be deferred.</param>
        /// <returns>Ok</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<SwarmAddress> UploadChunkAsync(
            PostageBatchId batchId,
            bool swarmPin = false,
            bool swarmDeferredUpload = true,
            Stream? body = null,
            CancellationToken cancellationToken = default);

        /// <summary>Upload data</summary>
        /// <param name="batchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="swarmTag">Associate upload with an existing Tag UID</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarmEncrypt">Represents the encrypting state of the file
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarmDeferredUpload">Determines if the uploaded data should be sent to the network immediately or in a deferred fashion. By default the upload will be deferred.</param>
        /// <returns>Reference hash</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<SwarmAddress> UploadBytesAsync(
            PostageBatchId batchId,
            Stream content,
            bool swarmPin = false, 
            bool swarmDeferredUpload = true,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
            CancellationToken cancellationToken = default);

        /// <summary>Upload file or a collection of files</summary>
        /// <param name="batchId">ID of Postage Batch that is used to upload data with</param>
        /// <param name="content">Input file content</param>
        /// <param name="name">Filename when uploading single file</param>
        /// <param name="contentType">The specified content-type is preserved for download of the asset</param>
        /// <param name="swarmTag">Associate upload with an existing Tag UID</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarmEncrypt">Represents the encrypting state of the file
        /// <br/>Warning! Not available for nodes that run in Gateway mode!</param>
        /// <param name="swarmCollection">Upload file/files as a collection</param>
        /// <param name="swarmIndexDocument">Default file to be referenced on path, if exists under that path</param>
        /// <param name="swarmErrorDocument">Configure custom error document to be returned when a specified path can not be found in collection</param>
        /// <param name="swarmDeferredUpload">Determines if the uploaded data should be sent to the network immediately or in a deferred fashion. By default the upload will be deferred.</param>
        /// <param name="swarmRedundancyLevel">Add redundancy to the data being uploaded so that downloaders can download it with better UX. 0 value is default and does not add any redundancy to the file.</param>
        /// <returns>Reference hash</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<SwarmAddress> UploadFileAsync(
            PostageBatchId batchId,
            Stream content,
            string? name = null,
            string? contentType = null,
            bool swarmPin = false,
            bool swarmDeferredUpload = true,
            RedundancyLevel swarmRedundancyLevel = RedundancyLevel.None,
            CancellationToken cancellationToken = default);

        /// <summary>Upload single owner chunk</summary>
        /// <param name="owner">Owner</param>
        /// <param name="id">Id</param>
        /// <param name="signature">Signature</param>
        /// <param name="content">The SOC binary data is composed of the span (8 bytes) and the at most 4KB payload.</param>
        /// <param name="swarmPin">Represents if the uploaded data should be also locally pinned on the node.</param>
        /// <returns>Reference hash</returns>
        /// <exception cref="BeeNetGatewayApiException">A server side error occurred.</exception>
        Task<SwarmAddress> UploadSocAsync(
            string owner,
            string id,
            string signature,
            PostageBatchId batchId,
            Stream content,
            bool swarmPin = false,
            CancellationToken cancellationToken = default);
    }
}