﻿using System;
using System.Net.Http;

namespace Etherna.ServicesClient.Clients.Sso
{
    internal sealed class UserSsoClient : IUserSsoClient
    {
        // Fields.
        private readonly Uri baseUrl;
        private readonly HttpClient httpClient;

        // Constructor.
        public UserSsoClient(
            Uri baseUrl,
            HttpClient httpClient)
        {
            this.baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        // Properties.
        public IIdentityClient IdentityClient =>
            new IdentityClient(baseUrl.AbsoluteUri, httpClient);
    }
}
