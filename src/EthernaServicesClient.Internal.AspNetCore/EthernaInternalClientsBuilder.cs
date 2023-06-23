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

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Etherna.ServicesClient.Internal.AspNetCore
{
    internal sealed class EthernaInternalClientsBuilder : IEthernaInternalClientsBuilder
    {
        // Consts.
        private const string EthernaInternalHttpClientName = "ethernaInternalHttpClient";
        private const string EthernaInternalCreditTokenClientName = "ethernaInternalCreditTokenClient";
        private const string EthernaInternalSsoTokenClientName = "ethernaInternalSsoTokenClient";

        // Fields.
        private readonly ClientCredentialsTokenManagementBuilder cctmBuilder;
        private readonly IServiceCollection services;
        private readonly Uri ssoBaseUrl;
        private readonly string tokenEndpoint;

        // Constructor.
        public EthernaInternalClientsBuilder(
            IServiceCollection services,
            Uri ssoBaseUrl,
            string tokenEndpoint,
            ClientCredentialsTokenManagementBuilder clientCredentialsTokenManagementBuilder)
        {
            this.services = services;
            this.ssoBaseUrl = ssoBaseUrl;
            this.tokenEndpoint = tokenEndpoint;
            cctmBuilder = clientCredentialsTokenManagementBuilder;
        }

        // Methods.
        public IEthernaInternalClientsBuilder AddEthernaCreditClient(
            Uri creditServiceBaseUrl,
            string clientId,
            string clientSecret)
        {
            // Register client to token management.
            cctmBuilder.AddClient(EthernaInternalCreditTokenClientName, options =>
            {
                options.TokenEndpoint = tokenEndpoint;

                options.ClientId = clientId;
                options.ClientSecret = clientSecret;

                options.Scope = "ethernaCredit_serviceInteract_api";
            });

            // Register http client.
            services.AddClientCredentialsHttpClient(EthernaInternalHttpClientName, EthernaInternalCreditTokenClientName);

            // Register service.
            services.AddSingleton<IEthernaInternalCreditClient>(serviceProvider =>
            {
                var clientFactory = serviceProvider.GetService<IHttpClientFactory>()!;
                return new EthernaInternalCreditClient(
                    creditServiceBaseUrl,
                    clientFactory.CreateClient(EthernaInternalHttpClientName));
            });

            return this;
        }

        public IEthernaInternalClientsBuilder AddEthernaSsoClient(
            string clientId,
            string clientSecret)
        {
            // Register client to token management.
            cctmBuilder.AddClient(EthernaInternalSsoTokenClientName, options =>
            {
                options.TokenEndpoint = tokenEndpoint;

                options.ClientId = clientId;
                options.ClientSecret = clientSecret;

                options.Scope = "ethernaSso_userContactInfo_api";
            });

            // Register http client.
            services.AddClientCredentialsHttpClient(EthernaInternalHttpClientName, EthernaInternalSsoTokenClientName);

            // Register service.
            services.AddSingleton<IEthernaInternalSsoClient>(serviceProvider =>
            {
                var clientFactory = serviceProvider.GetService<IHttpClientFactory>()!;
                return new EthernaInternalSsoClient(
                    ssoBaseUrl,
                    clientFactory.CreateClient(EthernaInternalHttpClientName));
            });

            return this;
        }
    }
}
