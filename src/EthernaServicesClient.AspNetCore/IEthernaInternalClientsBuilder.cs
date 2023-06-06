using System;

namespace Etherna.ServicesClient.AspNetCore
{
    public interface IEthernaInternalClientsBuilder
    {
        IEthernaInternalClientsBuilder AddEthernaCreditClient(
            Uri creditServiceBaseUrl,
            string clientId,
            string clientSecret);

        IEthernaInternalClientsBuilder AddEthernaSsoClient(
            string clientId,
            string clientSecret);
    }
}