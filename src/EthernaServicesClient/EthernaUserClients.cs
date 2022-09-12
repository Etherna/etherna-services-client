using Etherna.ServicesClient.Clients.Credit;
using Etherna.ServicesClient.Clients.Gateway;
using Etherna.ServicesClient.Clients.Index;
using Etherna.ServicesClient.Clients.Sso;
using System;
using System.Net.Http;

namespace Etherna.ServicesClient
{
    public class EthernaUserClients : IEthernaUserClients
    {
        public EthernaUserClients(
            Uri creditServiceBaseUrl,
            Uri gatewayServiceBaseUrl,
            Uri indexServiceBaseUrl,
            Uri ssoServiceBaseUrl,
            Func<HttpClient> createHttpClient)
        {
            if (createHttpClient is null)
                throw new ArgumentNullException(nameof(createHttpClient));

            var httpClient = createHttpClient();

            CreditClient = new UserCreditClient(creditServiceBaseUrl, httpClient);
            GatewayClient = new UserGatewayClient(gatewayServiceBaseUrl, httpClient);
            IndexClient = new UserIndexClient(indexServiceBaseUrl, httpClient);
            SsoClient = new UserSsoClient(ssoServiceBaseUrl, httpClient);
        }

        public IUserCreditClient CreditClient { get; }
        public IUserGatewayClient GatewayClient { get; }
        public IUserIndexClient IndexClient { get; }
        public IUserSsoClient SsoClient { get; }
    }
}
