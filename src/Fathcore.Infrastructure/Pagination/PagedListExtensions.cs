using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fathcore.Infrastructure.Pagination;

namespace Fathcore.Extensions
{
    /// <summary>
    /// Extension methods for Collection.
    /// </summary>
    public static class PagedListExtensions
    {
        /// <summary>
        /// Creates a <see cref="IPagedList{T}"/> from an <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">The <see cref="IQueryable{T}"/> to create a <see cref="IPagedList{T}"/>.</param>
        /// <param name="pageIndex">Sets the page index.</param>
        /// <param name="pageSize">Sets the page size.</param>
        /// <returns>A <see cref="IPagedList{T}"/> that contains elements from the input sequence.</returns>
        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            return new PagedList<T>(source, pageIndex, pageSize);
        }

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
            return source.AsQueryable().ToPagedList(pageIndex, pageSize);
        }

        /// <summary>
        /// Asynchronously creates a <see cref="IPagedList{T}"/> from an <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">The <see cref="IQueryable{T}"/> to create a <see cref="IPagedList{T}"/>.</param>
        /// <param name="pageIndex">Sets the page index.</param>
        /// <param name="pageSize">Sets the page size.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IPagedList{T}"/> that contains elements from the input sequence.</returns>
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => new PagedList<T>(source, pageIndex, pageSize), cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates a <see cref="IPagedList{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to create a <see cref="IPagedList{T}"/>.</param>
        /// <param name="pageIndex">Sets the page index.</param>
        /// <param name="pageSize">Sets the page size.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IPagedList{T}"/> that contains elements from the input sequence.</returns>
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IEnumerable<T> source, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            return await source.AsQueryable().ToPagedListAsync(pageIndex, pageSize, cancellationToken);
        }

        /// <summary>
        /// Creates a <see cref="IPagedList{T}"/> from an <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">The <see cref="IQueryable{T}"/> to create a <see cref="IPagedList{T}"/>.</param>
        /// <param name="paginationData">Sets the data for pagination.</param>
        /// <returns>A <see cref="IPagedList{T}"/> that contains elements from the input sequence.</returns>
        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, IPaginationData<T> paginationData)
        {
            var query = source;

            if (paginationData.PageSorts != null)
                foreach (var pageSort in paginationData.PageSorts)
                    query = query.OrderBy(pageSort.KeySelector, pageSort.SortDirection);

            return new PagedList<T>(query, paginationData.PageIndex, paginationData.PageSize);
        }

        /// <summary>
        /// Creates a <see cref="IPagedList{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to create a <see cref="IPagedList{T}"/>.</param>
        /// <param name="paginationData">Sets the data for pagination.</param>
        /// <returns>A <see cref="IPagedList{T}"/> that contains elements from the input sequence.</returns>
        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, IPaginationData<T> paginationData)
        {
            return source.AsQueryable().ToPagedList(paginationData);
        }

        /// <summary>
        /// Asynchronously creates a <see cref="IPagedList{T}"/> from an <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">The <see cref="IQueryable{T}"/> to create a <see cref="IPagedList{T}"/>.</param>
        /// <param name="paginationData">Sets the data for pagination.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IPagedList{T}"/> that contains elements from the input sequence.</returns>
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, IPaginationData<T> paginationData, CancellationToken cancellationToken = default)
        {
            var query = source;

            if (paginationData.PageSorts != null)
                foreach (var pageSort in paginationData.PageSorts)
                    query = query.OrderBy(pageSort.KeySelector, pageSort.SortDirection);

            return await Task.Run(() => new PagedList<T>(query, paginationData.PageIndex, paginationData.PageSize), cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates a <see cref="IPagedList{T}"/> from an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to create a <see cref="IPagedList{T}"/>.</param>
        /// <param name="paginationData">Sets the data for pagination.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IPagedList{T}"/> that contains elements from the input sequence.</returns>
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IEnumerable<T> source, IPaginationData<T> paginationData, CancellationToken cancellationToken = default)
        {
            return await source.AsQueryable().ToPagedListAsync(paginationData, cancellationToken);
        }
    }
}
