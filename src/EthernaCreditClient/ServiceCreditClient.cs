using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Etherna.CreditClient
{
    public class ServiceCreditClient : IServiceCreditClient
    {
        // Fields.
        private readonly Uri baseUrl;
        private readonly Func<HttpClient> createHttpClient;

        // Constructor.
        public ServiceCreditClient(
            Uri baseUrl,
            Func<HttpClient> createHttpClient)
        {
            this.baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            this.createHttpClient = createHttpClient;
        }

        // Properties.
        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> ClientDefaultRequestHeaders =>
            createHttpClient().DefaultRequestHeaders;
        public IServiceInteractClient ServiceInteract =>
            new ServiceInteractClient(baseUrl.AbsoluteUri, createHttpClient());
    }
}
