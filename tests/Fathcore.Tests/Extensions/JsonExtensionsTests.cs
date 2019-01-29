using System;
using System.Collections.Generic;
using Fathcore.Extensions;
using Newtonsoft.Json;
using Xunit;

namespace Fathcore.Tests.Extensions
{
    public class JsonExtensionsTests
    {
        [Theory]
        [MemberData(nameof(Data))]
        public void Should_Determine_Valid_Json_String(object data)
        {
            string jsonObject = JsonConvert.SerializeObject(data);

            bool isValidJsonObject = jsonObject.IsValidJson();
            
            Assert.True(isValidJsonObject);
        }

        public static IEnumerable<object[]> Data()
        {
            return new List<object[]>
            {
                new object[] { new { Name = "Name", Valid = true } },
                new object[] { new object[] { 1, "String", 3 } },
                new object[] { new { Name = "Name", Valid = true, Object = new object[] { 1, "String", 3 } } }
            };
        }
    }
}
