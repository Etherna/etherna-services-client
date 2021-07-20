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

using Etherna.ServicesClient;
using Etherna.ServicesClient.Clients.Credit;
using Etherna.ServicesClient.Clients.Sso;
using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        // Consts.
        private const string CreditClientName = "ethernaCreditServiceClient";
        private const string SsoClientName = "ethernaSsoServiceClient";

        // Methods.
        public static void AddEthernaCreditClientForServices(
            this IServiceCollection services,
            Uri creditServiceBaseUrl,
            Uri ssoBaseUrl,
            string clientId,
            string clientSecret)
        {
            var task = RegisterTokenManagerAsync(
                services,
                ssoBaseUrl,
                CreditClientName,
                clientId,
                clientSecret,
                "ethernaCredit_serviceInteract_api");
            task.Wait();

            // Register http client. (don't remove it!, `new HttpClient()` doesn't work)
            services.AddClientAccessTokenClient(CreditClientName);

            // Register service.
            services.AddSingleton<IServiceCreditClient>(serviceProvider =>
            {
                var clientFactory = serviceProvider.GetService<IHttpClientFactory>()!;
                return new ServiceCreditClient(
                    creditServiceBaseUrl,
                    () => clientFactory.CreateClient(CreditClientName));
            });
        }

        public static void AddEthernaSsoClientForServices(
            this IServiceCollection services,
            Uri ssoBaseUrl,
            string clientId,
            string clientSecret)
        {
            var task = RegisterTokenManagerAsync(
                services,
                ssoBaseUrl,
                SsoClientName,
                clientId,
                clientSecret,
                "ethernaSso_userContactInfo_api");
            task.Wait();

            // Register http client. (don't remove it!, `new HttpClient()` doesn't work)
            services.AddClientAccessTokenClient(SsoClientName);

            // Register service.
            services.AddSingleton<IServiceSsoClient>(serviceProvider =>
            {
                var clientFactory = serviceProvider.GetService<IHttpClientFactory>()!;
                return new ServiceSsoClient(
                    ssoBaseUrl,
                    () => clientFactory.CreateClient(SsoClientName));
            });
        }

        public static void AddEthernaClientsForUsers(
            this IServiceCollection services,
            Uri creditServiceBaseUrl,
            Uri ssoServicebaseUrl)
        {
            if (creditServiceBaseUrl is null)
                throw new ArgumentNullException(nameof(creditServiceBaseUrl));

            services.AddSingleton<IEthernaUserClients>(new EthernaUserClients(
                creditServiceBaseUrl,
                ssoServicebaseUrl,
                () => new HttpClient()));
        }

        // Helpers.
        private static async Task RegisterTokenManagerAsync(
            IServiceCollection services,
            Uri ssoBaseUrl,
            string clientName,
            string clientId,
            string clientSecret,
            string clientScope)
        {
            if (ssoBaseUrl is null)
                throw new ArgumentNullException(nameof(ssoBaseUrl));

            // Discover endpoints from metadata.
            using var httpClient = new HttpClient();
            var discoveryDoc = await httpClient.GetDiscoveryDocumentAsync(ssoBaseUrl.AbsoluteUri).ConfigureAwait(false);
            if (discoveryDoc.IsError)
                throw discoveryDoc.Exception ?? new InvalidOperationException();

            // Register token manager.
            services.AddAccessTokenManagement(options =>
            {
                options.Client.Clients.Add(clientName, new ClientCredentialsTokenRequest
                {
                    Address = discoveryDoc.TokenEndpoint,
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    Scope = clientScope
                });
            });
        }
    }
}
