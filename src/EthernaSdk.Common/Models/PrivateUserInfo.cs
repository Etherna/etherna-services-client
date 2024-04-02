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

using Etherna.Sdk.Common.GenClients.Sso;
using System.Collections.Generic;

namespace Etherna.Sdk.Common.Models
{
    public class PrivateUserInfo
    {
        internal PrivateUserInfo(PrivateUserDto privateInfo)
        {
            AccountType = privateInfo.AccountType;
            Email = privateInfo.Email;
            EtherAddress = privateInfo.EtherAddress;
            EtherManagedPrivateKey = privateInfo.EtherManagedPrivateKey;
            EtherPreviousAddresses = privateInfo.EtherPreviousAddresses;
            EtherLoginAddress = privateInfo.EtherLoginAddress;
            PhoneNumber = privateInfo.PhoneNumber;
            Username = privateInfo.Username;
        }
        
        public string AccountType { get; }
        public string? Email { get; }
        public string EtherAddress { get; }
        public string? EtherManagedPrivateKey { get; }
        public IEnumerable<string> EtherPreviousAddresses { get; }
        public string? EtherLoginAddress { get; }
        public string? PhoneNumber { get; }
        public string? Username { get; }
    }
}