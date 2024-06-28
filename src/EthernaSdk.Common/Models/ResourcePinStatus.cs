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

namespace Etherna.Sdk.Common.Models
{
    public class ResourcePinStatus
    {
        // Constructors.
        internal ResourcePinStatus(GenClients.Gateway.ResourcePinStatusDto pinStatus)
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