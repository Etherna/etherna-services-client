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
using System.Collections.Generic;

namespace Etherna.Sdk.Users.Index.Serialization.Dtos.Manifest2
{
    public class Manifest2DetailDto
    {
        // Consts.
        private const int PersonalDataMaxLength = 200;

        // Fields.
        private string? _personalData;

        // Constructors.
        public Manifest2DetailDto(
            string description,
            float aspectRatio,
            string batchId,
            string? personalData,
            IEnumerable<Manifest2VideoSourceDto> sources)
        {
            Description = description;
            AspectRatio = aspectRatio;
            BatchId = batchId;
            PersonalData = personalData;
            Sources = sources;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Manifest2DetailDto() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Properties.
        public string Description { get; private set; }
        public float AspectRatio { get; private set; }
        public string BatchId { get; private set; }
        public string? PersonalData
        {
            get => _personalData;
            private set
            {
                if (value is not null && value.Length > PersonalDataMaxLength)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _personalData = value;
            }
        }
        public IEnumerable<Manifest2VideoSourceDto> Sources { get; private set; }
    }
}