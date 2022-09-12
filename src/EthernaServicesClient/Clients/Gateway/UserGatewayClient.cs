using System;
using System.Net.Http;

namespace Etherna.ServicesClient.Clients.Gateway
{
    class UserGatewayClient : IUserGatewayClient
    {
        // Fields.
        private readonly Uri baseUrl;
        private readonly HttpClient httpClient;

        // Constructor.
        public UserGatewayClient(
            Uri baseUrl,
            HttpClient httpClient)
        {
            this.baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        // Properties.
        public IPostageClient PostageClient => new PostageClient(baseUrl.AbsoluteUri, httpClient);
        public IResourcesClient ResourcesClient => new ResourcesClient(baseUrl.AbsoluteUri, httpClient);
        public ISystemClient SystemClient => new SystemClient(baseUrl.AbsoluteUri, httpClient);
        public IUsersClient UsersClient => new UsersClient(baseUrl.AbsoluteUri, httpClient);
    }
}
