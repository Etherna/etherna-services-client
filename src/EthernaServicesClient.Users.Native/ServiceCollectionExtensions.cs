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

using Etherna.Authentication.Native;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Etherna.ServicesClient.Users.Native
{
    public static class ServiceCollectionExtensions
    {
        // Consts.
        public const string DefaultEthernaUserHttpClientName = "ethernaUserHttpClient";

        // Methods.
        public static IEthernaUserClientsBuilder AddEthernaUserClientsWithApiKeyAuth(
            this IServiceCollection services,
            string authority,
            string apiKey,
            IEnumerable<string> scopes,
            string httpClientName = DefaultEthernaUserHttpClientName,
            Action<HttpClient>? configureHttpClient = null)
        {
            // Register Etherna OpenId Connect client with "password" flow.
            services.AddEthernaApiKeyOidcClient(
                authority,
                apiKey,
                scopes,
                httpClientName,
                configureHttpClient);

            return new EthernaUserClientsBuilder(
                services,
                httpClientName,
                new Uri(authority));
        }

        public static IEthernaUserClientsBuilder AddEthernaUserClientsWithCodeAuth(
            this IServiceCollection services,
            string authority,
            string clientId,
            string? clientSecret,
            int returnUrlPort,
            IEnumerable<string> scopes,
            string httpClientName = DefaultEthernaUserHttpClientName,
            Action<HttpClient>? configureHttpClient = null)
        {
            // Register Etherna OpenId Connect client with "code" flow.
            services.AddEthernaCodeOidcClient(
                authority,
                clientId,
                clientSecret,
                returnUrlPort,
                scopes,
                httpClientName,
                configureHttpClient);

            return new EthernaUserClientsBuilder(
                services,
                httpClientName,
                new Uri(authority));
        }
    }
}