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
    public class UserContactInfo
    {
        internal UserContactInfo(Etherna.Sdk.Common.GenClients.Sso.UserContactInfoDto contactInfo)
        {
            Email = contactInfo.Email;
            PhoneNumber = contactInfo.PhoneNumber;
        }
        
        public string? Email { get; }
        public string? PhoneNumber { get; }
    }
}