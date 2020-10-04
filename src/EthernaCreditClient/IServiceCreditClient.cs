using System.Threading.Tasks;

namespace Etherna.CreditClient
{
    public interface IServiceCreditClient
    {
        string? BearerToken { get; }
        IServiceInteractClient ServiceInteract { get; }

        Task InitializeAsync();
    }
}