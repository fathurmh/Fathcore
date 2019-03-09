using System.Linq;
using Fathcore.EntityFramework.Extensions;
using Fathcore.Tests.EntityFramework.Fakes;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fathcore.Tests.EntityFramework
{
    public class BaseDbContextTest
    {
        [Fact]
        public void Should_Detach_An_Entity()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Detach_An_Entity")
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
                context.Detach(entity);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<TestEntity>().Find(testEntity.Id);
                Assert.NotEqual("Modified", result.TestField);
            }
        }

        [Fact]
        public void Should_Exec_Query_From_Sql()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(connection)
                .Options;

            using (var context = new TestDbContext(options))
            {
                context.ExecuteSqlScript(context.GenerateCreateScript());
            }

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
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.QueryFromSql<StringQueryType>($"SELECT {nameof(TestEntity.TestField)} AS {nameof(StringQueryType.Value)} FROM [{nameof(TestEntity)}]").Select(p => p.Value).FirstOrDefault();
                Assert.Equal("Modified", result);
            }
        }
    }
}
