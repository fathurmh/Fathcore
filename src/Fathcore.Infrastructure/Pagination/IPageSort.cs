using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Fathcore.Infrastructure.Pagination
{
    /// <summary>
    /// Provides the interface for page sorting.
    /// </summary>
    /// <typeparam name="T">The type of entity data.</typeparam>
    public interface IPageSort<T>
    {
        /// <summary>
        /// Gets or sets the key selector contained in the <see cref="IPageSort{T}"/>.
        /// </summary>
        Expression<Func<T, dynamic>> KeySelector { get; }

        /// <summary>
        /// Gets or sets the sort direction contained in the <see cref="IPageSort{T}"/>.
        /// </summary>
        ListSortDirection SortDirection { get; }
    }
}
