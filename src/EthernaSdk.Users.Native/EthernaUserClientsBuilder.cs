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

using Etherna.BeeNet;
using Etherna.Sdk.Credit.Users.Clients;
using Etherna.Sdk.Gateway.Users.Clients;
using Etherna.Sdk.Index.Users.Clients;
using Etherna.Sdk.Sso.Users.Clients;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Etherna.Sdk.Users.Native
{
    internal sealed class EthernaUserClientsBuilder : IEthernaUserClientsBuilder
    {
        // Fields.
        private readonly string httpClientName;
        private readonly IServiceCollection services;
        private readonly Uri ssoBaseUrl;

        // Constructor.
        public EthernaUserClientsBuilder(
            IServiceCollection services,
            string httpClientName,
            Uri ssoBaseUrl)
        {
            this.httpClientName = httpClientName;
            this.services = services;
            this.ssoBaseUrl = ssoBaseUrl;
        }

        // Methods.
        public IEthernaUserClientsBuilder AddEthernaCreditClient(
            Uri creditServiceBaseUrl)
        {
            // Register client.
            services.AddSingleton<IEthernaUserCreditClient>(serviceProvider =>
            {
                var clientFactory = serviceProvider.GetService<IHttpClientFactory>()!;
                return new EthernaUserCreditClient(
                    creditServiceBaseUrl,
                    clientFactory.CreateClient(httpClientName));
            });

            return this;
        }

        public IEthernaUserClientsBuilder AddEthernaGatewayClient(
            Uri gatewayBaseUrl)
        {
            // Register client.
            services.AddSingleton<IEthernaUserGatewayClient>(serviceProvider =>
            {
                var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>()!;
                var httpClient = httpClientFactory.CreateClient(httpClientName);
                
                var beeClient = new BeeClient(
                    gatewayBaseUrl,
                    httpClient);
                
                return new EthernaUserGatewayClient(
                    gatewayBaseUrl,
                    beeClient,
                    httpClient);
            });

            return this;
        }

        public IEthernaUserClientsBuilder AddEthernaIndexClient(
            Uri indexBaseUrl)
        {
            // Register client.
            services.AddSingleton<IEthernaUserIndexClient>(serviceProvider =>
            {
                var clientFactory = serviceProvider.GetService<IHttpClientFactory>()!;
                return new EthernaUserIndexClient(
                    indexBaseUrl,
                    clientFactory.CreateClient(httpClientName));
            });

            return this;
        }

        public IEthernaUserClientsBuilder AddEthernaSsoClient()
        {
            // Register client.
            services.AddSingleton<IEthernaUserSsoClient>(serviceProvider =>
            {
                var clientFactory = serviceProvider.GetService<IHttpClientFactory>()!;
                return new EthernaUserSsoClient(
                    ssoBaseUrl,
                    clientFactory.CreateClient(httpClientName));
            });

            return this;
        }
    }
}
