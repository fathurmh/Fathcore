using System;
using System.Collections.Generic;
using System.Text;
using Fathcore.Helpers;
using Fathcore.Helpers.Abstractions;
using Xunit;

namespace Fathcore.Tests.Helpers
{
    public class CommonHelpersTests
    {
        [Theory]
        [MemberData(nameof(Data))]
        public void Should_Determine_Base_64_Encoded_String(string stringEncoded, bool expected)
        {
            ICommonHelpers commonHelpers = new CommonHelpers();

            bool result = commonHelpers.IsBase64Encoded(stringEncoded);

            Assert.Equal(expected, result);
        }
        
        public static IEnumerable<object[]> Data()
        {
            return new List<object[]>
            {
                new object[] { Convert.ToBase64String(Encoding.ASCII.GetBytes("Sample Strings")), true },
                new object[] { Encoding.ASCII.GetBytes("Sample Strings").ToString(), false },
                new object[] { "Sample Strings", false }
            };
        }
    }
}
