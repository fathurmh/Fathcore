using System.Collections;
using System.Collections.Generic;

namespace Fathcore.Infrastructure.Pagination
{
    /// <summary>
    /// Provides the interface for strongly typed paged list of objects that can be accessed by index. Provides methods to search, sort, and manipulate lists.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public interface IPagedList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IPagedData, IList, ICollection, IEnumerable
    {
        /// <summary>
        /// Gets the number of elements contained in the <see cref="IPagedList{T}"/>.
        /// </summary>
        new int Count { get; }
    }
}
