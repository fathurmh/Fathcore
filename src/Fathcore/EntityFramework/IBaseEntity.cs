using System;

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
        new TKey Id { get; }
    }

    /// <summary>
    /// Provides the interface for entity.
    /// </summary>
    public interface IBaseEntity
    {
        /// <summary>
        /// Gets the identifier in the <see cref="IBaseEntity"/>.
        /// </summary>
        object Id { get; }
    }
}
