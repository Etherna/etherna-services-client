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

using Etherna.BeeNet.Models;
using Etherna.Sdk.Users.Index.Serialization.Dtos.Manifest2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Etherna.Sdk.Users.Index.Models
{
    /// <summary>
    /// Video Manifest utility class, able to serialize/deserialize to/from json
    /// </summary>
    public class VideoManifest(
        float aspectRatio,
        PostageBatchId? batchId,
        DateTimeOffset createdAt,
        string description,
        TimeSpan duration,
        string title,
        string ownerAddress,
        string? personalData,
        IEnumerable<VideoManifestVideoSource> sources,
        VideoManifestImage thumbnail,
        DateTimeOffset? updatedAt)
    {
        // Fields.
        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        // Methods.
        public string SerializeDetailsManifest()
        {
            var manifestDetails = new Manifest2DetailDto(
                description: Description,
                aspectRatio: AspectRatio,
                batchId: BatchId,
                personalData: PersonalDataRaw,
                sources: Sources.Select(s => new Manifest2VideoSourceDto(
                    type: s.Type,
                    quality: s.Quality,
                    path: s.Uri,
                    size: s.Size)));
            return JsonSerializer.Serialize(manifestDetails, jsonSerializerOptions);
        }

        public string SerializePreviewManifest()
        {
            var manifestPreview = new Manifest2PreviewDto(
                title: Title,
                createdAt: CreatedAt.ToUnixTimeSeconds(),
                updatedAt: UpdatedAt?.ToUnixTimeSeconds(),
                ownerAddress: OwnerAddress,
                duration: (long)Duration.TotalSeconds,
                thumbnail: new Manifest2ThumbnailDto(
                    aspectRatio: Thumbnail.AspectRatio,
                    blurhash: Thumbnail.Blurhash,
                    sources: Thumbnail.Sources.Select(s => new Manifest2ThumbnailSourceDto(
                        width: s.Width,
                        type: Enum.Parse<Manifest2ThumbnailSourceType>(s.Type),
                        path: s.Uri))));
            return JsonSerializer.Serialize(manifestPreview, jsonSerializerOptions);
        }
        
        // Static methods.
        public VideoManifest DeserializeManifest(string jsonManifestPreview, string jsonManifestDetail)
        {
            throw new NotImplementedException();
        }

        // Properties.
        public float AspectRatio { get; } = aspectRatio;
        public PostageBatchId BatchId { get; set; } = batchId ?? PostageBatchId.Zero; //can be updated later
        public DateTimeOffset CreatedAt { get; } = createdAt;
        public string Description { get; } = description;
        public TimeSpan Duration { get; } = duration;
        public string Title { get; } = title;
        public string OwnerAddress { get; } = ownerAddress;
        public VideoManifestPersonalData? PersonalData { get; } = TryParsePersonalData(personalData);
        public string? PersonalDataRaw { get; } = personalData;
        public IEnumerable<VideoManifestVideoSource> Sources { get; } = sources;
        public VideoManifestImage Thumbnail { get; } = thumbnail;
        public DateTimeOffset? UpdatedAt { get; } = updatedAt;
        
        // Methods.
        private static VideoManifestPersonalData? TryParsePersonalData(string? personalDataRaw)
        {
            if (personalDataRaw is null) return null;
            return VideoManifestPersonalData.TryDeserialize(personalDataRaw, out var personalData)
                ? personalData : null;
        }
    }
}