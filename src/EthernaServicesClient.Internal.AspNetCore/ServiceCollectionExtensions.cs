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

using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Etherna.ServicesClient.Internal.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        // Methods.
        public static IEthernaInternalClientsBuilder AddEthernaInternalClients(
            this IServiceCollection services,
            Uri ssoBaseUrl,
            bool requireHttps = true)
        {
            if (ssoBaseUrl is null)
                throw new ArgumentNullException(nameof(ssoBaseUrl));

            // Register memory cache to keep tokens.
            services.AddDistributedMemoryCache();

            // Register client token management.
            var clientCredentialsTokenManagementBuilder = services.AddClientCredentialsTokenManagement();

            // Discover token endpoint.
            var discoverTokenEndpointTask = DiscoverTokenEndpointAsync(requireHttps, ssoBaseUrl);
            discoverTokenEndpointTask.Wait();
            var tokenEndpoint = discoverTokenEndpointTask.Result;

            return new EthernaInternalClientsBuilder(
                services,
                ssoBaseUrl,
                tokenEndpoint,
                clientCredentialsTokenManagementBuilder);
        }

        // Helpers.
        private static async Task<string> DiscoverTokenEndpointAsync(
            bool requireHttps,
            Uri ssoBaseUrl)
        {
            // Discover endpoints from metadata.
            using var httpClient = new HttpClient();
            using var request = new DiscoveryDocumentRequest
            {
                Address = ssoBaseUrl.AbsoluteUri,
                Policy = new DiscoveryPolicy { RequireHttps = requireHttps }
            };

            var discoveryDocResult = await httpClient.GetDiscoveryDocumentAsync(request).ConfigureAwait(false);

            if (discoveryDocResult.IsError)
                throw discoveryDocResult.Exception ?? new InvalidOperationException();

            return discoveryDocResult.TokenEndpoint!;
        }
    }
}
