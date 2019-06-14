using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fathcore.EntityFramework.Tests.Fakes;
using Fathcore.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace Fathcore.EntityFramework.Tests
{
    public class CachedRepositoryTest
    {
        private ICacheSetting _cacheSetting;
        public ICacheSetting CacheSetting
        {
            get
            {
                if (_cacheSetting == null)
                {
                    var mockCache = new Mock<ICacheSetting>();
                    mockCache.Setup(p => p.CachePrefix).Returns("cachedrepositorytest.");
                    mockCache.Setup(p => p.CacheTime).Returns(1440);
                    _cacheSetting = mockCache.Object;
                }

                return _cacheSetting;
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void SelectList_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("CachedRepository_SelectList_ShouldPass", provider);
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                var timingResult = GetTiming(
                    () => cachedRepository.SelectList(),
                    () => cachedRepository.SelectList()
                );

                Assert.True(timingResult.First() > timingResult.Last());
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void SelectList_WithLambda_NavigationProperty_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("CachedRepository_SelectList_WithLambda_NavigationProperty_ShouldPass", provider);
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                var timingResult = GetTiming(
                    () => cachedRepository.SelectList(prop => prop.Students),
                    () => cachedRepository.SelectList(prop => prop.Students)
                );

                Assert.True(timingResult.First() > timingResult.Last());
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void SelectList_WithString_NavigationProperty_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("CachedRepository_SelectList_WithString_NavigationProperty_ShouldPass", provider);
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                var timingResult = GetTiming(
                    () => cachedRepository.SelectList($"{nameof(Classroom.Students)}"),
                    () => cachedRepository.SelectList($"{nameof(Classroom.Students)}")
                );

                Assert.True(timingResult.First() > timingResult.Last());
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void SelectList_MatchesPredicate_WithoutNavigationProperty_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("CachedRepository_SelectList_MatchesPredicate_WithoutNavigationProperty_ShouldPass", provider);
            var entity = FakeEntityGenerator.Classrooms.First();
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                var timingResult = GetTiming(
                    () => cachedRepository.SelectList(prop => prop.Code == entity.Code),
                    () => cachedRepository.SelectList(prop => prop.Code == entity.Code)
                );

                Assert.True(timingResult.First() > timingResult.Last());
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void SelectList_MatchesPredicate_WithLambda_NavigationProperty_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("CachedRepository_SelectList_MatchesPredicate_WithLambda_NavigationProperty_ShouldPass", provider);
            var entity = FakeEntityGenerator.Classrooms.First();
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                var timingResult = GetTiming(
                    () => cachedRepository.SelectList(prop => prop.Code == entity.Code, prop => prop.Students),
                    () => cachedRepository.SelectList(prop => prop.Code == entity.Code, prop => prop.Students)
                );

                Assert.True(timingResult.First() > timingResult.Last());
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void SelectList_MatchesPredicate_WithString_NavigationProperty_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("CachedRepository_SelectList_MatchesPredicate_WithString_NavigationProperty_ShouldPass", provider);
            var entity = FakeEntityGenerator.Classrooms.First();
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                var timingResult = GetTiming(
                    () => cachedRepository.SelectList(prop => prop.Code == entity.Code, $"{nameof(Classroom.Students)}"),
                    () => cachedRepository.SelectList(prop => prop.Code == entity.Code, $"{nameof(Classroom.Students)}")
                );

                Assert.True(timingResult.First() > timingResult.Last());
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Select_MatchesPredicate_WithoutNavigationProperty_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("CachedRepository_Select_MatchesPredicate_WithoutNavigationProperty_ShouldPass", provider);
            var entity = FakeEntityGenerator.Classrooms.First();
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                var timingResult = GetTiming(
                    () => cachedRepository.Select(prop => prop.Code == entity.Code),
                    () => cachedRepository.Select(prop => prop.Code == entity.Code)
                );

                Assert.True(timingResult.First() > timingResult.Last());
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Select_MatchesPredicate_WithLambda_NavigationProperty_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("CachedRepository_Select_MatchesPredicate_WithLambda_NavigationProperty_ShouldPass", provider);
            var entity = FakeEntityGenerator.Classrooms.First();
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                var timingResult = GetTiming(
                    () => cachedRepository.Select(prop => prop.Code == entity.Code, prop => prop.Students),
                    () => cachedRepository.Select(prop => prop.Code == entity.Code, prop => prop.Students)
                );

                Assert.True(timingResult.First() > timingResult.Last());
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Select_MatchesPredicate_WithString_NavigationProperty_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("CachedRepository_Select_MatchesPredicate_WithString_NavigationProperty_ShouldPass", provider);
            var entity = FakeEntityGenerator.Classrooms.First();
            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                var timingResult = GetTiming(
                    () => cachedRepository.Select(prop => prop.Code == entity.Code, $"{nameof(Classroom.Students)}"),
                    () => cachedRepository.Select(prop => prop.Code == entity.Code, $"{nameof(Classroom.Students)}")
                );

                Assert.True(timingResult.First() > timingResult.Last());
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Select_MatchesPrimaryKey_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("CachedRepository_Select_MatchesPrimaryKey_ShouldPass", provider);
            Classroom entity;

            using (var context = new TestDbContext(options))
            {
                entity = context.Set<Classroom>().First();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                var timingResult = GetTiming(
                    () => cachedRepository.Select(entity.Id),
                    () => cachedRepository.Select(entity.Id)
                );

                Assert.True(timingResult.First() > timingResult.Last());
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Insert_Entity_SafetyCheck(Provider provider)
        {
            var options = TestHelper.Options("CachedRepository_Insert_Entity_SafetyCheck", provider);
            Classroom entity = default;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                Assert.Throws<ArgumentNullException>(() => cachedRepository.Insert(entity));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Insert_Entity_ShouldPass(Provider provider)
        {
            var options = TestHelper.Options("CachedRepository_Insert_Entity_ShouldPass", provider);
            Classroom entity = FakeEntityGenerator.Classrooms.First();

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);
                cachedRepository.Insert(entity);
                cachedRepository.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                var timingResult = GetTiming(
                    () => cachedRepository.Select(entity.Id),
                    () => cachedRepository.Select(entity.Id)
                );

                Assert.True(timingResult.First() > timingResult.Last());
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Insert_Entities_SafetyCheck(Provider provider)
        {
            var options = TestHelper.Options("CachedRepository_Insert_Entities_SafetyCheck", provider);
            IEnumerable<Classroom> entities = default;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                Assert.Throws<ArgumentNullException>(() => cachedRepository.Insert(entities));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Insert_Entities_ShouldPass(Provider provider)
        {
            var options = TestHelper.Options("CachedRepository_Insert_Entities_ShouldPass", provider);
            IEnumerable<Classroom> entities = FakeEntityGenerator.Classrooms;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);
                cachedRepository.Insert(entities);
                cachedRepository.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                var timingResult = GetTiming(
                    () => cachedRepository.SelectList(),
                    () => cachedRepository.SelectList()
                );

                Assert.True(timingResult.First() > timingResult.Last());
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Update_Entity_SafetyCheck(Provider provider)
        {
            var options = TestHelper.OptionsWithData("CachedRepository_Update_Entity_SafetyCheck", provider);
            Classroom entity = default;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                Assert.Throws<ArgumentNullException>(() => cachedRepository.Update(entity));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Update_Entity_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("CachedRepository_Update_Entity_ShouldPass", provider);
            Classroom entity;
            string modified = "Modified";

            using (var context = new TestDbContext(options))
            {
                entity = context.Set<Classroom>().First();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);
                entity.Code = modified;
                cachedRepository.Update(entity);
                cachedRepository.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                var timingResult = GetTiming(
                    () => cachedRepository.Select(entity.Id),
                    () => cachedRepository.Select(entity.Id)
                );

                Assert.True(timingResult.First() > timingResult.Last());
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Update_Entities_SafetyCheck(Provider provider)
        {
            var options = TestHelper.OptionsWithData("CachedRepository_Update_Entities_SafetyCheck", provider);
            IEnumerable<Classroom> entities = default;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                Assert.Throws<ArgumentNullException>(() => cachedRepository.Update(entities));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Update_Entities_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("CachedRepository_Update_Entities_ShouldPass", provider);
            IEnumerable<Classroom> entities;
            string modified = "Modified";

            using (var context = new TestDbContext(options))
            {
                entities = context.Set<Classroom>().ToList();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);
                foreach (var entity in entities)
                {
                    entity.Code = modified;
                }
                cachedRepository.Update(entities);
                cachedRepository.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                var timingResult = GetTiming(
                    () => cachedRepository.SelectList(),
                    () => cachedRepository.SelectList()
                );

                Assert.True(timingResult.First() > timingResult.Last());
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Delete_Entity_SafetyCheck(Provider provider)
        {
            var options = TestHelper.OptionsWithData("CachedRepository_Delete_Entity_SafetyCheck", provider);
            Classroom entity = default;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                Assert.Throws<ArgumentNullException>(() => cachedRepository.Delete(entity));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Delete_Entity_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("CachedRepository_Delete_Entity_ShouldPass", provider);
            Classroom entity;

            using (var context = new TestDbContext(options))
            {
                entity = context.Set<Classroom>().First();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);
                cachedRepository.Delete(entity);
                cachedRepository.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                var timingResult = GetTiming(
                    () => cachedRepository.Select(entity.Id),
                    () => cachedRepository.Select(entity.Id)
                );

                Assert.True(timingResult.First() > timingResult.Last());
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Delete_Entity_ByPrimaryKey_SafetyCheck(Provider provider)
        {
            var options = TestHelper.OptionsWithData("CachedRepository_Delete_Entity_ByPrimaryKey_SafetyCheck", provider);

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                Assert.Throws<ArgumentNullException>(() => cachedRepository.Delete(keyValue: null));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Delete_Entity_ByPrimaryKey_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("CachedRepository_Delete_Entity_ByPrimaryKey_ShouldPass", provider);
            int keyValue;

            using (var context = new TestDbContext(options))
            {
                var entity = context.Set<Classroom>().First();
                keyValue = entity.Id;
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);
                cachedRepository.Delete(keyValue);
                cachedRepository.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                var timingResult = GetTiming(
                    () => cachedRepository.Select(keyValue),
                    () => cachedRepository.Select(keyValue)
                );

                Assert.True(timingResult.First() > timingResult.Last());
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Delete_Entities_SafetyCheck(Provider provider)
        {
            var options = TestHelper.OptionsWithData("CachedRepository_Delete_Entities_SafetyCheck", provider);
            IEnumerable<Classroom> entities = default;

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                Assert.Throws<ArgumentNullException>(() => cachedRepository.Delete(entities));
            }
        }

        [Theory]
        [InlineData(Provider.InMemory)]
        [InlineData(Provider.Sqlite)]
        public void Delete_Entities_ShouldPass(Provider provider)
        {
            var options = TestHelper.OptionsWithData("CachedRepository_Delete_Entities_ShouldPass", provider);
            IEnumerable<Classroom> entities = default;

            using (var context = new TestDbContext(options))
            {
                entities = context.Set<Classroom>().ToList();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);
                cachedRepository.Delete(entities);
                cachedRepository.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var repository = new Repository<Classroom>(context);
                var cache = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()), CacheSetting);
                var cachedRepository = new CachedRepository<Classroom>(repository, cache);

                var timingResult = GetTiming(
                    () => cachedRepository.SelectList(),
                    () => cachedRepository.SelectList()
                );

                Assert.True(timingResult.First() > timingResult.Last());
            }
        }

        public long[] GetTiming<T>(params Func<T>[] actions)
        {
            var timing = new List<long>();
            var sw = new Stopwatch();

            foreach (var action in actions)
            {
                sw.Start();
                action();
                sw.Stop();
                timing.Add(sw.ElapsedTicks);
                sw.Reset();
            }

            return timing.ToArray();
        }
    }
}
