using Etherna.ServicesClient.Clients.Credit;
using Etherna.ServicesClient.Clients.Sso;

namespace Etherna.ServicesClient
{
    public interface IEthernaUserClients
    {
        public IUserCreditClient CreditClient { get; }
        public IUserSsoClient SsoClient { get; }
    }
}