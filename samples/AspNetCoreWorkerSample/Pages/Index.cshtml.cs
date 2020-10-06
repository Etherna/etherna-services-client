using Etherna.CreditClient;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace AspNetCoreWorkerSample.Pages
{
    public class IndexModel : PageModel
    {
        // Fields.
        private readonly IConfiguration configuration;
        private readonly IServiceCreditClient creditClient;

        // Constructor.
        public IndexModel(
            IConfiguration configuration,
            IServiceCreditClient creditClient)
        {
            this.configuration = configuration;
            this.creditClient = creditClient;
        }

        // Properties.
        public double UserCredit { get; set; }

        // Methods.
        public async Task OnGetAsync()
        {
            UserCredit = await creditClient.ServiceInteract.BalanceGetAsync(configuration["SampleConfig:Address"]);
        }
    }
}
