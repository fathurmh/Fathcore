using Fathcore.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fathcore.Data
{
    /// <summary>
    /// Represents base query type mapping configuration
    /// </summary>
    /// <typeparam name="TQuery">Query type type</typeparam>
    public class QueryTypeConfiguration<TQuery> : IMappingConfiguration, IQueryTypeConfiguration<TQuery> where TQuery : class
    {
        /// <summary>
        /// Developers can override this method in custom partial classes in order to add some custom configuration code
        /// </summary>
        /// <param name="builder">The builder to be used to configure the query</param>
        protected virtual void PostConfigure(QueryTypeBuilder<TQuery> builder)
        {
        }

        /// <summary>
        /// Configures the query type
        /// </summary>
        /// <param name="builder">The builder to be used to configure the query type</param>
        public virtual void Configure(QueryTypeBuilder<TQuery> builder)
        {
            //add custom configuration
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
