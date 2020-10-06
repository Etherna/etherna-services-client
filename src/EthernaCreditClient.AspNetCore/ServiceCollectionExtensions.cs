﻿using Etherna.CreditClient;
using IdentityModel.Client;
using System;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        // Consts.
        private const string ClientName = "ethernaCreditServiceClient";

        // Methods.
        public static void AddEthernaCreditClientForServices(
            this IServiceCollection services,
            Uri serviceBaseUrl,
            Uri ssoBaseUrl,
            string clientId,
            string clientSecret)
        {
            if (ssoBaseUrl is null)
                throw new ArgumentNullException(nameof(ssoBaseUrl));

            // Discover endpoints from metadata.
            using var httpClient = new HttpClient();
            var discoverTask = httpClient.GetDiscoveryDocumentAsync(ssoBaseUrl.AbsoluteUri);
            discoverTask.Wait();
            var discoveryDoc = discoverTask.Result;
            if (discoveryDoc.IsError)
                throw discoveryDoc.Exception ?? new InvalidOperationException();

            // Register token manager.
            services.AddAccessTokenManagement(options =>
            {
                options.Client.Clients.Add("identityserver", new ClientCredentialsTokenRequest
                {
                    Address = discoveryDoc.TokenEndpoint,
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    Scope = "ethernaCredit_serviceInteract_api"
                });
            });

            // Register http client.
            services.AddClientAccessTokenClient(ClientName);

            // Register service.
            services.AddSingleton<IServiceCreditClient>(serviceProvider =>
            {
                var clientFactory = serviceProvider.GetService<IHttpClientFactory>();
                return new ServiceCreditClient(
                    serviceBaseUrl,
                    () => clientFactory.CreateClient(ClientName));
            });
        }

        public static void AddEthernaCreditClientForUsers(this IServiceCollection services, Uri serviceBaseUrl)
        {
            if (serviceBaseUrl is null)
                throw new ArgumentNullException(nameof(serviceBaseUrl));

            services.AddSingleton<IUserCreditClient>(new UserCreditClient(serviceBaseUrl, () => new HttpClient()));
        }
    }
}
