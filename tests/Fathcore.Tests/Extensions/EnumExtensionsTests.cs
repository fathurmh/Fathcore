using System;
using System.ComponentModel;
using Fathcore.Extensions;
using Xunit;

namespace Fathcore.Tests.Extensions
{
    public class EnumExtensionsTests
    {
        [Theory]
        [InlineData(1, "Satu")]
        [InlineData(2, "Dua")]
        [InlineData(3, "Tiga")]
        public void Should_Get_Enum_Description(int numb, string desc)
        {
            EnumTest enumVar = (EnumTest)numb;
            string enumDescription = enumVar.GetDescription();

            Assert.Equal(desc, enumDescription);
        }
    }

    internal enum EnumTest
    {
        [Description("Satu")]
        One = 1,
        [Description("Dua")]
        Two = 2,
        [Description("Tiga")]
        Three = 3
    }
}
