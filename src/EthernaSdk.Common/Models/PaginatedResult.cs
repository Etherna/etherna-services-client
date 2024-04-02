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

using System.Collections.Generic;

namespace Etherna.Sdk.Common.Models
{
    public class PaginatedResult<TModel>
    {
        // Constructors.
        internal PaginatedResult(
            IEnumerable<TModel> elements,
            long totalElements,
            int pageSize,
            int currentPage,
            long maxPage)
        {
            Elements = elements;
            TotalElements = totalElements;
            PageSize = pageSize;
            CurrentPage = currentPage;
            MaxPage = maxPage;
        }
        
        // Properties.
        public int CurrentPage { get; }
        public IEnumerable<TModel> Elements { get; }
        public long MaxPage { get; }
        public int PageSize { get; }
        public long TotalElements { get; }
    }
}