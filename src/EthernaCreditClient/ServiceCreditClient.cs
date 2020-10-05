using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.CreditClient
{
    public class ServiceCreditClient : IServiceCreditClient, IDisposable
    {
        // Fields.
        private ServiceInteractClient _serviceInteract = default!;
        private readonly HttpClient apiClient = new HttpClient();
        private readonly Uri baseUrl;
        private readonly SemaphoreSlim initLock = new SemaphoreSlim(1);
        private readonly Uri ssoBaseUrl;
        private readonly string ssoClientId;
        private readonly string ssoClientSecret;

        // Constructors and Dispose.
        public ServiceCreditClient(
            Uri baseUrl,
            Uri ssoBaseUrl,
            string ssoClientId,
            string ssoClientSecret)
        {
            if (string.IsNullOrEmpty(ssoClientId))
                throw new ArgumentException($"'{nameof(ssoClientId)}' cannot be null or empty", nameof(ssoClientId));
            if (string.IsNullOrEmpty(ssoClientSecret))
                throw new ArgumentException($"'{nameof(ssoClientSecret)}' cannot be null or empty", nameof(ssoClientSecret));

            this.baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            this.ssoBaseUrl = ssoBaseUrl ?? throw new ArgumentNullException(nameof(ssoBaseUrl));
            this.ssoClientId = ssoClientId;
            this.ssoClientSecret = ssoClientSecret;
        }

        public void Dispose()
        {
            apiClient.Dispose();
            initLock.Dispose();
        }

        // Properties.
        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> DefaultRequestHeaders
        {
            get
            {
                if (!IsInitialized)
                    InitializeAsync().Wait();
                return apiClient.DefaultRequestHeaders;
            }
        }
        public bool IsInitialized { get; private set; }
        public IServiceInteractClient ServiceInteract
        {
            get
            {
                if (!IsInitialized)
                    InitializeAsync().Wait();
                return _serviceInteract;
            }
        }

        // Methods.
        public async Task InitializeAsync()
        {
            await initLock.WaitAsync().ConfigureAwait(false);

            try
            {
                if (IsInitialized)
                    return;

                // Discover endpoints from metadata.
                using var httpClient = new HttpClient();
                var discoveryDoc = await httpClient.GetDiscoveryDocumentAsync(ssoBaseUrl.AbsoluteUri).ConfigureAwait(false);
                if (discoveryDoc.IsError)
                    throw discoveryDoc.Exception ?? new InvalidOperationException();

                // Request token.
                using var tokenRequest = new ClientCredentialsTokenRequest
                {
                    Address = discoveryDoc.TokenEndpoint,

                    ClientId = ssoClientId,
                    ClientSecret = ssoClientSecret,
                    Scope = "ethernaCredit_serviceInteract_api"
                };
                var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(tokenRequest).ConfigureAwait(false);

                if (tokenResponse.IsError)
                    throw tokenResponse.Exception ?? new InvalidOperationException();

                // Set api bearer token.
                apiClient.SetBearerToken(tokenResponse.AccessToken);

                // Create service clients.
                _serviceInteract = new ServiceInteractClient(baseUrl.AbsoluteUri, apiClient);

                // Set initialized.
                IsInitialized = true;
            }
            finally
            {
                initLock.Release();
            }
        }
    }
}
