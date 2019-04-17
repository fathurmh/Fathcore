using System.Collections.Generic;
using Xunit;

namespace Fathcore.Extensions.Tests
{
    public class StringExtensionsTest
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("abc==", false)]
        [InlineData(" abc== ", false)]
        [InlineData("abc=", true)]
        [InlineData(" abc= ", true)]
        public void IsBase64String_Should_Pass(string source, bool expected)
        {
            var result = source.IsBase64String();

            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(EmailData))]
        public void IsValidEmail_Should_Pass(string source, bool expected)
        {
            var result = source.IsValidEmail();

            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(JsonStringData))]
        public void IsValidJson_Should_Pass(string source, bool expected)
        {
            var result = source.IsValidJson();

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData(" ", " ")]
        [InlineData("plain", "P")]
        [InlineData(" plain", " ")]
        [InlineData("#plain", "#")]
        [InlineData("あいうえお", "あ")]
        public void FirstLetterToUpper_Should_Pass(string source, string expected)
        {
            var result = source.FirstLetterToUpper();

            Assert.Equal(expected, result?.Length >= 1 ? result?.Substring(0, 1) : result);
        }

        [Theory]
        [InlineData(null, Status.Started)]
        [InlineData("", Status.Started)]
        [InlineData(" ", Status.Started)]
        [InlineData("some string", Status.Started)]
        [InlineData("Started", Status.Started)]
        [InlineData("Pending", Status.Pending)]
        [InlineData("Completed", Status.Completed)]
        public void ToEnum_Should_Pass(string source, Status expected)
        {
            var result = source.ToEnum(Status.Started);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData(" ", "")]
        [InlineData("abc==", "abc==")]
        [InlineData(" abc== ", "abc==")]
        [InlineData("ab c=", "abc=")]
        [InlineData(" abc= ", "abc=")]
        public void RemoveWhitespace_Should_Pass(string source, string expected)
        {
            var result = source.RemoveWhitespace();

            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> EmailData()
        {
            var data = new List<object[]>
            {
                new object[] { null, false },
                new object[] { string.Empty, false },
                new object[] { " ", false },
                new object[] { "plainaddress", false},
                new object[] { "#@%^%#$@#$@#.com", false},
                new object[] { "@domain.com", false},
                new object[] { "Fathur <fathur@domain.com>", false},
                new object[] { "email.domain.com", false},
                new object[] { "email@domain@domain.com", false},
                new object[] { ".email@domain.com", false},
                new object[] { "email.@domain.com", false},
                new object[] { "email..email@domain.com", false},
                new object[] { "email@domain.com (Fathur)", false},
                new object[] { "email@domain", false},
                new object[] { "email@-domain.com", false},
                new object[] { "email@111.222.333.44444", false},
                new object[] { "email@domain..com", false},
                new object[] { "email@123.123.123.123", false},
                new object[] { "email@[123.123.123.123]", false},
                new object[] { "email@domain.com", true},
                new object[] { "firstname.lastname@domain.com", true},
                new object[] { "email@subdomain.domain.com", true},
                new object[] { "firstname+lastname@domain.com", true},
                new object[] { "\"email\"@domain.com", true},
                new object[] { "1234567890@domain.com", true},
                new object[] { "email@domain-one.com", true},
                new object[] { "_______@domain.com", true},
                new object[] { "email@domain.name", true},
                new object[] { "email@domain.co.jp", true},
                new object[] { "firstname-lastname@domain.com", true},
                new object[] { "あいうえお@domain.com", true},
            };

            return data;
        }

        public static IEnumerable<object[]> JsonStringData()
        {
            var data = new List<object[]>
            {
                new object[] { null, false },
                new object[] { string.Empty, false },
                new object[] { " ", false },
                new object[] { "plainaddress", false},
                new object[] { "{ \"key\": value, \"anotherKey\": value }", false},
                new object[] { "[ \"key\": value, \"anotherKey\": value ]", false},
                new object[] { "[ \"key\": \"value\", \"another Key\": [\"value\", \"another value\"] ]", false},
                new object[] { "{ \"key\": \"value\", \"anotherKey\": \"value\" }", true},
                new object[] { "{ \"key\": \"value\", \"another Key\": \"value\" }", true},
                new object[] { "{ \"key\": \"value\", \"another Key\": [\"value\", \"another value\"] }", true},
            };

            return data;
        }

        public enum Status
        {
            Started = 0,
            Pending = 1,
            Completed = 2
        }
    }
}
