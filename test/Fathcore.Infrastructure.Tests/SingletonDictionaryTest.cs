using System;
using System.Collections.Generic;
using Xunit;

namespace Fathcore.Infrastructure.Tests
{
    public class SingletonDictionaryTest
    {
        [Fact]
        public void SingletonDictionary_Is_Created_By_Default()
        {
            IDictionary<SingletonDictionaryTest, object> instance = SingletonDictionary<SingletonDictionaryTest, object>.Instance;

            Assert.NotNull(instance);
        }

        [Fact]
        public void SingletonDictionary_Can_Store_Stuff()
        {
            IDictionary<Type, SingletonDictionaryTest> instance = SingletonDictionary<Type, SingletonDictionaryTest>.Instance;
            instance[typeof(SingletonDictionaryTest)] = this;

            Assert.Same(this, instance[typeof(SingletonDictionaryTest)]);
        }
    }
}
