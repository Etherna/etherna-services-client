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

using Etherna.BeeNet.Hasher;
using Etherna.Sdk.Users.Index.Serialization.Dtos.PersonalData1;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Etherna.Sdk.Users.Index.Models
{
    public class VideoManifestPersonalData
    {
        // Fields.
        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        // Constructor.
        public VideoManifestPersonalData(
            string clientName,
            string clientVersion,
            string sourceProviderName,
            string sourceVideoId)
        {
            var hashProvider = new HashProvider();
            
            ClientName = clientName;
            ClientVersion = clientVersion;
            SourceProviderName = sourceProviderName;
            SourceVideoIdHash = hashProvider.ComputeHash(sourceVideoId).ToHex();
        }

        // Properties.
        public string ClientName { get; }
        public string ClientVersion { get; }
        public string SourceProviderName { get; }
        public string SourceVideoIdHash { get; }
        
        // Methods.
        public string Serialize()
        {
            var dto = new ManifestPersonalDataDto(
                ClientName,
                ClientVersion,
                SourceProviderName,
                SourceVideoIdHash);
            return JsonSerializer.Serialize(dto, jsonSerializerOptions);
        }
        
        // Static methods.
        public static bool TryDeserialize(string rawPersonalData, out VideoManifestPersonalData personalData)
        {
            personalData = default!;
            
            try
            {
                var dto = JsonSerializer.Deserialize<ManifestPersonalDataDto>(rawPersonalData);
                if (dto is null)
                    return false;
                
                personalData = new VideoManifestPersonalData(
                    dto.CliName,
                    dto.CliV,
                    dto.SrcName,
                    dto.SrcVIdHash);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}