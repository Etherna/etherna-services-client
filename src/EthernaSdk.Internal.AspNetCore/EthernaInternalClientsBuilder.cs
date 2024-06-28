// Copyright 2020-present Etherna SA
// This file is part of Etherna SDK .Net.
// 
// Etherna SDK .Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Etherna SDK .Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Etherna SDK .Net.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.Sdk.Internal.Clients;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Etherna.Sdk.Internal.AspNetCore
{
    internal sealed class EthernaInternalClientsBuilder : IEthernaInternalClientsBuilder
    {
        // Consts.
        private const string EthernaInternalCreditTokenClientName = "ethernaInternalCreditTokenClient";
        private const string EthernaInternalSsoTokenClientName = "ethernaInternalSsoTokenClient";

        // Fields.
        private readonly ClientCredentialsTokenManagementBuilder cctmBuilder;
        private readonly Action<HttpClient>? configureHttpClient;
        private readonly string httpClientName;
        private readonly IServiceCollection services;
        private readonly Uri ssoBaseUrl;
        private readonly string tokenEndpoint;

        // Constructor.
        public EthernaInternalClientsBuilder(
            IServiceCollection services,
            Uri ssoBaseUrl,
            string tokenEndpoint,
            ClientCredentialsTokenManagementBuilder clientCredentialsTokenManagementBuilder,
            string httpClientName,
            Action<HttpClient>? configureHttpClient)
        {
            cctmBuilder = clientCredentialsTokenManagementBuilder;
            this.configureHttpClient = configureHttpClient;
            this.httpClientName = httpClientName;
            this.services = services;
            this.ssoBaseUrl = ssoBaseUrl;
            this.tokenEndpoint = tokenEndpoint;
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
            services.AddClientCredentialsHttpClient(
                httpClientName,
                EthernaInternalCreditTokenClientName,
                configureHttpClient);

            // Register service.
            services.AddSingleton<IEthernaInternalCreditClient>(serviceProvider =>
            {
                var clientFactory = serviceProvider.GetService<IHttpClientFactory>()!;
                return new EthernaInternalCreditClient(
                    creditServiceBaseUrl,
                    clientFactory.CreateClient(httpClientName));
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
            services.AddClientCredentialsHttpClient(
                httpClientName,
                EthernaInternalSsoTokenClientName,
                configureHttpClient);

            // Register service.
            services.AddSingleton<IEthernaInternalSsoClient>(serviceProvider =>
            {
                var clientFactory = serviceProvider.GetService<IHttpClientFactory>()!;
                return new EthernaInternalSsoClient(
                    ssoBaseUrl,
                    clientFactory.CreateClient(httpClientName));
            });

            return this;
        }
    }
}
