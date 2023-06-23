using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Etherna.ServicesClient.Users.Native
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
