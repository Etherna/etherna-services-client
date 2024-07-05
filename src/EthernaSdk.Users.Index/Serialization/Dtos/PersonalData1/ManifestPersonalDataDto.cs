// Copyright 2022-present Etherna SA
// This file is part of Etherna Video Importer.
// 
// Etherna Video Importer is free software: you can redistribute it and/or modify it under the terms of the
// GNU Affero General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Etherna Video Importer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Etherna Video Importer.
// If not, see <https://www.gnu.org/licenses/>.

using System.Diagnostics.CodeAnalysis;

namespace Etherna.Sdk.Users.Index.Serialization.Dtos.PersonalData1
{
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    internal sealed class ManifestPersonalDataDto
    {
        // Constructors.
        public ManifestPersonalDataDto(
            string clientName,
            string clientVersion,
            string sourceProviderName,
            string sourceVideoIdHash)
        {
            CliName = clientName;
            CliV = clientVersion;
            SrcName = sourceProviderName;
            SrcVIdHash = sourceVideoIdHash;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private ManifestPersonalDataDto() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        
        // Properties.
        public string CliName { get; private set; }
        public string CliV { get; private set; }
        public string SrcName { get; private set; }
        public string SrcVIdHash { get; private set; }
        public string V => "1";
    }
}
