using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Etherna.ServicesClient.AspNetCore
{
    internal sealed class EthernaClientForServicesBuilder : IEthernaClientForServicesBuilder
    {
        // Constructor.
        public EthernaClientForServicesBuilder(
            string clientId,
            string clientName,
            string clientScope,
            string clientSecret,
            Uri ssoBaseUrl)
        {
            SsoBaseUrl = ssoBaseUrl;
            ClientId = clientId;
            ClientName = clientName;
            ClientSecret = clientSecret;
            ClientScope = clientScope;
        }

        // Properties.
        public string ClientId { get; }
        public string ClientName { get; }
        public string ClientScope { get; }
        public string ClientSecret { get; }
        public Uri SsoBaseUrl { get; }

        // Methods.
        public async Task<ClientCredentialsTokenRequest> GetClientCredentialsTokenRequestAsync(bool requireHttps = true)
        {
            // Discover endpoints from metadata.
            using var httpClient = new HttpClient();
            using var request = new DiscoveryDocumentRequest
            {
                Address = SsoBaseUrl.AbsoluteUri,
                Policy = new DiscoveryPolicy { RequireHttps = requireHttps }
            };

            var discoveryDoc = await httpClient.GetDiscoveryDocumentAsync(request).ConfigureAwait(false);
            if (discoveryDoc.IsError)
                throw discoveryDoc.Exception ?? new InvalidOperationException();

            // Return credentials.
            return new ClientCredentialsTokenRequest
            {
                Address = discoveryDoc.TokenEndpoint,
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                Scope = ClientScope
            };
        }
    }
}
