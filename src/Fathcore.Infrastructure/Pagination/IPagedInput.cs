using System.Collections.Generic;

namespace Fathcore.Infrastructure.Pagination
{
    /// <summary>
    /// Provides the interface for pagination.
    /// </summary>
    /// <typeparam name="T">The type of entity data.</typeparam>
    public interface IPagedInput<T>
    {
        /// <summary>
        /// Gets or sets the page index contained in the <see cref="IPagedInput{T}"/>.
        /// </summary>
        int PageIndex { get; }

        /// <summary>
        /// Gets or sets the page size contained in the <see cref="IPagedInput{T}"/>.
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Gets or sets the page sorting contained in the <see cref="IPagedInput{T}"/>.
        /// </summary>
        IEnumerable<IPagedSort<T>> PageSorts { get; }
    }
}
