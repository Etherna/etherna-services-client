// Copyright 2020-present Etherna SA
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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