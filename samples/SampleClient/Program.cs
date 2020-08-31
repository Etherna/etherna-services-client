using System;
using System.Threading.Tasks;

namespace Etherna.CreditClient.SampleClient
{
    class Program
    {
        static async Task Main()
        {
            // Request Credit service info.
            Console.WriteLine("Etherna Credit base url:");
            var baseUrl = Console.ReadLine();
            Console.WriteLine("User address:");
            var address = Console.ReadLine();
            Console.WriteLine();

            // Request SSO server info.
            Console.WriteLine("SSO base url:");
            var ssoBaseUrl = Console.ReadLine();
            Console.WriteLine("SSO Etherna Credit client id:");
            var ssoClientId = Console.ReadLine();
            Console.WriteLine("SSO Etherna Credit client secret:");
            var ssoClientSecret = Console.ReadLine();
            Console.WriteLine();

            // Create client.
            var client = new EthernaCreditClient(new Uri(baseUrl), new Uri(ssoBaseUrl), ssoClientId, ssoClientSecret);

            // Consume service api.
            var balance = await client.ServiceInteract.BalanceGetAsync(address);
            Console.WriteLine($"Current balance: ${balance}");
        }
    }
}
