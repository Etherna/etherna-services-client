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

using Etherna.Sdk.Common.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.Sdk.Internal.Clients
{
    public interface IEthernaInternalSsoClient
    {
        /// <summary>
        /// Get contact information about an user.
        /// </summary>
        /// <param name="userAddress">User's ethereum address</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>User contact information</returns>
        /// <exception cref="Common.Sso.EthernaSsoApiException">A server side error occurred.</exception>
        Task<UserContactInfo> ContactsAsync(string userAddress, CancellationToken cancellationToken = default);
    }
}