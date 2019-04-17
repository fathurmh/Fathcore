using System.Collections;
using System.Collections.Generic;

namespace Fathcore.Collections
{
    /// <summary>
    /// Provides the interface for strongly typed paged list of objects that can be accessed by index. Provides methods to search, sort, and manipulate lists.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public interface IPagedList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IPagedList, IList, ICollection, IEnumerable
    {
        /// <summary>
        /// Gets the number of elements contained in the <see cref="IPagedList{T}"/>.
        /// </summary>
        new int Count { get; }
    }

    /// <summary>
    /// Provides the interface for strongly typed paged list of objects that can be accessed by index. Provides methods to search, sort, and manipulate lists.
    /// </summary>
    public interface IPagedList : IList, ICollection, IEnumerable
    {
        /// <summary>
        /// Gets the page index contained in the <see cref="IPagedList"/>.
        /// </summary>
        int PageIndex { get; }

        /// <summary>
        /// Gets the page size contained in the <see cref="IPagedList"/>.
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Gets the number of total elements contained in the <see cref="IPagedList"/>.
        /// </summary>
        int TotalCount { get; }

        /// <summary>
        /// Gets the number of total pages contained in the <see cref="IPagedList"/>.
        /// </summary>
        int TotalPages { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IPagedList"/> has previous page.
        /// </summary>
        bool HasPreviousPage { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IPagedList"/> has next page.
        /// </summary>
        bool HasNextPage { get; }
    }
}
