using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Fathcore.EntityFramework;
using Fathcore.EntityFramework.AuditTrail;
using Fathcore.EntityFramework.Extensions;
using Fathcore.Tests.Fakes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Fathcore.Tests.EntityFramework.Extensions
{
    public class RepositoryExtensionsTest : TestBase
    {
        public IHttpContextAccessor HttpContextAccessor
        {
            get
            {
                var mock = new Mock<IHttpContextAccessor>();
                var context = new DefaultHttpContext()
                {
                    User = new GenericPrincipal(new GenericIdentity(DefaultIdentity), null)
                };

                mock.Setup(p => p.HttpContext).Returns(context);
                return mock.Object;
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]

        public async Task TableQueryableAsync_AsTracking(Provider provider)
        {
            var options = OptionsWithData("TableQueryableAsync_AsTracking", provider);
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var query = from c in repository.Table select c;

                var result = await query.ToListAsync();

                Assert.NotEmpty(result);
                Assert.All(result, prop => Assert.Contains(context.Set<Classroom>().Local, e => e.Id == prop.Id));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]

        public async Task TableQueryableAsync_AsNoTracking(Provider provider)
        {
            var options = OptionsWithData("TableQueryableAsync_AsNoTracking", provider);
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                repository.AsNoTracking();
                var query = from c in repository.Table select c;

                var result = await query.ToListAsync();

                Assert.NotEmpty(result);
                Assert.All(result, prop => Assert.DoesNotContain(context.Set<Classroom>().Local, e => e.Id == prop.Id));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]

        public async Task TableQueryableAsync_Default_EnabledQueryFilter(Provider provider)
        {
            var options = OptionsWithData("TableQueryableAsync_Default_EnabledQueryFilter", provider);
            var auditHandler = new AuditHandler(HttpContextAccessor);
            int totalBefore;

            using (var context = new TestDbContext(options))
            {
                var entity = await context.Set<Classroom>().FirstAsync();
                totalBefore = await context.Set<Classroom>().CountAsync();
                context.Remove(entity);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var query = from c in repository.Table select c;

                var result = await query.ToListAsync();

                Assert.True(result.Count < totalBefore);
                Assert.All(result, prop => Assert.True(!prop.IsDeleted));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]

        public async Task TableQueryableAsync_IgnoreQueryFilter(Provider provider)
        {
            var options = OptionsWithData("TableQueryableAsync_IgnoreQueryFilter", provider);
            var auditHandler = new AuditHandler(HttpContextAccessor);
            int totalBefore;

            using (var context = new TestDbContext(options))
            {
                var entity = await context.Set<Classroom>().FirstAsync();
                totalBefore = await context.Set<Classroom>().CountAsync();
                context.Remove(entity);
                await auditHandler.HandleAsync(context);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var query = from c in repository.TableNoFilters select c;

                var result = await query.ToListAsync();

                Assert.True(result.Count == totalBefore);
                Assert.Contains(result, prop => prop.IsDeleted);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task SelectListAsync_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SelectListAsync_ShouldPass", provider);
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                var result = await repository.SelectListAsync();

                Assert.Equal(FakeEntityGenerator.Classrooms.Count, result.Count());
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task SelectListAsync_WithLambda_NavigationProperty_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SelectListAsync_WithLambda_NavigationProperty_ShouldPass", provider);
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                var result = await repository.SelectListAsync(prop => prop.Students);

                Assert.Equal(FakeEntityGenerator.Classrooms.Count, result.Count());
                Assert.Equal(FakeEntityGenerator.Classrooms.Sum(p => p.Students.Count), result.Sum(p => p.Students.Count));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task SelectListAsync_WithString_NavigationProperty_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SelectListAsync_WithString_NavigationProperty_ShouldPass", provider);
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                var result = await repository.SelectListAsync($"{nameof(Classroom.Students)}");

                Assert.Equal(FakeEntityGenerator.Classrooms.Count, result.Count());
                Assert.Equal(FakeEntityGenerator.Classrooms.Sum(p => p.Students.Count), result.Sum(p => p.Students.Count));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task SelectListAsync_MatchesPredicate_WithoutNavigationProperty_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SelectListAsync_MatchesPredicate_WithoutNavigationProperty_ShouldPass", provider);
            var entity = FakeEntityGenerator.Classrooms.First();
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                var result = await repository.SelectListAsync(prop => prop.Code == entity.Code);

                Assert.Single(result);
                Assert.Empty(result.Single().Students);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task SelectListAsync_MatchesPredicate_WithLambda_NavigationProperty_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SelectListAsync_MatchesPredicate_WithLambda_NavigationProperty_ShouldPass", provider);
            var entity = FakeEntityGenerator.Classrooms.First();
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                var result = await repository.SelectListAsync(prop => prop.Code == entity.Code, prop => prop.Students);

                Assert.Single(result);
                Assert.NotEmpty(result.Single().Students);
                Assert.Equal(entity.Students.Count, result.Single().Students.Count);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task SelectListAsync_MatchesPredicate_WithString_NavigationProperty_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SelectListAsync_MatchesPredicate_WithString_NavigationProperty_ShouldPass", provider);
            var entity = FakeEntityGenerator.Classrooms.First();
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                var result = await repository.SelectListAsync(prop => prop.Code == entity.Code, $"{nameof(Classroom.Students)}");

                Assert.Single(result);
                Assert.NotEmpty(result.Single().Students);
                Assert.Equal(entity.Students.Count, result.Single().Students.Count);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task SelectAsync_MatchesPredicate_WithoutNavigationProperty_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SelectAsync_MatchesPredicate_WithoutNavigationProperty_ShouldPass", provider);
            var entity = FakeEntityGenerator.Classrooms.First();
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                var result = await repository.SelectAsync(prop => prop.Code == entity.Code);

                Assert.NotNull(result);
                Assert.Empty(result.Students);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task SelectAsync_MatchesPredicate_WithLambda_NavigationProperty_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SelectAsync_MatchesPredicate_WithLambda_NavigationProperty_ShouldPass", provider);
            var entity = FakeEntityGenerator.Classrooms.First();
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                var result = await repository.SelectAsync(prop => prop.Code == entity.Code, prop => prop.Students);

                Assert.NotNull(result);
                Assert.NotEmpty(result.Students);
                Assert.Equal(entity.Students.Count, result.Students.Count);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task SelectAsync_MatchesPredicate_WithString_NavigationProperty_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SelectAsync_MatchesPredicate_WithString_NavigationProperty_ShouldPass", provider);
            var entity = FakeEntityGenerator.Classrooms.First();
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                var result = await repository.SelectAsync(prop => prop.Code == entity.Code, $"{nameof(Classroom.Students)}");

                Assert.NotNull(result);
                Assert.NotEmpty(result.Students);
                Assert.Equal(entity.Students.Count, result.Students.Count);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task SelectAsync_MatchesPrimaryKey_SafetyCheck(Provider provider)
        {
            var options = Options("SelectAsync_MatchesPrimaryKey_SafetyCheck", provider);

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                await Assert.ThrowsAsync<ArgumentNullException>(() => repository.SelectAsync(keyValue: null));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory, true)]
        [InlineData(Provider.InMemory, false)]
        [InlineData(Provider.Sqlite, true)]
        [InlineData(Provider.Sqlite, false)]
        public async Task SelectAsync_MatchesPrimaryKey_ShouldPass(Provider provider, bool asTracking)
        {
            var options = OptionsWithData("SelectAsync_MatchesPrimaryKey_ShouldPass", provider);
            Classroom entity;

            using (var context = new TestDbContext(options))
            {
                entity = await context.Set<Classroom>().FirstAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                if (!asTracking)
                    repository.AsNoTracking();

                var result = await repository.SelectAsync(entity.Id);

                Assert.NotNull(result);
                Assert.Empty(result.Students);
                if (asTracking)
                    Assert.Contains(context.Set<Classroom>().Local, e => e.Id == result.Id);
                else
                    Assert.DoesNotContain(context.Set<Classroom>().Local, e => e.Id == result.Id);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task InsertAsync_Entity_SafetyCheck(Provider provider)
        {
            var options = Options("InsertAsync_Entity_SafetyCheck", provider);
            Classroom entity = default;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                await Assert.ThrowsAsync<ArgumentNullException>(() => repository.InsertAsync(entity));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task InsertAsync_Entity_ShouldPass(Provider provider)
        {
            var options = Options("InsertAsync_Entity_ShouldPass", provider);
            Classroom entity = FakeEntityGenerator.Classrooms.First();

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                await repository.InsertAsync(entity);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var result = await repository.SelectAsync(entity.Id);

                Assert.NotNull(result);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task InsertAsync_Entities_SafetyCheck(Provider provider)
        {
            var options = Options("InsertAsync_Entities_SafetyCheck", provider);
            IEnumerable<Classroom> entities = default;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                await Assert.ThrowsAsync<ArgumentNullException>(() => repository.InsertAsync(entities));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task InsertAsync_Entities_ShouldPass(Provider provider)
        {
            var options = Options("InsertAsync_Entities_ShouldPass", provider);
            IEnumerable<Classroom> entities = FakeEntityGenerator.Classrooms;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                await repository.InsertAsync(entities);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var result = await repository.SelectListAsync();

                Assert.NotEmpty(result);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task UpdateAsync_Entity_SafetyCheck(Provider provider)
        {
            var options = OptionsWithData("UpdateAsync_Entity_SafetyCheck", provider);
            Classroom entity = default;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                await Assert.ThrowsAsync<ArgumentNullException>(() => repository.UpdateAsync(entity));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task UpdateAsync_Entity_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("UpdateAsync_Entity_ShouldPass", provider);
            Classroom entity;
            string modified = "Modified";

            using (var context = new TestDbContext(options))
            {
                entity = context.Set<Classroom>().First();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                entity.Code = modified;
                await repository.UpdateAsync(entity);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var result = await context.Set<Classroom>().FirstAsync();

                Assert.Equal(modified, result.Code);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task UpdateAsync_Entities_SafetyCheck(Provider provider)
        {
            var options = OptionsWithData("UpdateAsync_Entities_SafetyCheck", provider);
            IEnumerable<Classroom> entities = default;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                await Assert.ThrowsAsync<ArgumentNullException>(() => repository.UpdateAsync(entities));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task UpdateAsync_Entities_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("UpdateAsync_Entities_ShouldPass", provider);
            IEnumerable<Classroom> entities;
            string modified = "Modified";

            using (var context = new TestDbContext(options))
            {
                entities = await context.Set<Classroom>().ToListAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                foreach (var entity in entities)
                {
                    entity.Code = modified;
                }
                await repository.UpdateAsync(entities);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var result = await context.Set<Classroom>().ToListAsync();

                Assert.All(result, p => Assert.Equal(modified, p.Code));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task DeleteAsync_Entity_SafetyCheck(Provider provider)
        {
            var options = OptionsWithData("DeleteAsync_Entity_SafetyCheck", provider);
            Classroom entity = default;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                await Assert.ThrowsAsync<ArgumentNullException>(() => repository.DeleteAsync(entity));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task DeleteAsync_Entity_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("DeleteAsync_Entity_ShouldPass", provider);
            Classroom entity;

            using (var context = new TestDbContext(options))
            {
                entity = await context.Set<Classroom>().FirstAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                await repository.DeleteAsync(entity);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var result = await context.Set<Classroom>().FindAsync(entity.Id);

                Assert.Null(result);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task DeleteAsync_Entity_ByPrimaryKey_SafetyCheck(Provider provider)
        {
            var options = OptionsWithData("DeleteAsync_Entity_ByPrimaryKey_SafetyCheck", provider);

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                await Assert.ThrowsAsync<ArgumentNullException>(() => repository.DeleteAsync(keyValue: null));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task DeleteAsync_Entity_ByPrimaryKey_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("DeleteAsync_Entity_ByPrimaryKey_ShouldPass", provider);
            int keyValue;

            using (var context = new TestDbContext(options))
            {
                var entity = await context.Set<Classroom>().FirstAsync();
                keyValue = entity.Id;
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                await repository.DeleteAsync(keyValue);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var result = await context.Set<Classroom>().FindAsync(keyValue);

                Assert.Null(result);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task DeleteAsync_Entities_SafetyCheck(Provider provider)
        {
            var options = OptionsWithData("DeleteAsync_Entities_SafetyCheck", provider);
            IEnumerable<Classroom> entities = default;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                await Assert.ThrowsAsync<ArgumentNullException>(() => repository.DeleteAsync(entities));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task DeleteAsync_Entities_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("DeleteAsync_Entities_ShouldPass", provider);
            IEnumerable<Classroom> entities = default;

            using (var context = new TestDbContext(options))
            {
                entities = await context.Set<Classroom>().ToListAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                await repository.DeleteAsync(entities);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var result = await context.Set<Classroom>().ToListAsync();

                Assert.Empty(result);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task SaveChangesAsync_AsTracking_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SaveChangesAsync_AsTracking_ShouldPass", provider);
            Classroom entity;
            string modified = "Modified";

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                entity = await context.Set<Classroom>().FirstAsync();
                entity.Code = modified;
                await repository.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var result = await context.Set<Classroom>().FirstAsync();

                Assert.Equal(modified, result.Code);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task SaveChangesAsync_AsNoTracking_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SaveChangesAsync_AsNoTracking_ShouldPass", provider);
            Classroom entity;
            string modified = "Modified";

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                repository.AsNoTracking();
                entity = await context.Set<Classroom>().FirstAsync();
                entity.Code = modified;
                await repository.UpdateAsync(entity);
                await repository.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var result = await context.Set<Classroom>().FirstAsync();

                Assert.Equal(modified, result.Code);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task SaveChangesAsync_AsNoTracking_Disconnected_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SaveChangesAsync_AsNoTracking_Disconnected_ShouldPass", provider);
            Classroom entity;
            string modified = "Modified";

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                entity = await context.Set<Classroom>().FirstAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                repository.AsNoTracking();
                entity.Code = modified;
                await repository.UpdateAsync(entity);
                await repository.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var result = await context.Set<Classroom>().FirstAsync();

                Assert.Equal(modified, result.Code);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task SaveChangesAsync_AsNoTracking_ShouldNotSaved(Provider provider)
        {
            var options = OptionsWithData("SaveChangesAsync_AsNoTracking_ShouldNotSaved", provider);
            Classroom entity;
            string modified = "Modified";

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                repository.AsNoTracking();
                entity = await context.Set<Classroom>().FirstAsync();
                entity.Code = modified;
                await repository.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var result = await context.Set<Classroom>().FirstAsync();

                Assert.NotEqual(modified, result.Code);
            }
        }

        [Theory]
        //[InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public async Task SaveChangesAsync_ShouldRollbackEntityChanges(Provider provider)
        {
            var options = OptionsWithData("SaveChangesAsync_ShouldRollbackEntityChanges", provider);
            Classroom entity;
            string modifiedLinq = "Modified By Linq";
            string modifiedSqlCommand = "Modified By SqlCommand";

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                entity = await context.Set<Classroom>().FirstAsync();
                entity.Code = modifiedLinq;

                // simulate a concurrency conflict
                context.ExecuteSqlCommand("UPDATE [Classroom] SET [Code] = {0} WHERE Id = {1}", false, null, modifiedSqlCommand, entity.Id);

                await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => repository.SaveChangesAsync());
            }

            using (var context = new TestDbContext(options))
            {
                var result = await context.Set<Classroom>().FirstAsync();

                Assert.NotEqual(modifiedLinq, result.Code);
                Assert.Equal(modifiedSqlCommand, result.Code);
            }
        }
    }
}
