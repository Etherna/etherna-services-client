//   Copyright 2020-present Etherna SA
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

using Etherna.Sdk.Users.Clients;
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
                var clientFactory = serviceProvider.GetService<IHttpClientFactory>()!;
                return new EthernaUserGatewayClient(
                    gatewayBaseUrl,
                    clientFactory.CreateClient(httpClientName));
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
