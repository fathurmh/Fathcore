namespace Fathcore.Infrastructure.Pagination
{
    /// <summary>
    /// Provides the interface for strongly typed paged data objects that can be accessed.
    /// </summary>
    public interface IPagedData
    {
        /// <summary>
        /// Gets the page index contained in the <see cref="IPagedData"/>.
        /// </summary>
        int PageIndex { get; }

        /// <summary>
        /// Gets the page size contained in the <see cref="IPagedData"/>.
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Gets the number of total elements contained in the <see cref="IPagedData"/>.
        /// </summary>
        int TotalCount { get; }

        /// <summary>
        /// Gets the number of total pages contained in the <see cref="IPagedData"/>.
        /// </summary>
        int TotalPages { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IPagedData"/> has previous page.
        /// </summary>
        bool HasPreviousPage { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="IPagedData"/> has next page.
        /// </summary>
        bool HasNextPage { get; }
    }
}
