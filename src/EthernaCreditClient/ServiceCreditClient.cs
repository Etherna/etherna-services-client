using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.CreditClient
{
    public class ServiceCreditClient : IServiceCreditClient, IDisposable
    {
        // Fields.
        private ServiceInteractClient _serviceInteract = default!;
        private readonly Uri baseUrl;
        private readonly ReaderWriterLockSlim initLock = new ReaderWriterLockSlim();
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
            initLock.Dispose();
        }

        // Properties.
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
            initLock.EnterWriteLock();

            if (IsInitialized)
                return;

            try
            {
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

                // Set api token.
                var apiClient = new HttpClient();
                apiClient.SetBearerToken(tokenResponse.AccessToken);

                // Create service clients.
                _serviceInteract = new ServiceInteractClient(baseUrl.AbsoluteUri, apiClient);

                // Set initialized.
                IsInitialized = true;
            }
            finally
            {
                initLock.ExitWriteLock();
            }
        }
    }
}
