using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fathcore.EntityFramework.Builders
{
    /// <summary>
    /// Provides the base class for entity mapping configuration.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity being configured.</typeparam>
    /// <typeparam name="TKey">The type of identifier in the base class of entity.</typeparam>
    public abstract class EntityTypeConfiguration<TEntity, TKey> : IMappingConfiguration, IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity<TEntity, TKey>, IBaseEntity<TKey>, IBaseEntity
        where TKey : IComparable, IComparable<TKey>, IConvertible, IEquatable<TKey>, IFormattable
    {
        /// <summary>
        /// Post configures the entity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity.</param>
        protected virtual void PostConfigure(EntityTypeBuilder<TEntity> builder)
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(TEntity)))
            {
                builder.HasQueryFilter(entity => !((ISoftDeletable)entity).IsDeleted);
            }
        }

        /// <summary>
        /// Configures the entity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity.</param>
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(entity => entity.Id);

            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(TEntity)))
            {
                builder.Property(nameof(ISoftDeletable.IsDeleted))
                    .IsRequired();

                builder.Property(nameof(ISoftDeletable.DeletedTime))
                    .HasColumnType("datetime");
            }

            if (typeof(IAuditable).IsAssignableFrom(typeof(TEntity)))
            {
                builder.Property(nameof(IAuditable.CreatedBy))
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasDefaultValue("Anonymous");

                builder.Property(nameof(IAuditable.CreatedTime))
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValue(DateTime.Now);

                builder.Property(nameof(IAuditable.ModifiedBy))
                    .HasMaxLength(100);

                builder.Property(nameof(IAuditable.ModifiedTime))
                    .HasColumnType("datetime");
            }

            if (typeof(IConcurrentable).IsAssignableFrom(typeof(TEntity)))
            {
                builder.Property(nameof(IConcurrentable.RowVersion))
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .ValueGeneratedOnAddOrUpdate();
            }

            PostConfigure(builder);
        }

        /// <summary>
        /// Apply this mapping configuration.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for the database context.</param>
        public virtual void ApplyConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(this);
        }
    }
}
