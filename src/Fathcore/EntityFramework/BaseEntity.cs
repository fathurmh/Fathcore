using System;

namespace Fathcore.EntityFramework
{
    /// <summary>
    /// Provides the base class for entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <typeparam name="TKey">The type of identifier in the base class of entity.</typeparam>
    public abstract class BaseEntity<TEntity, TKey> : BaseEntity<TEntity>, IBaseEntity<TKey>, IBaseEntity
        where TEntity : IBaseEntity<TKey>, IBaseEntity
        where TKey : IComparable, IComparable<TKey>, IConvertible, IEquatable<TKey>, IFormattable
    {
        /// <summary>
        /// Gets or sets the identifier in the <see cref="BaseEntity{TEntity, TKey}"/>.
        /// </summary>
        public new TKey Id { get; set; }
    }

    /// <summary>
    /// Provides the base class for entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    public abstract class BaseEntity<TEntity> : IBaseEntity
    {
        /// <summary>
        /// Gets or sets the identifier in the <see cref="BaseEntity{TEntity}"/>.
        /// </summary>
        public object Id { get; set; }
    }
}
