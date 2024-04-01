// Copyright 2020-present Etherna SA
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

using System.Collections.Generic;

namespace Etherna.Sdk.Common.DtoModels
{
    public class UserInfoDto
    {
        // Constructors.
        internal UserInfoDto(GenClients.Gateway.UserDto userInfo)
        {
            EtherAddress = userInfo.EtherAddress;
            EtherPreviousAddresses = userInfo.EtherPreviousAddresses;
            Username = userInfo.Username;
        }

        internal UserInfoDto(GenClients.Sso.UserDto userInfo)
        {
            EtherAddress = userInfo.EtherAddress;
            EtherPreviousAddresses = userInfo.EtherPreviousAddresses;
            Username = userInfo.Username;
        }

        // Properties.
        public string EtherAddress { get; }
        public IEnumerable<string> EtherPreviousAddresses { get; }
        public string? Username { get; }
    }
}