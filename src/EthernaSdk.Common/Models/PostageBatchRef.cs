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

using Etherna.BeeNet.Models;

namespace Etherna.Sdk.Common.Models
{
    public class PostageBatchRef
    {
        // Constructors.
        internal PostageBatchRef(GenClients.Gateway.PostageBatchRefDto postageBatchRef)
        {
            BatchId = postageBatchRef.BatchId;
            Label = postageBatchRef.Label;
            OwnerNodeId = postageBatchRef.OwnerNodeId;
        }
        
        // Properties.
        public PostageBatchId BatchId { get; }
        public string? Label { get; }
        public string OwnerNodeId { get; }
    }
}