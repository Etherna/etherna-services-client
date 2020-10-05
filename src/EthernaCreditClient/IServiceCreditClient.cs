using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etherna.CreditClient
{
    public interface IServiceCreditClient
    {
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> DefaultRequestHeaders { get; }
        bool IsInitialized { get; }
        IServiceInteractClient ServiceInteract { get; }

        Task InitializeAsync();
    }
}