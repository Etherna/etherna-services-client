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

using Etherna.Authentication.Native;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Etherna.Sdk.Users.Native
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