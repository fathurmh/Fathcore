namespace Fathcore.EntityFramework.Builders
{
    /// <summary>
    /// Represents mapping configuration defaults.
    /// </summary>
    public static class MappingDefaults
    {
        /// <summary>
        /// The default value used for unique index prefix.
        /// <para>
        /// UIX
        /// </para>
        /// </summary>
        public static string UniqueIndexPrefix => "UIX";

        /// <summary>
        /// The default value used for index prefix.
        /// <para>
        /// IX
        /// </para>
        /// </summary>
        public static string IndexPrefix => "IX";

        /// <summary>
        /// The default value used for is not null filter.
        /// <para>
        /// [{0}] IS NOT NULL -> {0} : Column name.
        /// </para>
        /// </summary>
        public static string IsNotNullSuffix => "[{0}] IS NOT NULL";

        /// <summary>
        /// The default value used for is not null filter.
        /// <para>
        /// [{0}] IS NULL -> {0} : Column name.
        /// </para>
        /// </summary>
        public static string IsNullSuffix => "[{0}] IS NULL";

        /// <summary>
        /// Generate string for unique index.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="columnNames">The additional column name.</param>
        /// <returns>String formatted.</returns>
        public static string UniqueIndex(string tableName, string columnName, params string[] columnNames)
        {
            return columnNames.Length == 0
                ? string.Join("_", UniqueIndexPrefix, tableName, columnName)
                : string.Join("_", UniqueIndexPrefix, tableName, columnName, string.Join("_", columnNames));
        }

        /// <summary>
        /// Generate string for index.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="columnNames">The additional column name.</param>
        /// <returns>String formatted.</returns>
        public static string Index(string tableName, string columnName, params string[] columnNames)
        {
            return columnNames.Length == 0
                ? string.Join("_", IndexPrefix, tableName, columnName)
                : string.Join("_", IndexPrefix, tableName, columnName, string.Join("_", columnNames));
        }

        /// <summary>
        /// Generate string clause for not null column.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <returns>String formatted.</returns>
        public static string IsNotNull(string columnName)
        {
            return string.Format(IsNotNullSuffix, columnName);
        }

        /// <summary>
        /// Generate string clause for null column.
        /// </summary>
        /// <param name="columnName">The column name.</param>
        /// <returns>String formatted.</returns>
        public static string IsNull(string columnName)
        {
            return string.Format(IsNullSuffix, columnName);
        }
    }
}
