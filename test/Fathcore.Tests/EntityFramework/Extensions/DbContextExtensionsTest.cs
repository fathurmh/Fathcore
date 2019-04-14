using System;
using System.Linq;
using System.Text;
using Fathcore.EntityFramework;
using Fathcore.EntityFramework.Extensions;
using Fathcore.Infrastructure;
using Fathcore.Tests.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Fathcore.Tests.EntityFramework.Extensions
{
    public class DbContextExtensionsTest : TestBase
    {
        [Fact]
        public void ExecuteSqlScript_SafetyCheck()
        {
            Assert.Throws<ArgumentNullException>(() => default(IDbContext).ExecuteSqlScript(""));
        }

        [Fact]
        public void Should_ExecuteSqlScript()
        {
            var connection = new SqliteConnection($"DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder().UseSqlite(connection).Options;

            using (var context = new TestDbContext(options))
            {
                context.ExecuteSqlScript(context.GenerateCreateScript());
                context.ExecuteSqlScript("");
                context.ExecuteSqlScript(
                    @"
                        SELECT * FROM Classroom
                        GO 1
                    ");
            }

            using (var context = new TestDbContext(options))
            {
                Assert.Throws<SqliteException>(() => context.ExecuteSqlScript(context.GenerateCreateScript()));
            }

            connection.Close();
        }

        [Fact]
        public void Should_ExecuteSqlScript_2()
        {
            var options = OptionsWithData("Should_ExecuteSqlScript_2", Provider.Sqlite);

            Classroom classroom;

            using (var context = new TestDbContext(options))
            {
                classroom = context.Set<Classroom>().First();
                context.ExecuteSqlScript($"UPDATE [Classroom] SET [Code] = '{"Modified"}' WHERE Id = {classroom.Id}");
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<Classroom>().First();
                Assert.Equal("Modified", result.Code);
            }
        }

        [Fact]
        public void ExecuteSqlScriptFromFile_SafetyCheck()
        {
            Assert.Throws<ArgumentNullException>(() => default(IDbContext).ExecuteSqlScriptFromFile(""));
        }

        [Fact]
        public void Should_ExecuteSqlScriptFromFile()
        {
            var mockHostingEnvironment = new Mock<IHostingEnvironment>();
            mockHostingEnvironment.Setup(x => x.ContentRootPath).Returns(AppDomain.CurrentDomain.BaseDirectory);
            mockHostingEnvironment.Setup(x => x.WebRootPath).Returns(AppDomain.CurrentDomain.BaseDirectory);

            var fileProvider = new FileProvider(mockHostingEnvironment.Object);

            var connection = new SqliteConnection($"DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder().UseSqlite(connection).Options;

            using (var context = new TestDbContext(options))
            {
                fileProvider.WriteAllText("script.sql", context.GenerateCreateScript(), Encoding.Default);
                context.ExecuteSqlScriptFromFile("script.sql");
            }

            using (var context = new TestDbContext(options))
            {
                Assert.Throws<SqliteException>(() => context.ExecuteSqlScriptFromFile("script.sql"));
            }

            connection.Close();
        }

        [Fact]
        public void GetCurrentEntries_SafetyCheck()
        {
            Assert.Throws<ArgumentNullException>(() => default(IDbContext).GetCurrentEntries());
        }
    }
}
