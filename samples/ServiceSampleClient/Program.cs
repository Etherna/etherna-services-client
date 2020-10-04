using System;
using System.Threading.Tasks;

namespace Etherna.CreditClient.ServiceSampleClient
{
    class Program
    {
        // Consts.
        const string HelpText =
            "Etherna Credit client service sample help:\n\n" +
            "-c\tEtherna Credit base url\n" +
            "-a\tUser address\n" +
            "-l\tSSO base url\n" +
            "-i\tSSO Etherna Credit client id\n" +
            "-s\tSSO Etherna Credit client secret\n" +
            "\n" +
            "-h\tPrint help\n";

        static async Task Main(string[] args)
        {
            // Parse arguments.
            string? address = null;
            string? creditBaseUrl = null;
            string? ssoBaseUrl = null;
            string? ssoClientId = null;
            string? ssoClientSecret = null;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-c": creditBaseUrl = args[++i]; break;
                    case "-a": address = args[++i]; break;
                    case "-l": ssoBaseUrl = args[++i]; break;
                    case "-i": ssoClientId = args[++i]; break;
                    case "-s": ssoClientSecret = args[++i]; break;
                    case "-h": Console.Write(HelpText); return;
                    default: throw new ArgumentException(args[i] + " is not a valid argument");
                }
            }

            // Request Credit service info.
            Console.WriteLine("Etherna Credit base url:");
            if (creditBaseUrl is null) creditBaseUrl = Console.ReadLine();
            else Console.WriteLine(creditBaseUrl);

            Console.WriteLine("User address:");
            if (address is null) address = Console.ReadLine();
            else Console.WriteLine(address);

            Console.WriteLine();

            // Request SSO server info.
            Console.WriteLine("SSO base url:");
            if (ssoBaseUrl is null) ssoBaseUrl = Console.ReadLine();
            else Console.WriteLine(ssoBaseUrl);

            Console.WriteLine("SSO Etherna Credit client id:");
            if (ssoClientId is null) ssoClientId = Console.ReadLine();
            else Console.WriteLine(ssoClientId);

            Console.WriteLine("SSO Etherna Credit client secret:");
            if (ssoClientSecret is null) ssoClientSecret = Console.ReadLine();
            else Console.WriteLine(ssoClientSecret);

            Console.WriteLine();

            // Create client.
            var client = new ServiceCreditClient(new Uri(creditBaseUrl), new Uri(ssoBaseUrl), ssoClientId, ssoClientSecret);

            // Initialize and print initialization.
            await client.InitializeAsync();
            Console.WriteLine("Bearer token:");
            Console.WriteLine(client.BearerToken);
            Console.WriteLine();

            // Consume service api.
            var balance = await client.ServiceInteract.BalanceGetAsync(address);
            Console.WriteLine($"Current balance: ${balance}");
        }
    }
}
