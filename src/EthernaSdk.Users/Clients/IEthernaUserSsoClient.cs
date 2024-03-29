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

using Etherna.Sdk.Common.DtoModels;
using Etherna.Sdk.Common.GenClients.Sso;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.Sdk.Users.Clients
{
    public interface IEthernaUserSsoClient
    {
        /// <summary>
        /// Get current user private information.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Current user private information</returns>
        /// <exception cref="EthernaSsoApiException">A server side error occurred.</exception>
        Task<PrivateUserInfoDto> GetPrivateUserInfoAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get information about user by its ethereum address.
        /// </summary>
        /// <param name="userAddress">User's ethereum address</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>User information</returns>
        /// <exception cref="EthernaSsoApiException">A server side error occurred.</exception>
        Task<UserInfoDto> GetUserInfoByAddressAsync(string userAddress, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get information about user by its username.
        /// </summary>
        /// <param name="username">User's username</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>User information</returns>
        /// <exception cref="EthernaSsoApiException">A server side error occurred.</exception>
        Task<UserInfoDto> GetUserInfoByUsernameAsync(string username, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verify if an email is registered.
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>True if email is registered, false otherwise</returns>
        /// <exception cref="EthernaSsoApiException">A server side error occurred.</exception>
        Task<bool> IsEmailRegisteredAsync(string email, CancellationToken cancellationToken = default);
    }
}