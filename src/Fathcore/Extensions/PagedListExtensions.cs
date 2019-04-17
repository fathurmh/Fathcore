using System.Collections.Generic;
using Fathcore.Collections;

namespace Fathcore
{
    /// <summary>
    /// Extension methods for Collection.
    /// </summary>
    public static class PagedListExtensions
    {
        /// <summary>
        /// Creates a <see cref="IPagedList{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to create a <see cref="IPagedList{T}"/>.</param>
        /// <param name="pageIndex">Sets the page index.</param>
        /// <param name="pageSize">Sets the page size.</param>
        /// <returns>A <see cref="IPagedList{T}"/> that contains elements from the input sequence.</returns>
        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            return new PagedList<T>(source, pageIndex, pageSize);
        }
    }
}
