using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fathcore.Infrastructure.Pagination
{
    /// <summary>
    /// Represents a strongly typed paged list of objects that can be accessed by index. Provides methods to search, sort, and manipulate lists.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public class PagedList<T> : List<T>, IPagedList<T>, IList<T>, ICollection<T>, IEnumerable<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, IPagedList, IList, ICollection, IEnumerable
    {
        /// <summary>
        /// Gets the page index contained in the <see cref="PagedList{T}"/>.
        /// </summary>
        public int PageIndex { get; }

        /// <summary>
        /// Gets the page size contained in the <see cref="PagedList{T}"/>.
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Gets the number of total elements contained in the <see cref="PagedList{T}"/>.
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// Gets the number of total pages contained in the <see cref="PagedList{T}"/>.
        /// </summary>
        public int TotalPages { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="PagedList{T}"/> has previous page.
        /// </summary>
        public bool HasPreviousPage => PageIndex > 0;

        /// <summary>
        /// Gets a value indicating whether the <see cref="PagedList{T}"/> has next page.
        /// </summary>
        public bool HasNextPage => PageIndex + 1 < TotalPages;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class that contains elements copied from the specified <see cref="IQueryable{T}"/> collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="source">The collection whose elements are copied to the new list.</param>
        /// <param name="pageIndex">Sets the page index contained in the <see cref="PagedList{T}"/>.</param>
        /// <param name="pageSize">Sets the page size contained in the <see cref="PagedList{T}"/>.</param>
        public PagedList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
            TotalCount = source.Count();
            TotalPages = (int)Math.Ceiling((double)TotalCount / pageSize);

            AddRange(source.Skip(pageIndex * pageSize).Take(pageSize).ToList());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class that contains elements copied from the specified <see cref="IEnumerable{T}"/> collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="source">The collection whose elements are copied to the new list.</param>
        /// <param name="pageIndex">Sets the page index contained in the <see cref="PagedList{T}"/>.</param>
        /// <param name="pageSize">Sets the page size contained in the <see cref="PagedList{T}"/>.</param>
        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize)
            : this(source.AsQueryable(), pageIndex, pageSize) { }
    }
}
