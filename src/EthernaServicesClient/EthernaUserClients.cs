using Etherna.ServicesClient.Clients.Credit;
using Etherna.ServicesClient.Clients.Sso;
using System;
using System.Net.Http;

namespace Etherna.ServicesClient
{
    public class EthernaUserClients : IEthernaUserClients
    {
        public EthernaUserClients(
            Uri creditServiceBaseUrl,
            Uri ssoServicebaseUrl,
            Func<HttpClient> createHttpClient)
        {
            if (createHttpClient is null)
                throw new ArgumentNullException(nameof(createHttpClient));

            var httpClient = createHttpClient();

            CreditClient = new UserCreditClient(creditServiceBaseUrl, httpClient);
            SsoClient = new UserSsoClient(ssoServicebaseUrl, httpClient);
        }

        public IUserCreditClient CreditClient { get; }
        public IUserSsoClient SsoClient { get; }
    }
}
