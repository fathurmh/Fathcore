using System;
using System.ComponentModel.DataAnnotations;

namespace Fathcore.EntityFramework
{
    /// <summary>
    /// Provides the base class for entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <typeparam name="TKey">The type of identifier in the base class of entity.</typeparam>
    public abstract class BaseEntity<TEntity, TKey> : BaseEntity<TEntity>, IBaseEntity<TKey>, IBaseEntity
        where TEntity : BaseEntity<TEntity>, IBaseEntity<TKey>, IBaseEntity
        where TKey : IComparable, IComparable<TKey>, IConvertible, IEquatable<TKey>, IFormattable
    {
        protected BaseEntity() : base(typeof(TKey)) { }

        /// <summary>
        /// Gets or sets the identifier in the <see cref="BaseEntity{TEntity, TKey}"/>.
        /// </summary>
        [Key]
        public TKey Id { get; set; }
    }

    /// <summary>
    /// Provides the base class for entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    public abstract class BaseEntity<TEntity> : IBaseEntity
    {
        private readonly Type _keyType;

        /// <summary>
        /// Gets the type of entity key in the <see cref="IBaseEntity"/>.
        /// </summary>
        /// <returns>The type of entity key.</returns>
        public Type GetKeyType() => _keyType;

        /// <summary>
        /// Gets the type of entity in the <see cref="IBaseEntity"/>.
        /// </summary>
        /// <returns>The type of entity.</returns>
        public Type GetEntityType() => typeof(TEntity);

        protected BaseEntity(Type keyType)
        {
            _keyType = keyType;
        }
    }
}
