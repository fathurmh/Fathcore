using System;
using System.ComponentModel.DataAnnotations;

namespace Fathcore.EntityFramework
{
    /// <summary>
    /// Provides the interface for entity.
    /// </summary>
    /// <typeparam name="TKey">The type of identifier in the base class of entity.</typeparam>
    public interface IBaseEntity<TKey> : IBaseEntity
        where TKey : IComparable, IComparable<TKey>, IConvertible, IEquatable<TKey>, IFormattable
    {
        /// <summary>
        /// Gets the identifier in the <see cref="IBaseEntity{TKey}"/>.
        /// </summary>
        [Key]
        TKey Id { get; }
    }

    /// <summary>
    /// Provides the interface for entity.
    /// </summary>
    public interface IBaseEntity
    {
        /// <summary>
        /// Gets the type of entity key in the <see cref="IBaseEntity"/>.
        /// </summary>
        /// <returns>The type of entity key.</returns>
        Type GetKeyType();

        /// <summary>
        /// Gets the type of entity in the <see cref="IBaseEntity"/>.
        /// </summary>
        /// <returns>The type of entity.</returns>
        Type GetEntityType();
    }
}
