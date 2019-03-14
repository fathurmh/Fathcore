using System;
using System.Linq;
using Fathcore.EntityFramework.Extensions;
using Fathcore.EntityFramework.Fakes;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fathcore.EntityFramework
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
        public void Detach_An_Entity_Should_Throw_When_Entity_Null()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "Should_Detach_An_Entity")
                .Options;

            using (var context = new TestDbContext(options))
            {
                Assert.Throws<ArgumentNullException>(() => context.Detach<TestEntity>(null));
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

        [Fact]
        public void Should_Exec_Entity_From_Sql()
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
                var result = context.EntityFromSql<TestEntity>($"SELECT * FROM [{nameof(TestEntity)}]").Select(p => p).FirstOrDefault();
                Assert.Equal("Modified", result.TestField);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Should_Exec_Sql_Command(bool ensureTransaction)
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
                context.ExecuteSqlCommand("UPDATE [TestEntity] SET [TestField] = {0} WHERE Id = {1}", ensureTransaction, null, "Modified", testEntity.Id);
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.EntityFromSql<TestEntity>($"SELECT * FROM [{nameof(TestEntity)}]").Select(p => p).FirstOrDefault();
                Assert.Equal("Modified", result.TestField);
            }
        }

        [Fact]
        public void Should_Exec_Entity_From_Sql_With_Parameters()
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
                var param = new SqliteParameter("Id", testEntity.Id);
                var result = context.EntityFromSql<TestEntity>("SELECT * FROM [TestEntity] WHERE [Id] = ", param).FirstOrDefault();
                Assert.Equal(testEntity.TestField, result.TestField);
            }
        }
    }
}
