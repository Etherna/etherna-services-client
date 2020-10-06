using System.Collections.Generic;

namespace Etherna.CreditClient
{
    public interface IServiceCreditClient
    {
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> ClientDefaultRequestHeaders { get; }
        IServiceInteractClient ServiceInteract { get; }
    }
}