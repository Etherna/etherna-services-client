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

using Etherna.Sdk.Common.GenClients.Index;
using System;

namespace Etherna.Sdk.Common.Models
{
    public class VideoValidationErrorDetail
    {
        public enum ErrorTypes
        {
            InvalidAspectRatio = 0,
            InvalidBatchId = 1,
            InvalidDescription = 2,
            InvalidPersonalData = 3,
            InvalidThumbnailSource = 4,
            InvalidTitle = 5,
            InvalidVideoSource = 6,
            JsonConvert = 7,
            MissingDescription = 8,
            MissingDuration = 9,
            MissingManifestCreationTime = 10,
            MissingOriginalQuality = 11,
            MissingTitle = 12,
            Unknown = 13
        }
        
        // Constructors.
        internal VideoValidationErrorDetail(ErrorDetailDto errorDetail)
        {
            ErrorMessage = errorDetail.ErrorMessage;
            ErrorType = Enum.Parse<ErrorTypes>(errorDetail.ErrorType.ToString());
        }
        
        // Properties.
        public string ErrorMessage { get; }
        public ErrorTypes ErrorType { get; }
    }
}