using Newtonsoft.Json;
using Xunit;

namespace Fathcore.Extensions.Tests
{
    public class ObjectExtensionsTest
    {
        [Fact]
        public void Should_Not_SetPropertyValue_To_Expression_Which_Not_MemberExpression()
        {
            var testObject = new TestObject();

            testObject.SetPropertyValue(p => p.ToString(), "Property was Setted.");

            Assert.DoesNotContain("Property was Setted.", JsonConvert.SerializeObject(testObject));
        }

        [Fact]
        public void Should_Not_SetPropertyValue_To_MemberExpression_Which_Not_Property()
        {
            var testObject = new TestObject();

            testObject.SetPropertyValue(p => p.Method("Property was Setted."), "Property was Setted.");

            Assert.DoesNotContain("Property was Setted.", JsonConvert.SerializeObject(testObject));
        }

        [Fact]
        public void Should_Not_SetPropertyValue_To_Property_Which_Can_Not_Write()
        {
            var testObject = new TestObject();

            testObject.SetPropertyValue(p => p.GetOnlyProperty, "Property was Setted.");

            Assert.DoesNotContain("Property was Setted.", JsonConvert.SerializeObject(testObject));
        }

        [Fact]
        public void Should_SetPropertyValue_Only_To_PublicProperty()
        {
            var testObject = new TestObject();

            testObject.SetPropertyValue(p => p.PublicProperty, "Property was Setted.");

            Assert.Contains("Property was Setted.", JsonConvert.SerializeObject(testObject));
            Assert.Equal("Property was Setted.", testObject.PublicProperty);
        }

        private class TestObject
        {
            public string PublicProperty { get; set; }
            public string GetOnlyProperty { get; }

            public string Method(string input)
            {
                return input;
            }
        }
    }
}
