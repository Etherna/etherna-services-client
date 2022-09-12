namespace Etherna.ServicesClient.Clients.Sso
{
    public interface IUserSsoClient
    {
        IIdentityClient IdentityClient { get; }
    }
}