using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Etherna.CreditClient.SampleClient
{
    class Program
    {
        static async Task Main(string[] args)
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

            //*********
            // discover endpoints from metadata
            var httpClient = new HttpClient();
            var disco = await httpClient.GetDiscoveryDocumentAsync(ssoBaseUrl);
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }
            
            // request token
            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = ssoClientId,
                ClientSecret = ssoClientSecret,
                Scope = "ethernaCredit_serviceInteract_api"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);

            // call api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            //********

            // Create service client.
            ServiceInteractClient client = new ServiceInteractClient(baseUrl, apiClient);

            // Consume service api.
            var balance = await client.BalanceGetAsync(address);
            Console.WriteLine($"Current balance: ${balance}");
        }
    }
}
