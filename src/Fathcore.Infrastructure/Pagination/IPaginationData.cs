using System.Collections.Generic;

namespace Fathcore.Infrastructure.Pagination
{
    /// <summary>
    /// Provides the interface for pagination.
    /// </summary>
    /// <typeparam name="T">The type of entity data.</typeparam>
    public interface IPaginationData<T>
    {
        /// <summary>
        /// Gets or sets the page index contained in the <see cref="IPaginationData{T}"/>.
        /// </summary>
        int PageIndex { get; }

        /// <summary>
        /// Gets or sets the page size contained in the <see cref="IPaginationData{T}"/>.
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Gets or sets the page sorting contained in the <see cref="IPaginationData{T}"/>.
        /// </summary>
        IEnumerable<IPageSort<T>> PageSorts { get; }
    }
}
