using Fathcore.EntityFramework.Builders;
using Xunit;

namespace Fathcore.EntityFramework.Tests.Builders
{
    public class MappingDefaultsTest
    {
        [Theory]
        [InlineData("UIX_User_Username", "User", "Username")]
        [InlineData("UIX_User_Username_Password", "User", "Username", "Password")]
        [InlineData("UIX_User_Username_Email_Password", "User", "Username", "Email", "Password")]
        public void Generate_UniqueIndex(string expected, string tableName, string columnName, params string[] columnNames)
        {
            var result = MappingDefaults.UniqueIndex(tableName, columnName, columnNames);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("IX_User_Username", "User", "Username")]
        [InlineData("IX_User_Username_Password", "User", "Username", "Password")]
        [InlineData("IX_User_Username_Email_Password", "User", "Username", "Email", "Password")]
        public void Generate_Index(string expected, string tableName, string columnName, params string[] columnNames)
        {
            var result = MappingDefaults.Index(tableName, columnName, columnNames);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("[Username] IS NOT NULL", "Username")]
        public void Generate_IsNotNull(string expected, string columnName)
        {
            var result = MappingDefaults.IsNotNull(columnName);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("[Username] IS NULL", "Username")]
        public void Generate_IsNull(string expected, string columnName)
        {
            var result = MappingDefaults.IsNull(columnName);

            Assert.Equal(expected, result);
        }
    }
}
