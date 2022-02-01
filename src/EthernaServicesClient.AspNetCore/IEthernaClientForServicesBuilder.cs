using IdentityModel.Client;
using System;
using System.Threading.Tasks;

namespace Etherna.ServicesClient.AspNetCore
{
    public interface IEthernaClientForServicesBuilder
    {
        // Properties.
        string ClientId { get; }
        string ClientName { get; }
        string ClientScope { get; }
        string ClientSecret { get; }
        Uri SsoBaseUrl { get; }

        // Methods.
        Task<ClientCredentialsTokenRequest> GetClientCredentialsTokenRequestAsync();
    }
}