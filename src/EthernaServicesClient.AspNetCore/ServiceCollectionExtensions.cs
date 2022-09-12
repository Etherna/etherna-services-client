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

using Etherna.ServicesClient.Clients.Credit;
using Etherna.ServicesClient.Clients.Sso;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Etherna.ServicesClient.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        // Consts.
        private const string CreditClientName = "ethernaCreditServiceClient";
        private const string SsoClientName = "ethernaSsoServiceClient";

        // Methods.
        public static IEthernaClientForServicesBuilder AddEthernaCreditClientForServices(
            this IServiceCollection services,
            Uri creditServiceBaseUrl,
            Uri ssoBaseUrl,
            string clientId,
            string clientSecret)
        {
            // Register http client. (don't remove it!, `new HttpClient()` doesn't work)
            services.AddClientAccessTokenHttpClient(CreditClientName, configureClient: default(Action<HttpClient>));

            // Register service.
            services.AddSingleton<IServiceCreditClient>(serviceProvider =>
            {
                var clientFactory = serviceProvider.GetService<IHttpClientFactory>()!;
                return new ServiceCreditClient(
                    creditServiceBaseUrl,
                    () => clientFactory.CreateClient(CreditClientName));
            });

            // Return builder.
            return new EthernaClientForServicesBuilder(
                clientId,
                CreditClientName,
                "ethernaCredit_serviceInteract_api",
                clientSecret,
                ssoBaseUrl);
        }

        public static IEthernaClientForServicesBuilder AddEthernaSsoClientForServices(
            this IServiceCollection services,
            Uri ssoBaseUrl,
            string clientId,
            string clientSecret)
        {
            // Register http client. (don't remove it!, `new HttpClient()` doesn't work)
            services.AddClientAccessTokenHttpClient(SsoClientName, configureClient: default(Action<HttpClient>));

            // Register service.
            services.AddSingleton<IServiceSsoClient>(serviceProvider =>
            {
                var clientFactory = serviceProvider.GetService<IHttpClientFactory>()!;
                return new ServiceSsoClient(
                    ssoBaseUrl,
                    () => clientFactory.CreateClient(SsoClientName));
            });

            // Return builder.
            return new EthernaClientForServicesBuilder(
                clientId,
                SsoClientName,
                "ethernaSso_userContactInfo_api",
                clientSecret,
                ssoBaseUrl);
        }

        public static void AddEthernaClientsForUsers(
            this IServiceCollection services,
            Uri creditServiceBaseUrl,
            Uri gatewayServiceBaseUrl,
            Uri indexServiceBaseUrl,
            Uri ssoServicebaseUrl)
        {
            if (creditServiceBaseUrl is null)
                throw new ArgumentNullException(nameof(creditServiceBaseUrl));

            services.AddSingleton<IEthernaUserClients>(new EthernaUserClients(
                creditServiceBaseUrl,
                gatewayServiceBaseUrl,
                indexServiceBaseUrl,
                ssoServicebaseUrl,
                () => new HttpClient()));
        }
    }
}
