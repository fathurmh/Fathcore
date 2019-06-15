using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Fathcore.Infrastructure.Pagination
{
    /// <summary>
    /// Provides the class for pagination.
    /// </summary>
    /// <typeparam name="T">The type of entity data.</typeparam>
    public class PaginationData<T> : IPaginationData<T>
    {
        /// <summary>
        /// Gets or sets the page index contained in the <see cref="PaginationData{T}"/>.
        /// </summary>
        public int PageIndex { get; }

        /// <summary>
        /// Gets or sets the page size contained in the <see cref="PaginationData{T}"/>.
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Gets or sets the page sort elements contained in the <see cref="PaginationData{T}"/>.
        /// </summary>
        public IEnumerable<IPageSort<T>> PageSorts { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageSorts"></param>
        public PaginationData(int pageIndex = default, int pageSize = default, params IPageSort<T>[] pageSorts)
        {
            PageIndex = pageIndex <= 0 ? 0 : pageIndex;
            PageSize = pageSize <= 0 ? 10 : pageSize;
            PageSorts = pageSorts;
        }
    }

    /// <summary>
    /// Provides the class for page sorting.
    /// </summary>
    /// <typeparam name="T">The type of entity data.</typeparam>
    public class PageSort<T> : IPageSort<T>
    {
        /// <summary>
        /// Gets or sets the key selector contained in the <see cref="PageSort{T}"/>.
        /// </summary>
        public Expression<Func<T, dynamic>> KeySelector { get; }

        /// <summary>
        /// Gets or sets the sort direction contained in the <see cref="PageSort{T}"/>.
        /// </summary>
        public ListSortDirection SortDirection { get; }

        public PageSort(Expression<Func<T, dynamic>> keySelector, ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            KeySelector = keySelector;
            SortDirection = sortDirection;
        }
    }
}
