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

using Etherna.Sdk.Common.GenClients.Credit;
using System;

namespace Etherna.Sdk.Common.DtoModels
{
    public class UserOpLogDto
    {
        internal UserOpLogDto(OperationLogDto opLog)
        {
            Amount = opLog.Amount;
            Author = opLog.Author;
            CreationDateTime = opLog.CreationDateTime;
            IsApplied = opLog.IsApplied;
            OperationName = opLog.OperationName;
            Reason = opLog.Reason;
            UserAddress = opLog.UserAddress;
        }
        
        public double Amount { get; }
        public string Author { get; }
        public DateTimeOffset CreationDateTime { get; }
        public bool? IsApplied { get; }
        public string OperationName { get; }
        public string? Reason { get; }
        public string UserAddress { get; }
    }
}