using System.Collections;
using System.Collections.Generic;

namespace Fathcore
{
    /// <summary>
    /// Represents a strongly typed paged list of objects that can be accessed by index. Provides methods to search, sort, and manipulate lists.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public interface IPagedList<T> : ICollection<T>, IEnumerable<T>, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IEnumerable, IList
    {
        /// <summary>
        /// Page index.
        /// </summary>
        int PageIndex { get; }

        /// <summary>
        /// Page size.
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Total count.
        /// </summary>
        int TotalCount { get; }

        /// <summary>
        /// Total pages.
        /// </summary>
        int TotalPages { get; }

        /// <summary>
        /// Has previous page.
        /// </summary>
        bool HasPreviousPage { get; }

        /// <summary>
        /// Has next page.
        /// </summary>
        bool HasNextPage { get; }
    }
}
