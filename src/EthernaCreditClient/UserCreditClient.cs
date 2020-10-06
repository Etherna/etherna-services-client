using System;
using System.Net.Http;

namespace Etherna.CreditClient
{
    public class UserCreditClient : IUserCreditClient
    {
        // Fields.
        private readonly Uri baseUrl;
        private readonly Func<HttpClient> createHttpClient;

        // Constructor.
        public UserCreditClient(
            Uri baseUrl,
            Func<HttpClient> createHttpClient)
        {
            this.baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            this.createHttpClient = createHttpClient;
        }

        // Properties.
        public IUserClient UserClient =>
            new UserClient(baseUrl.AbsoluteUri, createHttpClient());
    }
}
