using System;
using System.Linq;
using System.Text.RegularExpressions;
using Fathcore.Abstractions;
using Fathcore.Helpers;
using Fathcore.Helpers.Abstractions;
using Fathcore.Providers;
using Fathcore.Providers.Abstractions;
using Xunit;

namespace Fathcore.Tests.Helpers
{
    public class PasswordHasherTests
    {
        [Theory]
        [InlineData("Sample String")]
        [InlineData("1sKQ^+@?J.Euq>|g|&lp6u$zNmWMo]RB{xe.iC!LsUqUP1o8)")]
        [InlineData("Песня про надежду")]
        public void Should_Hash_A_String(string plainPassword)
        {
            ICommonHelpers commonHelpers = new CommonHelpers();
            IPasswordHasher passwordHasher = new PasswordHasher(commonHelpers);

            string hashedPassword = passwordHasher.HashPassword(plainPassword);

            Assert.Matches(new Regex(@"^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$"), hashedPassword);
        }

        [Theory]
        [InlineData("Sample String")]
        [InlineData("1sKQ^+@?J.Euq>|g|&lp6u$zNmWMo]RB{xe.iC!LsUqUP1o8)")]
        [InlineData("Песня про надежду")]
        public void Should_Verify_Hashed_String(string plainPassword)
        {
            ICommonHelpers commonHelpers = new CommonHelpers();
            IPasswordHasher passwordHasher = new PasswordHasher(commonHelpers);

            string hashedPassword = passwordHasher.HashPassword(plainPassword);
            var result = passwordHasher.VerifyHashedPassword(hashedPassword, plainPassword);
            
            Assert.Equal(PasswordVerificationStatus.Success, result);
        }

        [Theory]
        [InlineData("Sample String")]
        [InlineData("1sKQ^+@?J.Euq>|g|&lp6u$zNmWMo]RB{xe.iC!LsUqUP1o8)")]
        [InlineData("Песня про надежду")]
        public void Should_Verify_Non_Hashed_String(string plainPassword)
        {
            ICommonHelpers commonHelpers = new CommonHelpers();
            IPasswordHasher passwordHasher = new PasswordHasher(commonHelpers);

            string hashedPassword = plainPassword;
            var result = passwordHasher.VerifyHashedPassword(hashedPassword, plainPassword);
            
            Assert.Equal(PasswordVerificationStatus.SuccessRehashNeeded, result);
        }

        [Theory]
        [InlineData("Sample String")]
        [InlineData("1sKQ^+@?J.Euq>|g|&lp6u$zNmWMo]RB{xe.iC!LsUqUP1o8)")]
        [InlineData("Песня про надежду")]
        public void Verify_Hashed_String_Should_Fail_If_Not_The_Same(string plainPassword)
        {
            ICommonHelpers commonHelpers = new CommonHelpers();
            IPasswordHasher passwordHasher = new PasswordHasher(commonHelpers);

            string hashedPassword = passwordHasher.HashPassword("Password Verification Status Will Failed");
            var result = passwordHasher.VerifyHashedPassword(hashedPassword, plainPassword);
            
            Assert.Equal(PasswordVerificationStatus.Failed, result);
        }

        [Theory]
        [InlineData("Sample String")]
        [InlineData("1sKQ^+@?J.Euq>|g|&lp6u$zNmWMo]RB{xe.iC!LsUqUP1o8)")]
        [InlineData("Песня про надежду")]
        public void Hashed_Password_Never_The_Same(string plainPassword)
        {
            ICommonHelpers commonHelpers = new CommonHelpers();
            IPasswordHasher passwordHasher = new PasswordHasher(commonHelpers);

            string hashedPassword = passwordHasher.HashPassword(plainPassword);
            string hashedPassword1 = passwordHasher.HashPassword(plainPassword);
            string hashedPassword2 = passwordHasher.HashPassword(plainPassword);

            Assert.NotEqual(hashedPassword, hashedPassword1);
            Assert.NotEqual(hashedPassword1, hashedPassword2);
            Assert.Matches(new Regex(@"^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$"), hashedPassword);
            Assert.Matches(new Regex(@"^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$"), hashedPassword1);
            Assert.Matches(new Regex(@"^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$"), hashedPassword2);
        }
    }
}
