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

namespace Etherna.Sdk.Tools.Video.Models
{
    public class ValidationError(
        ValidationErrorType errorType,
        string? errorMessage = null)
    {
        // Properties.
        public string ErrorMessage { get; } = errorMessage ?? errorType.ToString();
        public ValidationErrorType ErrorType { get; } = errorType;

        // Methods.
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is null) return false;
            return GetType() == obj.GetType() &&
                   EqualityComparer<string>.Default.Equals(ErrorMessage, (obj as ValidationError)!.ErrorMessage) &&
                   ErrorType.Equals((obj as ValidationError)?.ErrorType);
        }

        public override int GetHashCode() =>
            (ErrorMessage ?? "").GetHashCode(StringComparison.OrdinalIgnoreCase) ^ ErrorType.GetHashCode();
    }
}