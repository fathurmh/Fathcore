using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fathcore
{
    /// <summary>
    /// Represents a strongly typed paged list of objects that can be accessed by index. Provides methods to search, sort, and manipulate lists.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public class PagedList<T> : List<T>, ICollection<T>, IEnumerable<T>, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IEnumerable, IList, IPagedList<T>
    {
        /// <summary>
        /// Page index.
        /// </summary>
        public int PageIndex { get; }

        /// <summary>
        /// Page size.
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Total count.
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// Total pages.
        /// </summary>
        public int TotalPages { get; }

        /// <summary>
        /// Has previous page.
        /// </summary>
        public bool HasPreviousPage => PageIndex > 0;

        /// <summary>
        /// Has next page.
        /// </summary>
        public bool HasNextPage => PageIndex + 1 < TotalPages;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class that contains elements copied from the specified <see cref="IQueryable{T}"/> collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="source">The collection whose elements are copied to the new list.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        public PagedList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var total = source.Count();
            TotalCount = total;
            TotalPages = total / pageSize;

            if (total % pageSize > 0)
                TotalPages++;

            PageSize = pageSize;
            PageIndex = pageIndex;

            AddRange(source.Skip(pageIndex * pageSize).Take(pageSize).ToList());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class that contains elements copied from the specified <see cref="IList{T}"/> collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="source">The collection whose elements are copied to the new list.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        public PagedList(IList<T> source, int pageIndex, int pageSize)
            : this(source.AsQueryable(), pageIndex, pageSize) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class that contains elements copied from the specified <see cref="IEnumerable{T}"/> collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="source">The collection whose elements are copied to the new list.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        public PagedList(IEnumerable<T> source, int pageIndex, int pageSize)
            : this(source.AsQueryable(), pageIndex, pageSize) { }
    }
}
