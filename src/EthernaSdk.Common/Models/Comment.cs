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
using System.Collections.Generic;

namespace Etherna.Sdk.Common.Models
{
    public class Comment
    {
        // Constructors.
        internal Comment(Comment2Dto comment)
        {
            Id = comment.Id;
            CreationDateTime = comment.CreationDateTime;
            IsEditable = comment.IsEditable;
            IsFrozen = comment.IsFrozen;
            OwnerAddress = comment.OwnerAddress;
            TextHistory = comment.TextHistory;
            VideoId = comment.VideoId;
        }
        
        // Properties.
        public string Id { get; }
        public DateTimeOffset CreationDateTime { get; }
        public bool IsEditable { get; }
        public bool IsFrozen { get; }
        public string OwnerAddress { get; }
        public IDictionary<string, string> TextHistory { get; }
        public string VideoId { get; }
    }
}