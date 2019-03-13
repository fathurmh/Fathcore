using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Fathcore.Extensions.Tests
{
    public class LinqExtensionsTest
    {
        [Fact]
        public void Object_Should_Be_Modified()
        {
            var testObject = new TestObject() { Name = "Name of Test Object" };

            ModifyObject(testObject);

            Assert.Equal($"Name of Test Object modified", testObject.Name);
        }

        [Fact]
        public async Task Asynchronous_For_Each_Should_Be_Working()
        {
            var testObjects = new List<TestObject>()
            {
                new TestObject() { Name = "Name of Test Object" },
                new TestObject() { Name = "Name of Test Object" },
                new TestObject() { Name = "Name of Test Object" },
                new TestObject() { Name = "Name of Test Object" },
                new TestObject() { Name = "Name of Test Object" }
            };

            await testObjects.ForEachAsync(async testObject => await Task.Run(() => ModifyObject(testObject)).ConfigureAwait(false));

            Assert.All(testObjects, result => Assert.Equal($"Name of Test Object modified", result.Name));
        }

        [Fact]
        public async Task Asynchronous_For_Each_Should_Be_Working_On_Async_Method()
        {
            var testObjects = new List<TestObject>()
            {
                new TestObject() { Name = "Name of Test Object" },
                new TestObject() { Name = "Name of Test Object" },
                new TestObject() { Name = "Name of Test Object" },
                new TestObject() { Name = "Name of Test Object" },
                new TestObject() { Name = "Name of Test Object" }
            };

            await testObjects.ForEachAsync(async testObject => await ModifyObjectAsync(testObject));

            Assert.All(testObjects, result => Assert.Equal($"Name of Test Object modified", result.Name));
        }

        [Fact]
        public async Task Should_Throw_An_Exception_Is_Func_Not_Given()
        {
            var testObjects = new List<TestObject>()
            {
                new TestObject() { Name = "Name of Test Object" },
                new TestObject() { Name = "Name of Test Object" },
                new TestObject() { Name = "Name of Test Object" },
                new TestObject() { Name = "Name of Test Object" },
                new TestObject() { Name = "Name of Test Object" }
            };

            await testObjects.ForEachAsync(async testObject => await ModifyObjectAsync(testObject));

            await Assert.ThrowsAsync<ArgumentNullException>(() => testObjects.ForEachAsync(null));
        }

        private class TestObject
        {
            public string Name { get; set; }
        }

        private void ModifyObject(TestObject testObject)
        {
            testObject.Name = $"{testObject.Name} modified";
        }

        private Task ModifyObjectAsync(TestObject testObject)
        {
            testObject.Name = $"{testObject.Name} modified";
            return Task.CompletedTask;
        }
    }
}
