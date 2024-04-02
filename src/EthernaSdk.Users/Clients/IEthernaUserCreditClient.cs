// Copyright 2020-present Etherna SA
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Etherna.Sdk.Common.GenClients.Credit;
using Etherna.Sdk.Common.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.Sdk.Users.Clients
{
    public interface IEthernaUserCreditClient
    {
        /// <summary>
        /// Get credit status for current user
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="EthernaCreditApiException">A server side error occurred.</exception>
        Task<UserCredit> GetUserCreditAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get transaction logs for current user
        /// </summary>
        /// <param name="page">Current page of results</param>
        /// <param name="take">Number of items to retrieve. Max 100</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Current page on list</returns>
        /// <exception cref="EthernaCreditApiException">A server side error occurred.</exception>
        Task<IEnumerable<UserOpLog>> GetUserOpLogsAsync(
            int? page = null,
            int? take = null,
            CancellationToken cancellationToken = default);
    }
}