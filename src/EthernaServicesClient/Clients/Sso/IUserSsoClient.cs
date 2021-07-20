namespace Etherna.ServicesClient.Clients.Sso
{
    public interface IUserSsoClient
    {
        public IIdentityClient IdentityClient { get; }
    }
}