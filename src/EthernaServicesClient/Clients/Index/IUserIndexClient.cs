namespace Etherna.ServicesClient.Clients.Index
{
    public interface IUserIndexClient
    {
        ICommentsClient CommentsClient { get; }
        IModerationClient ModerationClient { get; }
        ISearchClient SearchClient { get; }
        ISystemClient SystemClient { get; }
        IUsersClient UsersClient { get; }
        IVideosClient VideosClient { get; }
    }
}