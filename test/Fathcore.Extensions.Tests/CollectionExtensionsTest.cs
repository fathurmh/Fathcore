using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fathcore.Extensions.Tests
{
    public class CollectionExtensionsTest
    {
        [Theory]
        [InlineData(default(IEnumerable<int>))]
        [InlineData(new[] { 1, 2, 3, 4, 5 })]
        public async Task ForEachAsync_SafetyCheck(IEnumerable<int> numbers)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => numbers.ForEachAsync(default));
        }

        [Theory]
        [InlineData(50)]
        [InlineData(100)]
        public async Task ForEachAsync_Benchmarks(int multiplier)
        {
            var numbers = new[] { 1, 5, 3, 2, 4 };

            var sw = new Stopwatch();
            sw.Start();
            await numbers.ForEachAsync(item => DoWorkAsync(item, multiplier));
            sw.Stop();

            var result = Math.Ceiling((double)sw.ElapsedMilliseconds / multiplier);

            Assert.True(numbers.Max() <= result);
        }

        [Theory]
        [InlineData(50)]
        [InlineData(100)]
        public async Task ForEachAsync_Benchmarks_2(int multiplier)
        {
            var numbers = new[] { 1, 5, 3, 2, 4 };

            var sw = new Stopwatch();
            sw.Start();
            await numbers.ForEachAsync(item => DoWorkAsync(item, multiplier));
            sw.Stop();

            var resultAsync = Math.Ceiling((double)sw.ElapsedMilliseconds / multiplier);

            sw.Restart();
            numbers.ToList().ForEach(item => DoWork(item, multiplier));
            sw.Stop();

            var resultSync = Math.Ceiling((double)sw.ElapsedMilliseconds / multiplier);

            Assert.True(numbers.Max() <= resultAsync);
            Assert.True(resultAsync < numbers.Sum());
            Assert.True(numbers.Sum() <= resultSync);
        }

        [Theory]
        [InlineData(50)]
        [InlineData(100)]
        public void ForEachAsync_Benchmarks_3(int multiplier)
        {
            var numbers = new[] { 1, 5, 3, 2, 4 };

            var sw = new Stopwatch();
            sw.Start();
            Helper.RunSync(() => numbers.ForEachAsync(item => DoWorkAsync(item, multiplier)));
            sw.Stop();

            var resultAsync = Math.Ceiling((double)sw.ElapsedMilliseconds / multiplier);

            sw.Restart();
            numbers.ToList().ForEach(item => DoWork(item, multiplier));
            sw.Stop();

            var resultSync = Math.Ceiling((double)sw.ElapsedMilliseconds / multiplier);

            Assert.True(numbers.Max() <= resultAsync);
            Assert.True(resultAsync < numbers.Sum());
            Assert.True(numbers.Sum() <= resultSync);
        }

        [Theory]
        [MemberData(nameof(FlattenList_SafetyCheck_Data))]
        public void FlattenList_SafetyCheck(IEnumerable<Menu> source)
        {
            Assert.Throws<ArgumentNullException>(() => source.FlattenList(default));
        }

        [Fact]
        public void FlattenList_Should_FlattenTree()
        {
            var menus = new List<Menu>
            {
                new Menu
                {
                    Id = 1, Name = "Parent Name", Children = new List<Menu>
                    {
                        new Menu { Id = 2, Name = "Child Name", ParentId = 1 }
                    }
                }
            };

            var result = menus.FlattenList(p => p.Children);

            Assert.Equal(2, result.Count());
        }

        [Theory]
        [MemberData(nameof(FlattenList_SafetyCheck_Data))]
        public void UnFlattenList_SafetyCheck(IEnumerable<Menu> source)
        {
            Assert.Throws<ArgumentNullException>(() => source.UnFlattenList<Menu, int>(default, default, default));
            Assert.Throws<ArgumentNullException>(() => source.UnFlattenList(p => p.Id, default, default));
            Assert.Throws<ArgumentNullException>(() => source.UnFlattenList(p => p.Id, p => p.ParentId, default));
        }

        [Fact]
        public void UnFlattenList_Should_GenerateTree()
        {
            var menus = new List<Menu>
            {
                new Menu
                {
                    Id = 1, Name = "Parent Name", Children = new List<Menu>
                    {
                        new Menu { Id = 2, Name = "Child Name", ParentId = 1 }
                    }
                }
            };

            var flatten = menus.FlattenList(p => p.Children);
            var result = flatten.UnFlattenList(p => p.Id, p => p.ParentId, p => p.Children);

            Assert.Single(result);
            Assert.Single(result.Single().Children);
        }

        [Fact]
        public void UnFlattenList_Enumerator_Should_Success()
        {
            var menus = new List<Menu>
            {
                new Menu
                {
                    Id = 1, Name = "Parent Name", Children = new List<Menu>
                    {
                        new Menu { Id = 2, Name = "Child Name", ParentId = 1 }
                    }
                }
            };

            var flatten = menus.FlattenList(p => p.Children);
            var result = flatten.UnFlattenList(p => p.Id, p => p.ParentId, p => p.Children);

            var enumerator = result.GetEnumerator();

            var type = enumerator.GetType();
            var fieldInfo = type.GetField("<>1__state", BindingFlags.Instance | BindingFlags.NonPublic);
            fieldInfo.SetValue(enumerator, -1);

            enumerator.MoveNext();

            Assert.Null(enumerator.Current);
        }

        private Task DoWorkAsync(int number, int multiplier = 1)
        {
            return Task.Delay(number * multiplier);
        }

        private void DoWork(int number, int multiplier = 1)
        {
            Thread.Sleep(number * multiplier);
        }

        public class Menu
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int? ParentId { get; set; }
            public ICollection<Menu> Children { get; set; }
            public static IEnumerable<Menu> Generate()
            {
                var menus = new List<Menu>
                {
                    new Menu
                    {
                        Id = 1, Name = "Parent Name", Children = new List<Menu>
                        {
                            new Menu { Id = 2, Name = "Child Name", ParentId = 1 }
                        }
                    }
                };

                return menus;
            }
        }

        public static IEnumerable<object[]> FlattenList_SafetyCheck_Data() =>
            new List<object[]>
            {
                new object[] { default(IEnumerable<Menu>) },
                new object[] { Menu.Generate() }
            };
    }
}
