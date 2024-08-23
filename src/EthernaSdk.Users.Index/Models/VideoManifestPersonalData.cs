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

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Etherna.Sdk.Users.Index.Models
{
    public class VideoManifestPersonalData(
        string clientName,
        string clientVersion,
        string sourceProviderName,
        string sourceVideoId)
    {
        // Fields.
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Properties.
        public string ClientName { get; } = clientName;
        public string ClientVersion { get; } = clientVersion;
        public string SourceProviderName { get; } = sourceProviderName;
        public string SourceVideoId { get; } = sourceVideoId;

        // Methods.
        public string Serialize()
        {
            var dto = new Serialization.Dtos.PersonalData1.ManifestPersonalDataDto(
                ClientName,
                ClientVersion,
                SourceProviderName,
                SourceVideoId);
            return JsonSerializer.Serialize(dto, JsonSerializerOptions);
        }
        
        // Static methods.
        public static bool TryDeserialize(string rawPersonalData, out VideoManifestPersonalData personalData)
        {
            personalData = default!;
            
            try
            {
                var dto = JsonSerializer.Deserialize<Serialization.Dtos.PersonalData1.ManifestPersonalDataDto>(rawPersonalData, JsonSerializerOptions);
                if (dto is null)
                    return false;
                
                personalData = new VideoManifestPersonalData(
                    dto.CliName,
                    dto.CliV,
                    dto.SrcName,
                    dto.SrcVId);
                return true;
            }
            catch (Exception e) when (e is InvalidOperationException
                                        or JsonException)
            {
                return false;
            }
        }
    }
}