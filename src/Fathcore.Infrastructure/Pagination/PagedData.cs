using System.Runtime.Serialization;

namespace Fathcore.Infrastructure.Pagination
{
    /// <summary>
    /// Provides the class for strongly typed paged data objects that can be accessed.
    /// </summary>
    [DataContract]
    public class PagedData : IPagedData
    {
        /// <summary>
        /// Gets the page index contained in the <see cref="PagedData"/>.
        /// </summary>
        [DataMember]
        public int PageIndex { get; }

        /// <summary>
        /// Gets the page size contained in the <see cref="PagedData"/>.
        /// </summary>
        [DataMember]
        public int PageSize { get; }

        /// <summary>
        /// Gets the number of total elements contained in the <see cref="PagedData"/>.
        /// </summary>
        [DataMember]
        public int TotalCount { get; }

        /// <summary>
        /// Gets the number of total pages contained in the <see cref="PagedData"/>.
        /// </summary>
        [DataMember]
        public int TotalPages { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="PagedData"/> has previous page.
        /// </summary>
        [DataMember]
        public bool HasPreviousPage { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="PagedData"/> has next page.
        /// </summary>
        [DataMember]
        public bool HasNextPage { get; }

        public PagedData(IPagedData pagedData)
        {
            PageIndex = pagedData.PageIndex;
            PageSize = pagedData.PageSize;
            TotalCount = pagedData.TotalCount;
            TotalPages = pagedData.TotalPages;
            HasPreviousPage = pagedData.HasPreviousPage;
            HasNextPage = pagedData.HasNextPage;
        }
    }
}
