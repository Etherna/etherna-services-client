// Copyright 2020-present Etherna SA
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Etherna.Sdk.Common.GenClients.Index;

namespace Etherna.Sdk.Common.Models
{
    public class IndexParameters
    {
        // Constructors.
        internal IndexParameters(SystemParametersDto parameters)
        {
            CommentMaxLength = parameters.CommentMaxLength;
            VideoDescriptionMaxLength = parameters.VideoDescriptionMaxLength;
            VideoPersonalDataMaxLength = parameters.VideoPersonalDataMaxLength;
            VideoTitleMaxLength = parameters.VideoTitleMaxLength;
        }
        
        // Properties.
        public int CommentMaxLength { get; }
        public int VideoDescriptionMaxLength { get; }
        public int VideoPersonalDataMaxLength { get; }
        public int VideoTitleMaxLength { get; }
    }
}