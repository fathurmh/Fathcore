using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fathcore.EntityFramework.Extensions;
using Fathcore.Tests.EntityFramework.Fakes;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fathcore.Tests.EntityFramework
{
    public class DbContextExtensionsTest
    {
        [Fact]
        public void Should_Execute_Sql_Script()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(connection)
                .Options;

            using (var context = new TestDbContext(options))
            {
                context.ExecuteSqlScript(context.GenerateCreateScript());
                var result = context
                    .QueryFromSql<StringQueryType>("SELECT name as Value FROM sqlite_master WHERE type='table'")
                    .Select(p => p.Value).ToList();

                Assert.Contains("TestEntity", result);
                Assert.Contains("ChildTestEntity", result);
            }
        }

        [Fact]
        public void Should_Execute_Sql_Script_From_File()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(connection)
                .Options;

            using (var context = new TestDbContext(options))
            {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "script.sql"), context.GenerateCreateScript());
                context.ExecuteSqlScriptFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "script.sql"));

                var result = context
                    .QueryFromSql<StringQueryType>("SELECT name as Value FROM sqlite_master WHERE type='table'")
                    .Select(p => p.Value).ToList();

                Assert.Contains("TestEntity", result);
                Assert.Contains("ChildTestEntity", result);
            }
        }

        [Fact]
        public void Should_Get_Current_Entries()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Get_Current_Entries")
                .Options;

            var testEntity = new TestEntity().GenerateData();

            using (var context = new TestDbContext(options))
            {
                context.AddRange(testEntity);

                var result = context.GetCurrentEntries().ToList();

                Assert.Equal(30, result.Count);
            }
        }

        [Fact]
        public void Should_Rollback_Entity_Changes()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Rollback_Entity_Changes")
                .Options;

            var testEntity = new TestEntity().GenerateData().First();

            using (var context = new TestDbContext(options))
            {
                context.Add(testEntity);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var entity = context.Set<TestEntity>().Find(testEntity.Id);
                entity.TestField = "Modified";
                context.Update(entity);
                context.RollbackEntityChanges(new DbUpdateException("DbUpdateException", new Exception()));
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<TestEntity>().Include(prop => prop.ChildTestEntities).First(prop => prop.Id == testEntity.Id);
                Assert.NotEqual("Modified", result.TestField);
            }
        }

        #region Async

        [Fact]
        public async Task Should_Asynchronously_Rollback_Entity_Changes()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Asynchronously_Rollback_Entity_Changes")
                .Options;

            var testEntity = new TestEntity().GenerateData().First();

            using (var context = new TestDbContext(options))
            {
                await context.AddAsync(testEntity);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var entity = context.Set<TestEntity>().Find(testEntity.Id);
                entity.TestField = "Modified";
                context.Update(entity);
                await context.RollbackEntityChangesAsync(new DbUpdateException("DbUpdateException", new Exception()));
            }

            using (var context = new TestDbContext(options))
            {
                var result = await context.Set<TestEntity>().Include(prop => prop.ChildTestEntities).FirstAsync(prop => prop.Id == testEntity.Id);
                Assert.NotEqual("Modified", result.TestField);
            }
        }

        #endregion
    }
}
