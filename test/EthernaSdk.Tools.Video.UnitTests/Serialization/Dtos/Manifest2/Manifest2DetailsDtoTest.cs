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
using Xunit;

namespace Etherna.Sdk.Tools.Video.Serialization.Dtos.Manifest2
{
    public class Manifest2DetailsDtoTest
    {
        // Tests.
        [Fact]
        public void VerifyNotInvalidAspectRatio()
        {
            // Setup.
            var manifestDetails = new Manifest2DetailsDto
            {
                Description = "Description",
                Sources =
                [
                    new Manifest2VideoSourceDto
                    {
                        Path = "myPath",
                        Quality = "720",
                        Size = 42,
                        Type = "mp4"
                    }
                ]
            };
            
            // Action.
            var errors = manifestDetails.GetValidationErrors();

            // Assert.
            Assert.Equal([new ValidationError(ValidationErrorType.InvalidAspectRatio)], errors);
        }
    }
}