using Fathcore.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fathcore.Data
{
    /// <summary>
    /// Represents base entity mapping configuration
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public class EntityTypeConfiguration<TEntity> : IMappingConfiguration, IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        /// Gets a formatted name of the unique index column
        /// </summary>
        protected const string UniqueIndexPrefix = "UIX";

        /// <summary>
        /// Gets a formatted name of the index column
        /// </summary>
        protected const string IndexPrefix = "IX";

        /// <summary>
        /// Developers can override this method in custom partial classes in order to add some custom configuration code
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        protected virtual void PostConfigure(EntityTypeBuilder<TEntity> builder)
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(TEntity)))
            {
                builder.HasQueryFilter(entity => !((ISoftDeletable)entity).IsDeleted);
            }
        }

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(entity => entity.Id);

            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(TEntity)))
            {
                builder.Property(nameof(ISoftDeletable.IsDeleted))
                    .IsRequired()
                    .HasDefaultValue(false)
                    .HasDefaultValueSql("(0)");
                builder.Property(nameof(ISoftDeletable.DeletedTime))
                    .HasColumnType("datetime");
            }

            if (typeof(IAuditable).IsAssignableFrom(typeof(TEntity)))
            {                
                builder.Property(nameof(IAuditable.CreatedBy))
                    .IsRequired()
                    .HasMaxLength(400)
                    .HasDefaultValueSql("('')");

                builder.Property(nameof(IAuditable.CreatedTime))
                    .IsRequired()
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                builder.Property(nameof(IAuditable.ModifiedBy))
                    .HasMaxLength(400);

                builder.Property(nameof(IAuditable.ModifiedTime))
                    .HasColumnType("datetime");
            }

            if (typeof(IConcurrentable).IsAssignableFrom(typeof(TEntity)))
            {
                builder.Property(nameof(IConcurrentable.RowVersion))
                    .IsRequired()
                    .IsRowVersion();
            }

            PostConfigure(builder);
        }

        /// <summary>
        /// Apply this mapping configuration
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for the database context</param>
        public virtual void ApplyConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(this);
        }

        /// <summary>
        /// Constraint name formatting
        /// </summary>
        /// <param name="prefix">Constraint prefix</param>
        /// <param name="nameOfTable">Name of table</param>
        /// <param name="nameOfColumns">Name of columns</param>
        /// <returns>Formatted constraint name</returns>
        public string FormatName(string prefix, string nameOfTable, params string[] nameOfColumns)
        {
            return string.Format("{0}_{1}_{2}", prefix, nameOfTable, string.Join("_", nameOfColumns));
        }
    }
}
