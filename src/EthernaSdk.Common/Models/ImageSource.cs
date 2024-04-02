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
    public class ImageSource
    {
        // Constructors.
        internal ImageSource(ImageSourceDto imageSource)
        {
            Type = imageSource.Type;
            Path = imageSource.Path;
            Width = imageSource.Width;
        }
        
        // Properties.
        public string? Type { get; }
        public string Path { get; }
        public int Width { get; }
    }
}