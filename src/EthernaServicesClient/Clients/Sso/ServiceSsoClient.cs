using System;
using System.Net.Http;

namespace Etherna.ServicesClient.Clients.Sso
{
    public class ServiceSsoClient : IServiceSsoClient
    {
        // Fields.
        private readonly Uri baseUrl;
        private readonly Func<HttpClient> createHttpClient;

        // Constructor.
        public ServiceSsoClient(
            Uri baseUrl,
            Func<HttpClient> createHttpClient)
        {
            this.baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            this.createHttpClient = createHttpClient;
        }

        // Properties.
        public IServiceInteractClient ServiceInteract =>
            new ServiceInteractClient(baseUrl.AbsoluteUri, createHttpClient());
    }
}
