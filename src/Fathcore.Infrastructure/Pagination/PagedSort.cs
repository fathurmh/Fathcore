using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Fathcore.Infrastructure.Pagination
{
    /// <summary>
    /// Provides the class for page sorting.
    /// </summary>
    /// <typeparam name="T">The type of entity data.</typeparam>
    public class PagedSort<T> : IPagedSort<T>
    {
        /// <summary>
        /// Gets or sets the key selector contained in the <see cref="IPagedSort{T}"/>.
        /// </summary>
        public Expression<Func<T, dynamic>> KeySelector { get; }

        /// <summary>
        /// Gets or sets the sort direction contained in the <see cref="IPagedSort{T}"/>.
        /// </summary>
        public ListSortDirection SortDirection { get; }

        public PagedSort(Expression<Func<T, dynamic>> keySelector, ListSortDirection sortDirection)
        {
            KeySelector = keySelector;
            SortDirection = sortDirection;
        }
    }
}
