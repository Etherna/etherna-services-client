using System;
using System.Net.Http;

namespace Etherna.ServicesClient.Clients.Index
{
    class UserIndexClient : IUserIndexClient
    {
        // Fields.
        private readonly Uri baseUrl;
        private readonly HttpClient httpClient;

        // Constructor.
        public UserIndexClient(
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
