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

using System.Collections.Generic;

namespace Etherna.Sdk.Users.Index.Models
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