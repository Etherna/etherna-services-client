using Etherna.ServicesClient.Clients.Credit;
using Etherna.ServicesClient.Clients.Sso;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Etherna.ServicesClient.AspNetCore
{
    internal sealed class EthernaInternalClientsBuilder : IEthernaInternalClientsBuilder
    {
        // Consts.
        private const string CreditClientName = "ethernaCreditServiceClient";
        private const string SsoClientName = "ethernaSsoServiceClient";

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
            cctmBuilder.AddClient(CreditClientName, options =>
                {
                    options.TokenEndpoint = tokenEndpoint;

                    options.ClientId = clientId;
                    options.ClientSecret = clientSecret;

                    options.Scope = "ethernaCredit_serviceInteract_api";
                });

            // Register http client.
            services.AddClientCredentialsHttpClient(CreditClientName, CreditClientName);

            // Register service.
            services.AddSingleton<IServiceCreditClient>(serviceProvider =>
            {
                var clientFactory = serviceProvider.GetService<IHttpClientFactory>()!;
                return new ServiceCreditClient(
                    creditServiceBaseUrl,
                    () => clientFactory.CreateClient(CreditClientName));
            });

            return this;
        }

        public IEthernaInternalClientsBuilder AddEthernaSsoClient(
            string clientId,
            string clientSecret)
        {
            // Register client to token management.
            cctmBuilder.AddClient(SsoClientName, options =>
                {
                    options.TokenEndpoint = tokenEndpoint;

                    options.ClientId = clientId;
                    options.ClientSecret = clientSecret;

                    options.Scope = "ethernaSso_userContactInfo_api";
                });

            // Register http client.
            services.AddClientCredentialsHttpClient(SsoClientName, SsoClientName);

            // Register service.
            services.AddSingleton<IServiceSsoClient>(serviceProvider =>
            {
                var clientFactory = serviceProvider.GetService<IHttpClientFactory>()!;
                return new ServiceSsoClient(
                    ssoBaseUrl,
                    () => clientFactory.CreateClient(SsoClientName));
            });

            return this;
        }
    }
}
