namespace Etherna.ServicesClient.Clients.Gateway
{
    public interface IUserGatewayClient
    {
        IPostageClient PostageClient { get; }
        IResourcesClient ResourcesClient { get; }
        ISystemClient SystemClient { get; }
        IUsersClient UsersClient { get; }
    }
}