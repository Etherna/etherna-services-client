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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Etherna.Sdk.Users.Index.Serialization.Dtos.Manifest1
{
    [SuppressMessage("Performance", "CA1822:Mark members as static")]
    [SuppressMessage("ReSharper", "ClassCannotBeInstantiated")]
    internal sealed class Manifest1Dto
    {
        // Consts.
        private const int PersonalDataMaxLength = 200;

        // Fields.
        private string? _personalData;

        // Constructors.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Manifest1Dto() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // Properties.
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string OriginalQuality { get; private set; }
        public string OwnerAddress { get; private set; }
        public long Duration { get; private set; }
        public Manifest1ThumbnailDto? Thumbnail { get; private set; }
        public IEnumerable<Manifest1VideoSourceDto> Sources { get; private set; }
        public long CreatedAt { get; private set; }
        public long? UpdatedAt { get; private set; }
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
        public string V => "1.2";
    }
}
