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

using Etherna.BeeNet.Hasher;
using Nethereum.Hex.HexConvertors.Extensions;
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
            string sourceName,
            string sourceVideoId)
        {
            var hashProvider = new HashProvider();

            CliName = clientName;
            CliV = clientVersion;
            SrcName = sourceName;
            SrcVIdHash = hashProvider.ComputeHash(sourceVideoId).ToHex();
        }
        private ManifestPersonalDataDto() { }
        
        // Properties.
        public string? CliName { get; set; }
        public string? CliV { get; set; }
        public string? SrcName { get; set; }
        public string? SrcVIdHash { get; set; }
        public string V => "1";
    }
}
