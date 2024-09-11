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

using Etherna.Sdk.Tools.Video.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Etherna.Sdk.Tools.Video.Exceptions
{
    public class VideoManifestValidationException : Exception
    {
        // Constructors.
        public VideoManifestValidationException() : this(Array.Empty<ValidationError>())
        { }

        public VideoManifestValidationException(string message) : base(message)
        {
            ValidationErrors = [];
        }

        public VideoManifestValidationException(string message, Exception innerException) : base(message, innerException)
        {
            ValidationErrors = [];
        }
        
        public VideoManifestValidationException(ValidationError[] validationErrors)
            : base(ValidationErrosToString(validationErrors))
        {
            ValidationErrors = validationErrors;
        }
        
        public VideoManifestValidationException(ValidationError[] validationErrors, Exception innerException)
            : base(ValidationErrosToString(validationErrors), innerException)
        {
            ValidationErrors = validationErrors;
        }
        
        // Properties.
        public IEnumerable<ValidationError> ValidationErrors { get; }
        
        // Helpers.
        private static string ValidationErrosToString(ValidationError[] validationErrors) =>
            validationErrors.Aggregate(
                "",
                (a, e) =>
                {
                    if (!string.IsNullOrEmpty(a))
                        a += "\n";
                    a += $"{e.ErrorType}: {e.ErrorMessage}";
                    return a;
                });
    }
}