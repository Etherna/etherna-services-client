//   Copyright 2020-present Etherna Sagl
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

using Etherna.ServicesClient.GeneratedClients.Index;
using System;
using System.Net.Http;

namespace Etherna.ServicesClient.Users
{
    public class EthernaUserIndexClient : IEthernaUserIndexClient
    {
        // Fields.
        private readonly Uri baseUrl;
        private readonly HttpClient httpClient;

        // Constructor.
        public EthernaUserIndexClient(
            Uri baseUrl,
            HttpClient httpClient)
        {
            this.baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        // Properties.
        public ICommentsClient CommentsClient => new CommentsClient(baseUrl.AbsoluteUri, httpClient);
        public IModerationClient ModerationClient => new ModerationClient(baseUrl.AbsoluteUri, httpClient);
        public ISearchClient SearchClient => new SearchClient(baseUrl.AbsoluteUri, httpClient);
        public ISystemClient SystemClient => new SystemClient(baseUrl.AbsoluteUri, httpClient);
        public IUsersClient UsersClient => new UsersClient(baseUrl.AbsoluteUri, httpClient);
        public IVideosClient VideosClient => new VideosClient(baseUrl.AbsoluteUri, httpClient);
    }
}
