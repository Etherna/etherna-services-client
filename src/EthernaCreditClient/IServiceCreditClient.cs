using System.Threading.Tasks;

namespace Etherna.CreditClient
{
    public interface IServiceCreditClient
    {
        string? BearerToken { get; }
        bool IsInitialized { get; }
        IServiceInteractClient ServiceInteract { get; }

        Task InitializeAsync();
    }
}