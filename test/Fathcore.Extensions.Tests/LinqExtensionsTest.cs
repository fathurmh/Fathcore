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

        private class TestObject
        {
            public string Name { get; set; }
        }

        private void ModifyObject(TestObject testObject)
        {
            testObject.Name = $"{testObject.Name} modified";
        }
    }
}
