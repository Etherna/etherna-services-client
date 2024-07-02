// Copyright 2020-present Etherna SA
// This file is part of Etherna SDK .Net.
// 
// Etherna SDK .Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Etherna SDK .Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Etherna SDK .Net.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.Sdk.Credit.GenClients;
using Etherna.Sdk.Credit.Models;
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
