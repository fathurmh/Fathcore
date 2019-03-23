using System;
using System.Collections.Generic;
using System.Linq;
using Fathcore.EntityFramework;
using Fathcore.EntityFramework.AuditTrail;
using Fathcore.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fathcore.Tests.EntityFramework
{
    public class RepositoryTest : TestBase
    {
        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]

        public void DbContextParameter_ShouldTheSame(Provider provider)
        {
            var options = Options("DbContextParameter_ShouldTheSame", provider);
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                Assert.Same(context, repository.DbContext);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]

        public void Repository_Default_QueryTrackingBehavior_AsTracking(Provider provider)
        {
            var options = Options("Repository_Default_QueryTrackingBehavior_AsTracking", provider);
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                Assert.Equal(QueryTrackingBehavior.TrackAll, repository.DbContext.ChangeTracker.QueryTrackingBehavior);
                Assert.Equal(QueryTrackingBehavior.TrackAll, context.ChangeTracker.QueryTrackingBehavior);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]

        public void AsTracking_ShouldChange_QueryTrackingBehavior(Provider provider)
        {
            var options = Options("AsTracking_ShouldChange_QueryTrackingBehavior", provider);
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                repository.AsTracking();
                Assert.Equal(QueryTrackingBehavior.TrackAll, repository.DbContext.ChangeTracker.QueryTrackingBehavior);
                Assert.Equal(QueryTrackingBehavior.TrackAll, context.ChangeTracker.QueryTrackingBehavior);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]

        public void AsNoTracking_ShouldChange_QueryTrackingBehavior(Provider provider)
        {
            var options = Options("AsNoTracking_ShouldChange_QueryTrackingBehavior", provider);
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                repository.AsNoTracking();
                Assert.Equal(QueryTrackingBehavior.NoTracking, repository.DbContext.ChangeTracker.QueryTrackingBehavior);
                Assert.Equal(QueryTrackingBehavior.NoTracking, context.ChangeTracker.QueryTrackingBehavior);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]

        public void Tracking_ShouldChange_QueryTrackingBehavior(Provider provider)
        {
            var options = Options("Tracking_ShouldChange_QueryTrackingBehavior", provider);
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                Assert.Equal(QueryTrackingBehavior.TrackAll, repository.DbContext.ChangeTracker.QueryTrackingBehavior);
                Assert.Equal(QueryTrackingBehavior.TrackAll, context.ChangeTracker.QueryTrackingBehavior);

                repository.AsNoTracking();

                Assert.Equal(QueryTrackingBehavior.NoTracking, repository.DbContext.ChangeTracker.QueryTrackingBehavior);
                Assert.Equal(QueryTrackingBehavior.NoTracking, context.ChangeTracker.QueryTrackingBehavior);

                repository.AsTracking();

                Assert.Equal(QueryTrackingBehavior.TrackAll, repository.DbContext.ChangeTracker.QueryTrackingBehavior);
                Assert.Equal(QueryTrackingBehavior.TrackAll, context.ChangeTracker.QueryTrackingBehavior);

                repository.AsNoTracking();

                Assert.Equal(QueryTrackingBehavior.NoTracking, repository.DbContext.ChangeTracker.QueryTrackingBehavior);
                Assert.Equal(QueryTrackingBehavior.NoTracking, context.ChangeTracker.QueryTrackingBehavior);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]

        public void TableQueryable_AsTracking(Provider provider)
        {
            var options = OptionsWithData("TableQueryable_AsTracking", provider);
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var query = from c in repository.Table select c;

                var result = query.ToList();

                Assert.NotEmpty(result);
                Assert.All(result, prop => Assert.Contains(context.Set<Classroom>().Local, e => e.Id == prop.Id));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]

        public void TableQueryable_AsNoTracking(Provider provider)
        {
            var options = OptionsWithData("TableQueryable_AsNoTracking", provider);
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                repository.AsNoTracking();
                var query = from c in repository.Table select c;

                var result = query.ToList();

                Assert.NotEmpty(result);
                Assert.All(result, prop => Assert.DoesNotContain(context.Set<Classroom>().Local, e => e.Id == prop.Id));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]

        public void TableQueryable_Default_EnabledQueryFilter(Provider provider)
        {
            var options = OptionsWithData("TableQueryable_Default_EnabledQueryFilter", provider);
            var auditHandler = new AuditHandler(HttpContextAccessor);
            int totalBefore;

            using (var context = new TestDbContext(options))
            {
                var entity = context.Set<Classroom>().First();
                totalBefore = context.Set<Classroom>().Count();
                context.Remove(entity);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var query = from c in repository.Table select c;

                var result = query.ToList();

                Assert.True(result.Count < totalBefore);
                Assert.All(result, prop => Assert.True(!prop.IsDeleted));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]

        public void TableQueryable_IgnoreQueryFilter(Provider provider)
        {
            var options = OptionsWithData("TableQueryable_Default_AsFiltered", provider);
            var auditHandler = new AuditHandler(HttpContextAccessor);
            int totalBefore;

            using (var context = new TestDbContext(options))
            {
                var entity = context.Set<Classroom>().First();
                totalBefore = context.Set<Classroom>().Count();
                context.Remove(entity);
                auditHandler.Handle(context);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var query = from c in repository.TableNoFilters select c;

                var result = query.ToList();

                Assert.True(result.Count == totalBefore);
                Assert.Contains(result, prop => prop.IsDeleted);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void SelectList_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SelectList_ShouldPass", provider);
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                var result = repository.SelectList();

                Assert.Equal(FakeEntityGenerator.Classrooms.Count, result.Count());
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void SelectList_WithLambda_NavigationProperty_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SelectList_WithLambda_NavigationProperty_ShouldPass", provider);
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                var result = repository.SelectList(prop => prop.Students);

                Assert.Equal(FakeEntityGenerator.Classrooms.Count, result.Count());
                Assert.Equal(FakeEntityGenerator.Classrooms.Sum(p => p.Students.Count), result.Sum(p => p.Students.Count));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void SelectList_WithString_NavigationProperty_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SelectList_WithLambda_NavigationProperty_ShouldPass", provider);
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                var result = repository.SelectList($"{nameof(Classroom.Students)}");

                Assert.Equal(FakeEntityGenerator.Classrooms.Count, result.Count());
                Assert.Equal(FakeEntityGenerator.Classrooms.Sum(p => p.Students.Count), result.Sum(p => p.Students.Count));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void SelectList_MatchesPredicate_WithoutNavigationProperty_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SelectList_MatchesPredicate_WithoutNavigationProperty_ShouldPass", provider);
            var entity = FakeEntityGenerator.Classrooms.First();
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                var result = repository.SelectList(prop => prop.Code == entity.Code);

                Assert.Single(result);
                Assert.Empty(result.Single().Students);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void SelectList_MatchesPredicate_WithLambda_NavigationProperty_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SelectList_MatchesPredicate_WithLambda_NavigationProperty_ShouldPass", provider);
            var entity = FakeEntityGenerator.Classrooms.First();
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                var result = repository.SelectList(prop => prop.Code == entity.Code, prop => prop.Students);

                Assert.Single(result);
                Assert.NotEmpty(result.Single().Students);
                Assert.Equal(entity.Students.Count, result.Single().Students.Count);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void SelectList_MatchesPredicate_WithString_NavigationProperty_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SelectList_MatchesPredicate_WithString_NavigationProperty_ShouldPass", provider);
            var entity = FakeEntityGenerator.Classrooms.First();
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                var result = repository.SelectList(prop => prop.Code == entity.Code, $"{nameof(Classroom.Students)}");

                Assert.Single(result);
                Assert.NotEmpty(result.Single().Students);
                Assert.Equal(entity.Students.Count, result.Single().Students.Count);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Select_MatchesPredicate_WithoutNavigationProperty_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("Select_MatchesPredicate_WithoutNavigationProperty_ShouldPass", provider);
            var entity = FakeEntityGenerator.Classrooms.First();
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                var result = repository.Select(prop => prop.Code == entity.Code);

                Assert.NotNull(result);
                Assert.Empty(result.Students);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Select_MatchesPredicate_WithLambda_NavigationProperty_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("Select_MatchesPredicate_WithLambda_NavigationProperty_ShouldPass", provider);
            var entity = FakeEntityGenerator.Classrooms.First();
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                var result = repository.Select(prop => prop.Code == entity.Code, prop => prop.Students);

                Assert.NotNull(result);
                Assert.NotEmpty(result.Students);
                Assert.Equal(entity.Students.Count, result.Students.Count);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Select_MatchesPredicate_WithString_NavigationProperty_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("Select_MatchesPredicate_WithString_NavigationProperty_ShouldPass", provider);
            var entity = FakeEntityGenerator.Classrooms.First();
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                var result = repository.Select(prop => prop.Code == entity.Code, $"{nameof(Classroom.Students)}");

                Assert.NotNull(result);
                Assert.NotEmpty(result.Students);
                Assert.Equal(entity.Students.Count, result.Students.Count);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Select_MatchesPrimaryKey_SafetyCheck(Provider provider)
        {
            var options = Options("Select_MatchesPrimaryKey_SafetyCheck", provider);

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                Assert.Throws<ArgumentNullException>(() => repository.Select(keyValue: null));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory, true)]
        [InlineData(Provider.InMemory, false)]
        [InlineData(Provider.Sqlite, true)]
        [InlineData(Provider.Sqlite, false)]
        public void Select_MatchesPrimaryKey_ShouldPass(Provider provider, bool asTracking)
        {
            var options = OptionsWithData("Select_MatchesPrimaryKey_ShouldPass", provider);
            Classroom entity;

            using (var context = new TestDbContext(options))
            {
                entity = context.Set<Classroom>().First();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                if (!asTracking)
                    repository.AsNoTracking();

                var result = repository.Select(entity.Id);

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
        public void Insert_Entity_SafetyCheck(Provider provider)
        {
            var options = Options("Insert_Entity_SafetyCheck", provider);
            Classroom entity = default;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                Assert.Throws<ArgumentNullException>(() => repository.Insert(entity));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Insert_Entity_ShouldPass(Provider provider)
        {
            var options = Options("Insert_Entity_ShouldPass", provider);
            Classroom entity = FakeEntityGenerator.Classrooms.First();

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                repository.Insert(entity);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var result = repository.Select(entity.Id);

                Assert.NotNull(result);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Insert_Entities_SafetyCheck(Provider provider)
        {
            var options = Options("Insert_Entities_SafetyCheck", provider);
            IEnumerable<Classroom> entities = default;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                Assert.Throws<ArgumentNullException>(() => repository.Insert(entities));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Insert_Entities_ShouldPass(Provider provider)
        {
            var options = Options("Insert_Entities_ShouldPass", provider);
            IEnumerable<Classroom> entities = FakeEntityGenerator.Classrooms;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                repository.Insert(entities);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var result = repository.SelectList();

                Assert.NotEmpty(result);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Update_Entity_SafetyCheck(Provider provider)
        {
            var options = OptionsWithData("Update_Entity_SafetyCheck", provider);
            Classroom entity = default;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                Assert.Throws<ArgumentNullException>(() => repository.Update(entity));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Update_Entity_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("Update_Entity_ShouldPass", provider);
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
                repository.Update(entity);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<Classroom>().First();

                Assert.Equal(modified, result.Code);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Update_Entities_SafetyCheck(Provider provider)
        {
            var options = OptionsWithData("Update_Entities_SafetyCheck", provider);
            IEnumerable<Classroom> entities = default;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                Assert.Throws<ArgumentNullException>(() => repository.Update(entities));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Update_Entities_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("Update_Entities_ShouldPass", provider);
            IEnumerable<Classroom> entities;
            string modified = "Modified";

            using (var context = new TestDbContext(options))
            {
                entities = context.Set<Classroom>().ToList();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                foreach (var entity in entities)
                {
                    entity.Code = modified;
                }
                repository.Update(entities);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<Classroom>().ToList();

                Assert.All(result, p => Assert.Equal(modified, p.Code));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Delete_Entity_SafetyCheck(Provider provider)
        {
            var options = OptionsWithData("Delete_Entity_SafetyCheck", provider);
            Classroom entity = default;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                Assert.Throws<ArgumentNullException>(() => repository.Delete(entity));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Delete_Entity_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("Delete_Entity_ShouldPass", provider);
            Classroom entity;

            using (var context = new TestDbContext(options))
            {
                entity = context.Set<Classroom>().First();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                repository.Delete(entity);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<Classroom>().Find(entity.Id);

                Assert.Null(result);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Delete_Entity_ByPrimaryKey_SafetyCheck(Provider provider)
        {
            var options = OptionsWithData("Delete_Entity_ByPrimaryKey_SafetyCheck", provider);

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                Assert.Throws<ArgumentNullException>(() => repository.Delete(keyValue: null));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Delete_Entity_ByPrimaryKey_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("Delete_Entity_ByPrimaryKey_ShouldPass", provider);
            int keyValue;

            using (var context = new TestDbContext(options))
            {
                var entity = context.Set<Classroom>().First();
                keyValue = entity.Id;
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                repository.Delete(keyValue);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<Classroom>().Find(keyValue);

                Assert.Null(result);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Delete_Entities_SafetyCheck(Provider provider)
        {
            var options = OptionsWithData("Delete_Entities_SafetyCheck", provider);
            IEnumerable<Classroom> entities = default;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);

                Assert.Throws<ArgumentNullException>(() => repository.Delete(entities));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Delete_Entities_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("Delete_Entities_ShouldPass", provider);
            IEnumerable<Classroom> entities = default;

            using (var context = new TestDbContext(options))
            {
                entities = context.Set<Classroom>().ToList();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                repository.Delete(entities);
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<Classroom>().ToList();

                Assert.Empty(result);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void SaveChanges_AsTracking_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SaveChanges_AsTracking_ShouldPass", provider);
            Classroom entity;
            string modified = "Modified";

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                entity = context.Set<Classroom>().First();
                entity.Code = modified;
                repository.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<Classroom>().First();

                Assert.Equal(modified, result.Code);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void SaveChanges_AsNoTracking_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SaveChanges_AsNoTracking_ShouldPass", provider);
            Classroom entity;
            string modified = "Modified";

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                repository.AsNoTracking();
                entity = context.Set<Classroom>().First();
                entity.Code = modified;
                repository.Update(entity);
                repository.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<Classroom>().First();

                Assert.Equal(modified, result.Code);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void SaveChanges_AsNoTracking_Disconnected_ShouldPass(Provider provider)
        {
            var options = OptionsWithData("SaveChanges_AsNoTracking_Disconnected_ShouldPass", provider);
            Classroom entity;
            string modified = "Modified";

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                entity = context.Set<Classroom>().First();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                repository.AsNoTracking();
                entity.Code = modified;
                repository.Update(entity);
                repository.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<Classroom>().First();

                Assert.Equal(modified, result.Code);
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void SaveChanges_AsNoTracking_ShouldNotSaved(Provider provider)
        {
            var options = OptionsWithData("SaveChanges_AsNoTracking_ShouldNotSaved", provider);
            Classroom entity;
            string modified = "Modified";

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                repository.AsNoTracking();
                entity = context.Set<Classroom>().First();
                entity.Code = modified;
                repository.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<Classroom>().First();

                Assert.NotEqual(modified, result.Code);
            }
        }

        [Theory]
        //[InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void SaveChanges_ShouldRollbackEntityChanges(Provider provider)
        {
            var options = OptionsWithData("SaveChanges_ShouldRollbackEntityChanges", provider);
            Classroom entity;
            string modifiedLinq = "Modified By Linq";
            string modifiedSqlCommand = "Modified By SqlCommand";

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                entity = context.Set<Classroom>().First();
                entity.Code = modifiedLinq;

                // Change the person's name in the database to simulate a concurrency conflict
                context.ExecuteSqlCommand("UPDATE [Classroom] SET [Code] = {0} WHERE Id = {1}", false, null, modifiedSqlCommand, entity.Id);

                Assert.Throws<DbUpdateConcurrencyException>(() => repository.SaveChanges());
            }

            using (var context = new TestDbContext(options))
            {
                var result = context.Set<Classroom>().First();

                Assert.NotEqual(modifiedLinq, result.Code);
                Assert.Equal(modifiedSqlCommand, result.Code);
            }
        }
    }
}
