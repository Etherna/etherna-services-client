using Etherna.Authentication.Native;
using Etherna.Sdk.Users;
using Etherna.Sdk.Users.Native;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace ConsoleUserClientSample
{
    class Program
    {
        // Consts.
        private const string HelpText = $"""
            Etherna Credit client service sample help:

            -c    Etherna Credit base url
            -l    SSO base url
            -i    SSO Etherna Credit client id
            -s    SSO Etherna Credit client secret (optional)
            -k    Api key (optional)
            
            -ss   Skip SSO Etherna Credit client secret
            -sk   Skip api key
            
            -h    Print help";
            """;

        static async Task Main(string[] args)
        {
            // Parse arguments.
            string? apiKey = null;
            string? creditBaseUrl = null;
            bool skipApiKey = false;
            bool skipSsoClientSecret = false;
            string? ssoBaseUrl = null;
            string? ssoClientId = null;
            string? ssoClientSecret = null;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-c": creditBaseUrl = args[++i]; break;
                    case "-l": ssoBaseUrl = args[++i]; break;
                    case "-i": ssoClientId = args[++i]; break;
                    case "-s": ssoClientSecret = args[++i]; break;
                    case "-k": apiKey = args[++i]; break;
                    case "-ss": skipSsoClientSecret = true; break;
                    case "-sk": skipApiKey = true; break;
                    case "-h": Console.Write(HelpText); return;
                    default: throw new ArgumentException(args[i] + " is not a valid argument");
                }
            }

            // Request Credit service info.
            Console.WriteLine("Etherna Credit base url:");
            if (creditBaseUrl is null) creditBaseUrl = Console.ReadLine();
            else Console.WriteLine(creditBaseUrl);

            Console.WriteLine();

            // Request SSO server info.
            Console.WriteLine("SSO base url:");
            if (ssoBaseUrl is null) ssoBaseUrl = Console.ReadLine();
            else Console.WriteLine(ssoBaseUrl);

            Console.WriteLine("SSO Etherna Credit client id:");
            if (ssoClientId is null) ssoClientId = Console.ReadLine();
            else Console.WriteLine(ssoClientId);

            if (!skipSsoClientSecret)
            {
                Console.WriteLine("SSO Etherna Credit client secret:");
                if (ssoClientSecret is null) ssoClientSecret = Console.ReadLine();
                else Console.WriteLine(ssoClientSecret);
            }

            if (!skipApiKey)
            {
                Console.WriteLine("Api key (optional):");
                if (apiKey is null) apiKey = Console.ReadLine();
                else Console.WriteLine(apiKey);
            }

            Console.WriteLine();
            
            // Register etherna credit service client.
            var services = new ServiceCollection();
            IEthernaUserClientsBuilder ethernaClientsBuilder;
            if (string.IsNullOrWhiteSpace(apiKey)) //"code" grant flow
            {
                ethernaClientsBuilder = services.AddEthernaUserClientsWithCodeAuth(
                    ssoBaseUrl!,
                    ssoClientId!,
                    ssoClientSecret,
                    3000,
                    new[] { "userApi.credit" });
            }
            else //"password" grant flow
            {
                ethernaClientsBuilder = services.AddEthernaUserClientsWithApiKeyAuth(
                    ssoBaseUrl!,
                    apiKey,
                    new[] { "userApi.credit" });
            }
            ethernaClientsBuilder.AddEthernaCreditClient(new Uri(creditBaseUrl!));
            
            // Get client.
            var serviceProvider = services.BuildServiceProvider();
            var client = serviceProvider.GetRequiredService<IEthernaUserCreditClient>();
            
            // Signin user.
            var ethernaSignInService = serviceProvider.GetRequiredService<IEthernaSignInService>();
            try
            {
                await ethernaSignInService.SignInAsync();
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"Error during authentication");
                Console.WriteLine(e.Message);
                throw;
            }
            catch (Win32Exception e)
            {
                Console.WriteLine($"Error opening browser on local system. Try to authenticate with API key.");
                Console.WriteLine(e.Message);
                throw;
            }

            // Consume service api.
            var credit = await client.UserClient.CreditAsync();
            Console.WriteLine($"Current user balance: {credit.Balance} xDai");
        }
    }
}