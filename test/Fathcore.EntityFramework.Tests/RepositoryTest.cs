using System.Collections.Generic;
using System.Linq;
using Fathcore.EntityFramework.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Fathcore.EntityFramework.Tests
{
    public class RepositoryTest
    {
        #region Utilities

        private static readonly Dictionary<string, DbContextOptions<TestDbContext>> s_optionsDict = new Dictionary<string, DbContextOptions<TestDbContext>>();
        private static readonly List<TestEntity> s_testEntities = new TestEntity().GenerateData().ToList();
        private static readonly List<TestEntity> s_testEntitiesWithoutChildren = new TestEntity().GenerateDataWithoutChildren().ToList();

        private DbContextOptions<TestDbContext> Options(string name) => new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: name)
            .Options;

        private DbContextOptions<TestDbContext> OptionsWithData(string name)
        {
            if (s_optionsDict.TryGetValue(name, out var value))
                return value;

            var options = Options(name);
            s_optionsDict.Add(name, options);

            using (var context = new TestDbContext(options))
            {
                context.AddRange(s_testEntities);
                context.SaveChanges();
            }

            return options;
        }

        #endregion

        #region Synchronous

        #region Select All
        [Fact]
        public void Should_Select_All_Entities_Default_AsTracking()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Select_All_Entities_Default_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.SelectList();

                Assert.Equal(s_testEntities.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<TestEntity>().Local, e => e.Id == prop.Id));
            }
        }

        [Fact]
        public void Should_Select_All_Entities_AsTracking()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Select_All_Entities_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsTracking().SelectList();

                Assert.Equal(s_testEntities.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<TestEntity>().Local, e => e.Id == prop.Id));
            }
        }

        [Fact]
        public void Should_Select_All_Entities_AsNoTracking()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Select_All_Entities_AsNoTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsNoTracking().SelectList();

                Assert.Equal(s_testEntities.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.DoesNotContain(context.Set<TestEntity>().Local, e => e.Id == prop.Id));
            }
        }

        [Fact]
        public void Should_Select_All_Entities_With_Lambda_Navigation_Default_AsTracking()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Select_All_Entities_With_Lambda_Navigation_Default_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.SelectList(prop => prop.ChildTestEntities);

                Assert.Equal(s_testEntities.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<TestEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(s_testEntities.Single(p => p.TestField == prop.TestField).ChildTestEntities.Count, prop.ChildTestEntities.Count));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.Contains(context.Set<ChildTestEntity>().Local, e => e.Id == child.Id)));
            }
        }

        [Fact]
        public void Should_Select_All_Entities_With_Lambda_Navigation_AsTracking()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Select_All_Entities_With_Lambda_Navigation_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsTracking().SelectList(prop => prop.ChildTestEntities);

                Assert.Equal(s_testEntities.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<TestEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(s_testEntities.Single(p => p.TestField == prop.TestField).ChildTestEntities.Count, prop.ChildTestEntities.Count));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.Contains(context.Set<ChildTestEntity>().Local, e => e.Id == child.Id)));
            }
        }

        [Fact]
        public void Should_Select_All_Entities_With_Lambda_Navigation_AsNoTracking()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Select_All_Entities_With_Lambda_Navigation_AsNoTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsNoTracking().SelectList(prop => prop.ChildTestEntities);

                Assert.Equal(s_testEntities.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.DoesNotContain(context.Set<TestEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(s_testEntities.Single(p => p.TestField == prop.TestField).ChildTestEntities.Count, prop.ChildTestEntities.Count));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.DoesNotContain(context.Set<ChildTestEntity>().Local, e => e.Id == prop.Id)));
            }
        }

        [Fact]
        public void Should_Select_All_Entities_With_String_Navigation_Default_AsTracking()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Select_All_Entities_With_String_Navigation_Default_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.SelectList(nameof(TestEntity.ChildTestEntities));

                Assert.Equal(s_testEntities.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<TestEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(s_testEntities.Single(p => p.TestField == prop.TestField).ChildTestEntities.Count, prop.ChildTestEntities.Count));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.Contains(context.Set<ChildTestEntity>().Local, e => e.Id == child.Id)));
            }
        }

        [Fact]
        public void Should_Select_All_Entities_With_String_Navigation_AsTracking()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Select_All_Entities_With_String_Navigation_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsTracking().SelectList(nameof(TestEntity.ChildTestEntities));

                Assert.Equal(s_testEntities.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<TestEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(s_testEntities.Single(p => p.TestField == prop.TestField).ChildTestEntities.Count, prop.ChildTestEntities.Count));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.Contains(context.Set<ChildTestEntity>().Local, e => e.Id == child.Id)));
            }
        }

        [Fact]
        public void Should_Select_All_Entities_With_String_Navigation_AsNoTracking()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Select_All_Entities_With_String_Navigation_AsNoTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsNoTracking().SelectList(nameof(TestEntity.ChildTestEntities));

                Assert.Equal(s_testEntities.Count, result.Count());
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.DoesNotContain(context.Set<TestEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(s_testEntities.Single(p => p.TestField == prop.TestField).ChildTestEntities.Count, prop.ChildTestEntities.Count));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.DoesNotContain(context.Set<ChildTestEntity>().Local, e => e.Id == prop.Id)));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_All_Entities_Matches_Predicate_Default_AsTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_All_Entities_Matches_Predicate_Default_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.SelectList(prop => prop.TestField == entityToSelect.TestField);

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<TestEntity>().Local, e => e.Id == prop.Id));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_All_Entities_Matches_Predicate_AsTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_All_Entities_Matches_Predicate_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsTracking().SelectList(prop => prop.TestField == entityToSelect.TestField);

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<TestEntity>().Local, e => e.Id == prop.Id));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_All_Entities_Matches_Predicate_AsNoTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_All_Entities_Matches_Predicate_AsNoTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsNoTracking().SelectList(prop => prop.TestField == entityToSelect.TestField);

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.DoesNotContain(context.Set<TestEntity>().Local, e => e.Id == prop.Id));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_All_Entities_Matches_Predicate_With_Lambda_Navigation_Default_AsTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_All_Entities_Matches_Predicate_With_Lambda_Navigation_Default_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.SelectList(prop => prop.TestField == entityToSelect.TestField, prop => prop.ChildTestEntities);

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<TestEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(entityToSelect.ChildTestEntities.Count, prop.ChildTestEntities.Count));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.Contains(context.Set<ChildTestEntity>().Local, e => e.Id == child.Id)));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_All_Entities_Matches_Predicate_With_Lambda_Navigation_AsTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_All_Entities_Matches_Predicate_With_Lambda_Navigation_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsTracking().SelectList(prop => prop.TestField == entityToSelect.TestField, prop => prop.ChildTestEntities);

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<TestEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(entityToSelect.ChildTestEntities.Count, prop.ChildTestEntities.Count));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.Contains(context.Set<ChildTestEntity>().Local, e => e.Id == child.Id)));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_All_Entities_Matches_Predicate_With_Lambda_Navigation_AsNoTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_All_Entities_Matches_Predicate_With_Lambda_Navigation_AsNoTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsNoTracking().SelectList(prop => prop.TestField == entityToSelect.TestField, prop => prop.ChildTestEntities);

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.DoesNotContain(context.Set<TestEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(entityToSelect.ChildTestEntities.Count, prop.ChildTestEntities.Count));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.DoesNotContain(context.Set<ChildTestEntity>().Local, e => e.Id == child.Id)));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_All_Entities_Matches_Predicate_With_String_Navigation_Default_AsTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_All_Entities_Matches_Predicate_With_String_Navigation_Default_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.SelectList(prop => prop.TestField == entityToSelect.TestField, nameof(TestEntity.ChildTestEntities));

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<TestEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(entityToSelect.ChildTestEntities.Count, prop.ChildTestEntities.Count));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.Contains(context.Set<ChildTestEntity>().Local, e => e.Id == child.Id)));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_All_Entities_Matches_Predicate_With_String_Navigation_AsTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_All_Entities_Matches_Predicate_With_String_Navigation_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsTracking().SelectList(prop => prop.TestField == entityToSelect.TestField, nameof(TestEntity.ChildTestEntities));

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.Contains(context.Set<TestEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(entityToSelect.ChildTestEntities.Count, prop.ChildTestEntities.Count));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.Contains(context.Set<ChildTestEntity>().Local, e => e.Id == child.Id)));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_All_Entities_Matches_Predicate_With_String_Navigation_AsNoTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_All_Entities_Matches_Predicate_With_String_Navigation_AsNoTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsNoTracking().SelectList(prop => prop.TestField == entityToSelect.TestField, nameof(TestEntity.ChildTestEntities));

                Assert.Single(result);
                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.All(result, prop => Assert.DoesNotContain(context.Set<TestEntity>().Local, e => e.Id == prop.Id));

                Assert.All(result, prop => Assert.Equal(entityToSelect.ChildTestEntities.Count, prop.ChildTestEntities.Count));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.True(child.Id > 0)));
                Assert.All(result, prop => Assert.All(prop.ChildTestEntities, child => Assert.DoesNotContain(context.Set<ChildTestEntity>().Local, e => e.Id == child.Id)));
            }
        }

        #endregion

        #region Select

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_Predicate_Default_AsTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_Entity_Matches_Predicate_Default_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.Select(prop => prop.TestField == entityToSelect.TestField);

                Assert.True(result.Id > 0);
                Assert.Contains(context.Set<TestEntity>().Local, e => e.Id == result.Id);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_Predicate_AsTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_Entity_Matches_Predicate_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsTracking().Select(prop => prop.TestField == entityToSelect.TestField);

                Assert.True(result.Id > 0);
                Assert.Contains(context.Set<TestEntity>().Local, e => e.Id == result.Id);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_Predicate_AsNoTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_Entity_Matches_Predicate_AsNoTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsNoTracking().Select(prop => prop.TestField == entityToSelect.TestField);

                Assert.True(result.Id > 0);
                Assert.DoesNotContain(context.Set<TestEntity>().Local, e => e.Id == result.Id);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_Predicate_With_Lambda_Navigation_Default_AsTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_Entity_Matches_Predicate_With_Lambda_Navigation_Default_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.Select(prop => prop.TestField == entityToSelect.TestField, prop => prop.ChildTestEntities);

                Assert.True(result.Id > 0);
                Assert.Contains(context.Set<TestEntity>().Local, e => e.Id == result.Id);

                Assert.Equal(entityToSelect.ChildTestEntities.Count, result.ChildTestEntities.Count);
                Assert.All(result.ChildTestEntities, child => Assert.True(child.Id > 0));
                Assert.All(result.ChildTestEntities, child => Assert.Contains(context.Set<ChildTestEntity>().Local, e => e.Id == child.Id));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_Predicate_With_Lambda_Navigation_AsTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_Entity_Matches_Predicate_With_Lambda_Navigation_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsTracking().Select(prop => prop.TestField == entityToSelect.TestField, prop => prop.ChildTestEntities);

                Assert.True(result.Id > 0);
                Assert.Contains(context.Set<TestEntity>().Local, e => e.Id == result.Id);

                Assert.Equal(entityToSelect.ChildTestEntities.Count, result.ChildTestEntities.Count);
                Assert.All(result.ChildTestEntities, child => Assert.True(child.Id > 0));
                Assert.All(result.ChildTestEntities, child => Assert.Contains(context.Set<ChildTestEntity>().Local, e => e.Id == child.Id));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_Predicate_With_Lambda_Navigation_AsNoTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_Entity_Matches_Predicate_With_Lambda_Navigation_AsNoTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsNoTracking().Select(prop => prop.TestField == entityToSelect.TestField, prop => prop.ChildTestEntities);

                Assert.True(result.Id > 0);
                Assert.DoesNotContain(context.Set<TestEntity>().Local, e => e.Id == result.Id);

                Assert.Equal(entityToSelect.ChildTestEntities.Count, result.ChildTestEntities.Count);
                Assert.All(result.ChildTestEntities, child => Assert.True(child.Id > 0));
                Assert.All(result.ChildTestEntities, child => Assert.DoesNotContain(context.Set<ChildTestEntity>().Local, e => e.Id == child.Id));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_Predicate_With_String_Navigation_Default_AsTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_Entity_Matches_Predicate_With_String_Navigation_Default_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.Select(prop => prop.TestField == entityToSelect.TestField, nameof(TestEntity.ChildTestEntities));

                Assert.True(result.Id > 0);
                Assert.Contains(context.Set<TestEntity>().Local, e => e.Id == result.Id);

                Assert.Equal(entityToSelect.ChildTestEntities.Count, result.ChildTestEntities.Count);
                Assert.All(result.ChildTestEntities, child => Assert.True(child.Id > 0));
                Assert.All(result.ChildTestEntities, child => Assert.Contains(context.Set<ChildTestEntity>().Local, e => e.Id == child.Id));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_Predicate_With_String_Navigation_AsTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_Entity_Matches_Predicate_With_String_Navigation_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsTracking().Select(prop => prop.TestField == entityToSelect.TestField, nameof(TestEntity.ChildTestEntities));

                Assert.True(result.Id > 0);
                Assert.Contains(context.Set<TestEntity>().Local, e => e.Id == result.Id);

                Assert.Equal(entityToSelect.ChildTestEntities.Count, result.ChildTestEntities.Count);
                Assert.All(result.ChildTestEntities, child => Assert.True(child.Id > 0));
                Assert.All(result.ChildTestEntities, child => Assert.Contains(context.Set<ChildTestEntity>().Local, e => e.Id == child.Id));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_Predicate_With_String_Navigation_AsNoTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_Entity_Matches_Predicate_With_String_Navigation_AsNoTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsNoTracking().Select(prop => prop.TestField == entityToSelect.TestField, nameof(TestEntity.ChildTestEntities));

                Assert.True(result.Id > 0);
                Assert.DoesNotContain(context.Set<TestEntity>().Local, e => e.Id == result.Id);

                Assert.Equal(entityToSelect.ChildTestEntities.Count, result.ChildTestEntities.Count);
                Assert.All(result.ChildTestEntities, child => Assert.True(child.Id > 0));
                Assert.All(result.ChildTestEntities, child => Assert.DoesNotContain(context.Set<ChildTestEntity>().Local, e => e.Id == child.Id));
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_PrimaryKey_Default_AsTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_Entity_Matches_PrimaryKey_Default_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                entityToSelect = repository.Select(prop => prop.TestField == entityToSelect.TestField);
            }

            using (var context = new TestDbContext(OptionsWithData("Should_Select_Entity_Matches_PrimaryKey_Default_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.Select(entityToSelect.Id);

                Assert.True(result.Id > 0);
                Assert.Contains(context.Set<TestEntity>().Local, e => e.Id == result.Id);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_PrimaryKey_AsTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_Entity_Matches_PrimaryKey_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                entityToSelect = repository.AsTracking().Select(prop => prop.TestField == entityToSelect.TestField);
            }

            using (var context = new TestDbContext(OptionsWithData("Should_Select_Entity_Matches_PrimaryKey_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsTracking().Select(entityToSelect.Id);

                Assert.True(result.Id > 0);
                Assert.Contains(context.Set<TestEntity>().Local, e => e.Id == result.Id);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Select_Entity_Matches_PrimaryKey_AsNoTracking(int index)
        {
            TestEntity entityToSelect = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Select_Entity_Matches_PrimaryKey_AsNoTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                entityToSelect = repository.AsNoTracking().Select(prop => prop.TestField == entityToSelect.TestField);
            }

            using (var context = new TestDbContext(OptionsWithData("Should_Select_Entity_Matches_PrimaryKey_AsNoTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsNoTracking().Select(entityToSelect.Id);

                Assert.True(result.Id > 0);
                Assert.DoesNotContain(context.Set<TestEntity>().Local, e => e.Id == result.Id);
            }
        }

        #endregion

        #region Insert

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Insert_Entity(int index)
        {
            TestEntity entityToInsert = s_testEntitiesWithoutChildren[index];

            using (var context = new TestDbContext(Options("Should_Insert_Entity")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.Insert(entityToInsert);
                var saveCount = repository.SaveChanges();

                Assert.True(result.Id > 0);
                Assert.Equal(1, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Insert_Entity_With_Navigation(int index)
        {
            TestEntity entityToInsert = s_testEntities[index];

            using (var context = new TestDbContext(Options("Should_Insert_Entity_With_Navigation")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.Insert(entityToInsert);
                var saveCount = repository.SaveChanges();

                Assert.True(result.Id > 0);
                Assert.Equal(1 + entityToInsert.ChildTestEntities.Count, saveCount);
            }
        }

        [Fact]
        public void Should_Insert_Entities()
        {
            using (var context = new TestDbContext(Options("Should_Insert_Entities")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.Insert(s_testEntitiesWithoutChildren);
                var saveCount = repository.SaveChanges();

                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.Equal(s_testEntitiesWithoutChildren.Count, saveCount);
            }
        }

        [Fact]
        public void Should_Insert_Entities_With_Navigation()
        {
            using (var context = new TestDbContext(Options("Should_Insert_Entities_With_Navigation")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.Insert(s_testEntities);
                var saveCount = repository.SaveChanges();

                Assert.All(result, prop => Assert.True(prop.Id > 0));
                Assert.Equal(s_testEntities.Count + s_testEntities.Sum(prop => prop.ChildTestEntities.Count), saveCount);
            }
        }

        #endregion

        #region Update

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_Default_AsTracking(int index)
        {
            TestEntity entityToUpdate = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entity_Default_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.Select(prop => prop.TestField == entityToUpdate.TestField);
                result.TestField = "Modified";
                var saveCount = repository.SaveChanges();

                Assert.Equal(1, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_AsTracking(int index)
        {
            TestEntity entityToUpdate = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entity_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsTracking().Select(prop => prop.TestField == entityToUpdate.TestField);
                result.TestField = "Modified";
                var saveCount = repository.SaveChanges();

                Assert.Equal(1, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_AsNoTracking(int index)
        {
            TestEntity entityToUpdate = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entity_AsNoTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsNoTracking().Select(prop => prop.TestField == entityToUpdate.TestField);
                result.TestField = "Modified";
                var saveCount = repository.SaveChanges();

                Assert.Equal(0, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_Default_AsTracking_Using_Method(int index)
        {
            TestEntity entityToUpdate = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entity_Default_AsTracking_Using_Method")))
            {
                var repository = new Repository<TestEntity>(context);

                entityToUpdate = repository.Select(prop => prop.TestField == entityToUpdate.TestField);
                entityToUpdate.TestField = "Modified";

                var result = repository.Update(entityToUpdate);
                var saveCount = repository.SaveChanges();

                Assert.Equal(1, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_AsTracking_Using_Method(int index)
        {
            TestEntity entityToUpdate = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entity_AsTracking_Using_Method")))
            {
                var repository = new Repository<TestEntity>(context);

                entityToUpdate = repository.AsTracking().Select(prop => prop.TestField == entityToUpdate.TestField);
                entityToUpdate.TestField = "Modified";

                var result = repository.Update(entityToUpdate);
                var saveCount = repository.SaveChanges();

                Assert.Equal(1, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_AsNoTracking_Using_Method(int index)
        {
            TestEntity entityToUpdate = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entity_AsNoTracking_Using_Method")))
            {
                var repository = new Repository<TestEntity>(context);

                entityToUpdate = repository.AsNoTracking().Select(prop => prop.TestField == entityToUpdate.TestField);
                entityToUpdate.TestField = "Modified";

                var result = repository.Update(entityToUpdate);
                var saveCount = repository.SaveChanges();

                Assert.Equal(1, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_With_Navigation_Default_AsTracking(int index)
        {
            TestEntity entityToUpdate = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entity_With_Navigation_Default_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.Select(prop => prop.TestField == entityToUpdate.TestField, prop => prop.ChildTestEntities);
                result.TestField = "Modified";
                result.ChildTestEntities.First().ChildTestField = "Modified";
                var saveCount = repository.SaveChanges();

                Assert.Equal(2, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_With_Navigation_AsTracking(int index)
        {
            TestEntity entityToUpdate = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entity_With_Navigation_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsTracking().Select(prop => prop.TestField == entityToUpdate.TestField, prop => prop.ChildTestEntities);
                result.TestField = "Modified";
                result.ChildTestEntities.First().ChildTestField = "Modified";
                var saveCount = repository.SaveChanges();

                Assert.Equal(2, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_With_Navigation_AsNoTracking(int index)
        {
            TestEntity entityToUpdate = s_testEntities[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entity_With_Navigation_AsNoTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var result = repository.AsNoTracking().Select(prop => prop.TestField == entityToUpdate.TestField, prop => prop.ChildTestEntities);
                result.TestField = "Modified";
                result.ChildTestEntities.First().ChildTestField = "Modified";
                var saveCount = repository.SaveChanges();

                Assert.Equal(0, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_With_Navigation_Default_AsTracking_Using_Method(int index)
        {
            TestEntity entityToUpdate = s_testEntitiesWithoutChildren[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entity_With_Navigation_Default_AsTracking_Using_Method")))
            {
                var repository = new Repository<TestEntity>(context);

                entityToUpdate = repository.Select(prop => prop.TestField == entityToUpdate.TestField, prop => prop.ChildTestEntities);
                entityToUpdate.TestField = "Modified";
                entityToUpdate.ChildTestEntities.First().ChildTestField = "Modified";

                var result = repository.Update(entityToUpdate);
                var saveCount = repository.SaveChanges();

                Assert.Equal(2, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_With_Navigation_AsTracking_Using_Method(int index)
        {
            TestEntity entityToUpdate = s_testEntitiesWithoutChildren[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entity_With_Navigation_AsTracking_Using_Method")))
            {
                var repository = new Repository<TestEntity>(context);

                entityToUpdate = repository.AsTracking().Select(prop => prop.TestField == entityToUpdate.TestField, prop => prop.ChildTestEntities);
                entityToUpdate.TestField = "Modified";
                entityToUpdate.ChildTestEntities.First().ChildTestField = "Modified";

                var result = repository.Update(entityToUpdate);
                var saveCount = repository.SaveChanges();

                Assert.Equal(2, saveCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Should_Update_Entity_With_Navigation_AsNoTracking_Using_Method(int index)
        {
            TestEntity entityToUpdate = s_testEntitiesWithoutChildren[index];

            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entity_With_Navigation_AsNoTracking_Using_Method")))
            {
                var repository = new Repository<TestEntity>(context);

                entityToUpdate = repository.AsNoTracking().Select(prop => prop.TestField == entityToUpdate.TestField, prop => prop.ChildTestEntities);
                entityToUpdate.TestField = "Modified";
                entityToUpdate.ChildTestEntities.First().ChildTestField = "Modified";

                var result = repository.Update(entityToUpdate);
                var saveCount = repository.SaveChanges();

                Assert.Equal(1 + entityToUpdate.ChildTestEntities.Count, saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_Default_AsTracking()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entities_Default_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var results = repository.SelectList();

                foreach (var result in results)
                    result.TestField = "Modified";

                var saveCount = repository.SaveChanges();

                Assert.Equal(s_testEntities.Count, saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_AsTracking()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entities_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var results = repository.AsTracking().SelectList();

                foreach (var result in results)
                    result.TestField = "Modified";

                var saveCount = repository.SaveChanges();

                Assert.Equal(s_testEntities.Count, saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_AsNoTracking()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entities_AsNoTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var results = repository.AsNoTracking().SelectList();

                foreach (var result in results)
                    result.TestField = "Modified";

                var saveCount = repository.SaveChanges();

                Assert.Equal(0, saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_Default_AsTracking_Using_Method()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entities_Default_AsTracking_Using_Method")))
            {
                var repository = new Repository<TestEntity>(context);

                var results = repository.SelectList();

                foreach (var result in results)
                    result.TestField = "Modified";

                repository.Update(results);
                var saveCount = repository.SaveChanges();

                Assert.Equal(s_testEntities.Count, saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_AsTracking_Using_Method()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entities_AsTracking_Using_Method")))
            {
                var repository = new Repository<TestEntity>(context);

                var results = repository.AsTracking().SelectList();

                foreach (var result in results)
                    result.TestField = "Modified";

                repository.Update(results);
                var saveCount = repository.SaveChanges();

                Assert.Equal(s_testEntities.Count, saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_AsNoTracking_Using_Method()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entities_AsNoTracking_Using_Method")))
            {
                var repository = new Repository<TestEntity>(context);

                var results = repository.AsNoTracking().SelectList();

                foreach (var result in results)
                    result.TestField = "Modified";

                repository.Update(results);
                var saveCount = repository.SaveChanges();

                Assert.Equal(s_testEntities.Count, saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_With_Navigation_Default_AsTracking()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entities_With_Navigation_Default_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var results = repository.SelectList(prop => prop.ChildTestEntities);

                foreach (var result in results)
                {
                    result.TestField = "Modified";
                    if (result.ChildTestEntities.Any())
                        result.ChildTestEntities.First().ChildTestField = "Modified";
                }

                var saveCount = repository.SaveChanges();

                Assert.Equal(s_testEntities.Count + s_testEntities.Count(p => p.ChildTestEntities.Any()), saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_With_Navigation_AsTracking()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entities_With_Navigation_AsTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var results = repository.AsTracking().SelectList(prop => prop.ChildTestEntities);

                foreach (var result in results)
                {
                    result.TestField = "Modified";
                    if (result.ChildTestEntities.Any())
                        result.ChildTestEntities.First().ChildTestField = "Modified";
                }

                var saveCount = repository.SaveChanges();

                Assert.Equal(s_testEntities.Count + s_testEntities.Count(p => p.ChildTestEntities.Any()), saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_With_Navigation_AsNoTracking()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entities_With_Navigation_AsNoTracking")))
            {
                var repository = new Repository<TestEntity>(context);

                var results = repository.AsNoTracking().SelectList(prop => prop.ChildTestEntities);

                foreach (var result in results)
                {
                    result.TestField = "Modified";
                    if (result.ChildTestEntities.Any())
                        result.ChildTestEntities.First().ChildTestField = "Modified";
                }

                var saveCount = repository.SaveChanges();

                Assert.Equal(0, saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_With_Navigation_Default_AsTracking_Using_Method()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entities_With_Navigation_Default_AsTracking_Using_Method")))
            {
                var repository = new Repository<TestEntity>(context);

                var results = repository.SelectList(prop => prop.ChildTestEntities);

                foreach (var result in results)
                {
                    result.TestField = "Modified";
                    if (result.ChildTestEntities.Any())
                        result.ChildTestEntities.First().ChildTestField = "Modified";
                }

                repository.Update(results);
                var saveCount = repository.SaveChanges();

                Assert.Equal(s_testEntities.Count + s_testEntities.Count(p => p.ChildTestEntities.Any()), saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_With_Navigation_AsTracking_Using_Method()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entities_With_Navigation_AsTracking_Using_Method")))
            {
                var repository = new Repository<TestEntity>(context);

                var results = repository.AsTracking().SelectList(prop => prop.ChildTestEntities);

                foreach (var result in results)
                {
                    result.TestField = "Modified";
                    if (result.ChildTestEntities.Any())
                        result.ChildTestEntities.First().ChildTestField = "Modified";
                }

                repository.Update(results);
                var saveCount = repository.SaveChanges();

                Assert.Equal(s_testEntities.Count + s_testEntities.Count(p => p.ChildTestEntities.Any()), saveCount);
            }
        }

        [Fact]
        public void Should_Update_Entities_With_Navigation_AsNoTracking_Using_Method()
        {
            using (var context = new TestDbContext(OptionsWithData("Should_Update_Entities_With_Navigation_AsNoTracking_Using_Method")))
            {
                var repository = new Repository<TestEntity>(context);

                var results = repository.AsNoTracking().SelectList(prop => prop.ChildTestEntities);

                foreach (var result in results)
                {
                    result.TestField = "Modified";
                    if (result.ChildTestEntities.Any())
                        result.ChildTestEntities.First().ChildTestField = "Modified";
                }

                repository.Update(results);
                var saveCount = repository.SaveChanges();

                Assert.Equal(s_testEntities.Count + s_testEntities.Sum(p => p.ChildTestEntities.Count), saveCount);
            }
        }

        #endregion

        #region Delete

        #endregion

        #endregion

        #region Asynchronous
        #endregion
    }
}
