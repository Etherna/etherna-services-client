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

using System;
using System.Net.Http;

namespace Etherna.ServicesClient.Users
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

            CreditClient = new EthernaUserCreditClient(creditServiceBaseUrl, httpClient);
            GatewayClient = new EthernaUserGatewayClient(gatewayServiceBaseUrl, httpClient);
            IndexClient = new EthernaUserIndexClient(indexServiceBaseUrl, httpClient);
            SsoClient = new EthernaUserSsoClient(ssoServiceBaseUrl, httpClient);
        }

        public IEthernaUserCreditClient CreditClient { get; }
        public IEthernaUserGatewayClient GatewayClient { get; }
        public IEthernaUserIndexClient IndexClient { get; }
        public IEthernaUserSsoClient SsoClient { get; }
    }
}
