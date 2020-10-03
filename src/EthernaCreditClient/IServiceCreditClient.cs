using System.Threading.Tasks;

namespace Etherna.CreditClient
{
    public interface IServiceCreditClient
    {
        IServiceInteractClient ServiceInteract { get; }

        Task InitializeAsync();
    }
}