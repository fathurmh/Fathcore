using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fathcore.EntityFramework.Configuration
{
    /// <summary>
    /// Represents base query type mapping configuration.
    /// </summary>
    /// <typeparam name="TQuery">The query type being configured.</typeparam>
    public abstract class QueryTypeConfiguration<TQuery> : IMappingConfiguration, IQueryTypeConfiguration<TQuery> where TQuery : class
    {
        /// <summary>
        /// Post configures the query type.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the query type.</param>
        protected virtual void PostConfigure(QueryTypeBuilder<TQuery> builder)
        {
        }

        /// <summary>
        /// Configures the query type.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the query type.</param>
        public virtual void Configure(QueryTypeBuilder<TQuery> builder)
        {
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
