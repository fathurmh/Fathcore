using Microsoft.EntityFrameworkCore;

namespace Fathcore.Data.Abstractions
{
    /// <summary>
    /// Represents database context model mapping configuration
    /// </summary>
    public interface IMappingConfiguration
    {
        /// <summary>
        /// Apply this mapping configuration
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for the database context</param>
        void ApplyConfiguration(ModelBuilder modelBuilder);

        /// <summary>
        /// Constraint name formatting
        /// </summary>
        /// <param name="prefix">Constraint prefix</param>
        /// <param name="nameOfTable">Name of table</param>
        /// <param name="nameOfColumns">Name of columns</param>
        /// <returns>Returns formatted constraint name</returns>
        string FormatName(string prefix, string nameOfTable, params string[] nameOfColumns);
    }
}
