using Etherna.ServicesClient.Clients.Credit;
using Etherna.ServicesClient.Clients.Index;
using Etherna.ServicesClient.Clients.Sso;

namespace Etherna.ServicesClient
{
    public interface IEthernaUserClients
    {
        public IUserCreditClient CreditClient { get; }
        public IUserIndexClient IndexClient { get; }
        public IUserSsoClient SsoClient { get; }
    }
}