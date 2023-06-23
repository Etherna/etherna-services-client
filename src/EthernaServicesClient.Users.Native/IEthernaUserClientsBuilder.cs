using System;

namespace Etherna.ServicesClient.Users.Native
{
    public interface IEthernaUserClientsBuilder
    {
        IEthernaUserClientsBuilder AddEthernaCreditClient(
            Uri creditServiceBaseUrl);

        IEthernaUserClientsBuilder AddEthernaGatewayClient(
            Uri gatewayBaseUrl);

        IEthernaUserClientsBuilder AddEthernaIndexClient(
            Uri indexBaseUrl);

        IEthernaUserClientsBuilder AddEthernaSsoClient();
    }
}