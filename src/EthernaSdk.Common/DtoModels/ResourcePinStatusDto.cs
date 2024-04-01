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

namespace Etherna.Sdk.Common.DtoModels
{
    public class ResourcePinStatusDto
    {
        // Constructors.
        internal ResourcePinStatusDto(GenClients.Gateway.ResourcePinStatusDto pinStatus)
        {
            FreePinningEndOfLife = pinStatus.FreePinningEndOfLife;
            IsPinned = pinStatus.IsPinned;
            IsPinningInProgress = pinStatus.IsPinningInProgress;
            IsPinningRequired = pinStatus.IsPinningRequired;
        }

        // Properties.
        public System.DateTimeOffset? FreePinningEndOfLife { get; }
        public bool IsPinned { get; }
        public bool IsPinningInProgress { get; }
        public bool IsPinningRequired { get; }
    }
}