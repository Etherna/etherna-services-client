using System;
using System.Net.Http;

namespace Etherna.CreditClient
{
    public class UserCreditClient : IUserCreditClient
    {
        // Constructors.
        public UserCreditClient(Uri baseUrl)
        {
            if (baseUrl is null)
                throw new ArgumentNullException(nameof(baseUrl));

            var apiClient = new HttpClient();
            UserClient = new UserClient(baseUrl.AbsoluteUri, apiClient);
        }

        // Properties.
        public IUserClient UserClient { get; }
    }
}
