using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Xunit;

namespace Fathcore.Extensions.Tests
{
    public class ObjectExtensionsTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SetPropertyValue_SafetyCheck(bool nullSource)
        {
            TestObject source = default;

            if (!nullSource)
                source = new TestObject();

            Assert.Throws<ArgumentNullException>(() => source.SetPropertyValue<TestObject, string>(default, default));
        }

        [Fact]
        public void SetPropertyValue_WithExpression_IsNot_MemberExpression_ShouldFail()
        {
            var testObject = new TestObject();

            testObject.SetPropertyValue(p => p.ToString(), "Property was Setted.");

            Assert.DoesNotContain("Property was Setted.", JsonConvert.SerializeObject(testObject));
        }

        [Fact]
        public void SetPropertyValue_WithMemberExpression_IsNot_Property_ShouldFail()
        {
            var testObject = new TestObject();

            testObject.SetPropertyValue(p => p.Method("Property was Setted."), "Property was Setted.");

            Assert.DoesNotContain("Property was Setted.", JsonConvert.SerializeObject(testObject));
        }

        [Fact]
        public void SetPropertyValue_WithReadOnlyProperty_ShouldFail()
        {
            var testObject = new TestObject();

            testObject.SetPropertyValue(p => p.GetOnlyProperty, "Property was Setted.");

            Assert.DoesNotContain("Property was Setted.", JsonConvert.SerializeObject(testObject));
        }

        [Fact]
        public void SetPropertyValue_WithPublicProperty_Should_Success()
        {
            var testObject = new TestObject();

            testObject.SetPropertyValue(p => p.PublicProperty, "Property was Setted.");

            Assert.Contains("Property was Setted.", JsonConvert.SerializeObject(testObject));
            Assert.Equal("Property was Setted.", testObject.PublicProperty);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetPropertyValue_SafetyCheck(bool nullSource)
        {
            TestObject source = default;

            if (!nullSource)
                source = new TestObject();

            Assert.Throws<ArgumentNullException>(() => source.GetPropertyValue<string>(default));
            Assert.Throws<ArgumentNullException>(() => source.GetPropertyValue<string>(" "));
            Assert.Throws<ArgumentNullException>(() => source.GetPropertyValue(default));
            Assert.Throws<ArgumentNullException>(() => source.GetPropertyValue(" "));
        }

        [Fact]
        public void GetPropertyValue_Should_Success()
        {
            var testObject = new TestObject()
            {
                PublicProperty = "Property was Setted."
            };

            var result = testObject.GetPropertyValue<string>("PublicProperty");

            Assert.Equal("Property was Setted.", result);
        }

        [Fact]
        public void AsEnumerable_SafetyCheck()
        {
            TestObject source = default;

            Assert.Throws<ArgumentNullException>(() => source.AsEnumerable());
        }

        [Fact]
        public void AsEnumerable_FromSingleObject_Should_Success()
        {
            var source = new TestObject()
            {
                PublicProperty = "PublicProperty"
            };

            var result = source.AsEnumerable();

            Assert.Single(result);
            Assert.Equal(source.PublicProperty, result.Single().PublicProperty);
        }

        [Fact]
        public void AsEnumerable_FromSingleObject_Enumerator_Should_Success()
        {
            var source = new TestObject()
            {
                PublicProperty = "PublicProperty"
            };

            var enumerator = source.AsEnumerable().GetEnumerator();

            var type = enumerator.GetType();
            var fieldInfo = type.GetField("<>1__state", BindingFlags.Instance | BindingFlags.NonPublic);
            fieldInfo.SetValue(enumerator, -1);

            enumerator.MoveNext();

            Assert.Null(enumerator.Current);
        }

        [Fact]
        public void Clone_SafetyCheck()
        {
            var source = new UnserializableObject();

            Assert.Throws<ArgumentException>(() => source.Clone());
        }

        [Fact]
        public void Clone_NullObject_ShouldPass()
        {
            TestObject source = default;

            var result = source.Clone();

            Assert.Null(result);
        }

        [Fact]
        public void Clone_Object_ShouldPass()
        {
            var source = new TestObject()
            {
                PublicProperty = "PublicProperty"
            };

            var result = source.Clone();

            Assert.NotSame(source, result);
            Assert.Equal(source.PublicProperty, result.PublicProperty);
            Assert.Equal(source.GetOnlyProperty, result.GetOnlyProperty);
        }

        [Fact]
        public void CloneJson_NullObject_ShouldPass()
        {
            TestObject source = default;

            var result = source.CloneJson();

            Assert.Null(result);
        }

        [Fact]
        public void CloneJson_Object_ShouldPass()
        {
            var source = new TestObject()
            {
                PublicProperty = "PublicProperty"
            };

            var result = source.CloneJson();

            Assert.NotSame(source, result);
            Assert.Equal(source.PublicProperty, result.PublicProperty);
            Assert.Equal(source.GetOnlyProperty, result.GetOnlyProperty);
        }

        [Serializable]
        private class TestObject
        {
            public string PublicProperty { get; set; }
            public string GetOnlyProperty { get; }

            public TestObject()
            {
                GetOnlyProperty = "Value from constructor";
            }

            public string Method(string input)
            {
                return input;
            }
        }

        private class UnserializableObject { }
    }
}
