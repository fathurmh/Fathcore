using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fathcore.EntityFramework.AuditTrail;
using Fathcore.EntityFramework.Tests.Fakes;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Fathcore.EntityFramework.Tests
{
    public class BaseDbContextTest
    {
        public IServiceCollection ServiceDescriptors { get; } = new ServiceCollection();

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void BaseDbContext_ShouldCreateModels(Provider provider)
        {
            var options = TestHelper.Options("BaseDbContext_ShouldCreateModels", provider);
            using (var context = new TestDbContext(options))
            {
                var result = context.Model.GetEntityTypes();

                Assert.NotEmpty(result);
                Assert.Equal(4, result.Count());
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Detach_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("Detach_ShouldPass", provider);

            using (var context = new TestDbContext(options))
            {
                var entities = context.Set<Classroom>();
                foreach (var entity in entities)
                {
                    entity.Code = "Modified";
                    context.Detach(entity);
                }

                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<Classroom>().ToList();
                Assert.All(result, p => Assert.NotEqual("Modified", p.Code));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void DetachRange_SafetyCheck(Provider provider)
        {
            var options = TestHelper.OptionsWithData("DetachRange_SafetyCheck", provider);

            using (var context = new TestDbContext(options))
            {
                Assert.Throws<ArgumentNullException>(() => context.DetachRange(default(IEnumerable<Classroom>)));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void DetachRange_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("DetachRange_ShouldPass", provider);

            using (var context = new TestDbContext(options))
            {
                var entities = context.Set<Classroom>();
                foreach (var entity in entities)
                {
                    entity.Code = "Modified";
                }

                context.DetachRange(entities);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<Classroom>().ToList();
                Assert.All(result, p => Assert.NotEqual("Modified", p.Code));
            }
        }

        [Theory]
        [InlineData(Provider.Sqlite)]
        public void EntityFromSql_ShouldExecuted(Provider provider)
        {
            var options = TestHelper.OptionsWithData("EntityFromSql_ShouldExecuted", provider);

            using (var context = new TestDbContext(options))
            {
                var entity = context.Set<Classroom>().First();
                entity.Code = "Modified";
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.EntityFromSql<Classroom>($"SELECT * FROM [{nameof(Classroom)}]").First();
                Assert.Equal("Modified", result.Code);
            }
        }

        [Theory]
        [InlineData(Provider.Sqlite)]
        public void EntityFromSql_WithParameters_ShouldExecuted(Provider provider)
        {
            var options = TestHelper.OptionsWithData("EntityFromSql_WithParameters_ShouldExecuted", provider);

            Classroom entity;
            using (var context = new TestDbContext(options))
            {
                entity = context.Set<Classroom>().First();
                entity.Code = "Modified";
                context.SaveChanges();

            }

            using (var context = new TestDbContext(options))
            {
                var param = new SqliteParameter("Id", entity.Id);
                var result = context.EntityFromSql<Classroom>("SELECT * FROM [Classroom] WHERE [Id] = ", param).First();
                Assert.Equal("Modified", result.Code);
            }
        }

        [Theory]
        [InlineData(Provider.Sqlite, true)]
        [InlineData(Provider.Sqlite, false)]
        public void ExecuteSqlCommand_ShouldExecuted(Provider provider, bool ensureTransaction)
        {
            var options = TestHelper.OptionsWithData("ExecuteSqlCommand_ShouldExecuted", provider);

            Classroom entity;

            using (var context = new TestDbContext(options))
            {
                entity = context.Set<Classroom>().First();
                context.ExecuteSqlCommand("UPDATE [Classroom] SET [Code] = {0} WHERE Id = {1}", ensureTransaction, null, "Modified", entity.Id);
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.EntityFromSql<Classroom>("SELECT * FROM [Classroom] WHERE Id > 0", "0").First();
                Assert.Equal("Modified", result.Code);
            }
        }

        [Theory]
        [InlineData(Provider.Sqlite)]
        public void GenerateCreateScript_ShouldExecuted(Provider provider)
        {
            var options = TestHelper.OptionsWithData("GenerateCreateScript_ShouldExecuted", provider);

            using (var context = new TestDbContext(options))
            {
                var script = context.GenerateCreateScript();

                Assert.True(!string.IsNullOrWhiteSpace(script));
                Assert.Contains("CREATE", script);
            }
        }

        [Theory]
        [InlineData(Provider.Sqlite)]
        public void QueryFromSql_ShouldExecuted(Provider provider)
        {
            var options = TestHelper.OptionsWithData("QueryFromSql_ShouldExecuted", provider);

            Classroom entity;

            using (var context = new TestDbContext(options))
            {
                entity = context.Set<Classroom>().First();
                entity.Code = "Modified";
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.QueryFromSql<StringQueryType>($"SELECT {nameof(Classroom.Code)} AS {nameof(StringQueryType.Value)} FROM [{nameof(Classroom)}]").Select(p => p.Value).FirstOrDefault();
                Assert.Equal("Modified", result);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory, true, TestHelper.DefaultIdentity)]
        [InlineData(Provider.InMemory, false, "")]
        [InlineData(Provider.Sqlite, true, TestHelper.DefaultIdentity)]
        [InlineData(Provider.Sqlite, false, "")]
        public void SaveChanges_ShouldSave_Entity(Provider provider, bool injectAuditHandler, string expectedCreatedBy)
        {
            var options = TestHelper.Options($"SaveChanges_ShouldSave_Entity_{expectedCreatedBy}", provider);
            var auditHandler = new AuditHandler(TestHelper.HttpContextAccessor);

            var entities = FakeEntityGenerator.Classrooms;

            using (var context = injectAuditHandler ? new TestDbContext(options, auditHandler) : new TestDbContext(options))
            {
                context.AddRange(entities);
                context.SaveChanges();
            }

            using (var context = injectAuditHandler ? new TestDbContext(options, auditHandler) : new TestDbContext(options))
            {
                var result = context.Set<Classroom>().ToList();
                Assert.NotEmpty(result);

                if (injectAuditHandler)
                    Assert.All(result, p => Assert.Equal(expectedCreatedBy, p.CreatedBy));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory, true, TestHelper.DefaultIdentity)]
        [InlineData(Provider.InMemory, false, "")]
        [InlineData(Provider.Sqlite, true, TestHelper.DefaultIdentity)]
        [InlineData(Provider.Sqlite, false, "")]
        public async Task SaveChangesAsync_ShouldSave_Entity(Provider provider, bool injectAuditHandler, string expectedCreatedBy)
        {
            var options = TestHelper.Options($"SaveChangesAsync_ShouldSave_Entity_{expectedCreatedBy}", provider);
            var auditHandler = new AuditHandler(TestHelper.HttpContextAccessor);

            var entities = FakeEntityGenerator.Classrooms;

            using (var context = injectAuditHandler ? new TestDbContext(options, auditHandler) : new TestDbContext(options))
            {
                await context.AddRangeAsync(entities);
                await context.SaveChangesAsync();
            }

            using (var context = injectAuditHandler ? new TestDbContext(options, auditHandler) : new TestDbContext(options))
            {
                var result = await context.Set<Classroom>().ToListAsync();
                Assert.NotEmpty(result);

                if (injectAuditHandler)
                    Assert.All(result, p => Assert.Equal(expectedCreatedBy, p.CreatedBy));
            }
        }
    }
}
