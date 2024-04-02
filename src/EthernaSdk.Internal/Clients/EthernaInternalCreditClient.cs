//   Copyright 2020-present Etherna SA
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Etherna.Sdk.Common.GenClients.Credit;
using Etherna.Sdk.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.Sdk.Internal.Clients
{
    public class EthernaInternalCreditClient : IEthernaInternalCreditClient
    {
        // Fields.
        private readonly ServiceInteractClient generatedClient;

        // Constructor.
        public EthernaInternalCreditClient(Uri baseUrl, HttpClient httpClient)
        {
            ArgumentNullException.ThrowIfNull(baseUrl, nameof(baseUrl));

            generatedClient = new ServiceInteractClient(baseUrl.ToString(), httpClient);
        }

        // Methods.
        public async Task<UserCredit> GetUserCreditAsync(
            string userAddress,
            CancellationToken cancellationToken = default) =>
            new(await generatedClient.CreditAsync(userAddress, cancellationToken).ConfigureAwait(false));

        public async Task<IEnumerable<UserOpLog>> GetUserOpLogsAsync(
            string userAddress,
            DateTimeOffset? fromDate = null,
            DateTimeOffset? toDate = null,
            CancellationToken cancellationToken = default) =>
            (await generatedClient.OplogsAsync(userAddress, fromDate, toDate, cancellationToken).ConfigureAwait(false)).Select(op => new UserOpLog(op));

        public Task UpdateUserBalanceAsync(
            string userAddress,
            double amount,
            string reason,
            bool? isApplied = null,
            CancellationToken cancellationToken = default) =>
            generatedClient.BalanceAsync(userAddress, amount, reason, isApplied, cancellationToken);
    }
}
