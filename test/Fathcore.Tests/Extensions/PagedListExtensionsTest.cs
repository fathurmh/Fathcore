using System;
using System.Collections.Generic;
using System.Linq;
using Fathcore.Extensions;
using Xunit;

namespace Fathcore.Tests.Extensions
{
    public class CollectionExtensionsTest
    {
        [Theory]
        [InlineData(95, 0, 10, false, true)]
        [InlineData(95, 9, 10, true, false)]
        [InlineData(95, 8, 10, true, true)]
        [InlineData(95, 23, 4, true, false)]
        [InlineData(95, 24, 4, true, false)]
        public void ToPagedList_ShouldConvert_FromEnumerable(int totalCount, int pageIndex, int pageSize, bool hasPrev, bool hasNext)
        {
            var pagedList = TestClass.GenerateEnumerable(totalCount).ToPagedList(pageIndex, pageSize);
            var taken = totalCount - (pageIndex * pageSize);

            Assert.Equal(taken > pageSize ? pageSize : taken < 0 ? 0 : taken, pagedList.Count);
            Assert.Equal(pageIndex, pagedList.PageIndex);
            Assert.Equal(pageSize, pagedList.PageSize);
            Assert.Equal(totalCount, pagedList.TotalCount);
            Assert.Equal(Math.Ceiling((double)totalCount / pageSize), pagedList.TotalPages);
            Assert.Equal(hasPrev, pagedList.HasPreviousPage);
            Assert.Equal(hasNext, pagedList.HasNextPage);
        }

        [Theory]
        [InlineData(95, 0, 10, false, true)]
        [InlineData(95, 9, 10, true, false)]
        [InlineData(95, 8, 10, true, true)]
        [InlineData(95, 23, 4, true, false)]
        [InlineData(95, 24, 4, true, false)]
        public void ToPagedList_ShouldConvert_FromQueryable(int totalCount, int pageIndex, int pageSize, bool hasPrev, bool hasNext)
        {
            var pagedList = TestClass.GenerateQueryable(totalCount).ToPagedList(pageIndex, pageSize);
            var taken = totalCount - (pageIndex * pageSize);

            Assert.Equal(taken > pageSize ? pageSize : taken < 0 ? 0 : taken, pagedList.Count);
            Assert.Equal(pageIndex, pagedList.PageIndex);
            Assert.Equal(pageSize, pagedList.PageSize);
            Assert.Equal(totalCount, pagedList.TotalCount);
            Assert.Equal(Math.Ceiling((double)totalCount / pageSize), pagedList.TotalPages);
            Assert.Equal(hasPrev, pagedList.HasPreviousPage);
            Assert.Equal(hasNext, pagedList.HasNextPage);
        }

        [Theory]
        [InlineData(95, 0, 10, false, true)]
        [InlineData(95, 9, 10, true, false)]
        [InlineData(95, 8, 10, true, true)]
        [InlineData(95, 23, 4, true, false)]
        [InlineData(95, 24, 4, true, false)]
        public void ToPagedList_ShouldConvert_FromList(int totalCount, int pageIndex, int pageSize, bool hasPrev, bool hasNext)
        {
            var pagedList = TestClass.GenerateList(totalCount).ToPagedList(pageIndex, pageSize);
            var taken = totalCount - (pageIndex * pageSize);

            Assert.Equal(taken > pageSize ? pageSize : taken < 0 ? 0 : taken, pagedList.Count);
            Assert.Equal(pageIndex, pagedList.PageIndex);
            Assert.Equal(pageSize, pagedList.PageSize);
            Assert.Equal(totalCount, pagedList.TotalCount);
            Assert.Equal(Math.Ceiling((double)totalCount / pageSize), pagedList.TotalPages);
            Assert.Equal(hasPrev, pagedList.HasPreviousPage);
            Assert.Equal(hasNext, pagedList.HasNextPage);
        }

        private class TestClass
        {
            public string Name { get; set; }

            public static IList<TestClass> GenerateList(int count)
            {
                var list = new List<TestClass>();
                for (int i = 0; i < count; i++)
                    list.Add(new TestClass() { Name = $"Test Class {i}" });
                return list;
            }

            public static IQueryable<TestClass> GenerateQueryable(int count)
            {
                return GenerateList(count).AsQueryable();
            }

            public static IEnumerable<TestClass> GenerateEnumerable(int count)
            {
                return GenerateList(count).AsEnumerable();
            }
        }
    }
}
