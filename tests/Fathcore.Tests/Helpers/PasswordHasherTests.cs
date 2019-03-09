using System;
using System.Text.RegularExpressions;
using Fathcore.Helpers;
using Fathcore.Helpers.Abstractions;
using Fathcore.Providers;
using Fathcore.Providers.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;

namespace Fathcore.Tests.Helpers
{
    public class PasswordHasherTests
    {
        public ICommonHelpers CommonHelpers => new CommonHelpers(CoreFileProvider);
        public IValidationHelpers ValidationHelpers
        {
            get
            {
                var _validationHelpersMock = new Mock<IValidationHelpers>();
                _validationHelpersMock
                    .Setup(prop => prop.ThrowIfNull(null, null))
                    .Throws(new ArgumentException());

                return _validationHelpersMock.Object;
            }
        }
        public ICoreFileProvider CoreFileProvider
        {
            get
            {
                var _hostingEnvironmentMock = new Mock<IHostingEnvironment>();
                _hostingEnvironmentMock
                    .Setup(prop => prop.ContentRootPath)
                    .Returns(AppDomain.CurrentDomain.BaseDirectory);
                _hostingEnvironmentMock
                    .Setup(prop => prop.WebRootPath)
                    .Returns(AppDomain.CurrentDomain.BaseDirectory);

                return new CoreFileProvider(_hostingEnvironmentMock.Object);
            }
        }

        [Theory]
        [InlineData("Sample String")]
        [InlineData("1sKQ^+@?J.Euq>|g|&lp6u$zNmWMo]RB{xe.iC!LsUqUP1o8)")]
        [InlineData("Песня про надежду")]
        public void Should_Hash_A_String(string plainPassword)
        {
            IPasswordHasher passwordHasher = new PasswordHasher(CommonHelpers, ValidationHelpers);

            string hashedPassword = passwordHasher.HashPassword(plainPassword);

            Assert.Matches(new Regex(@"^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$"), hashedPassword);
        }

        [Theory]
        [InlineData("Sample String")]
        [InlineData("1sKQ^+@?J.Euq>|g|&lp6u$zNmWMo]RB{xe.iC!LsUqUP1o8)")]
        [InlineData("Песня про надежду")]
        public void Should_Verify_Hashed_String(string plainPassword)
        {
            IPasswordHasher passwordHasher = new PasswordHasher(CommonHelpers, ValidationHelpers);

            string hashedPassword = passwordHasher.HashPassword(plainPassword);
            var result = passwordHasher.VerifyHashedPassword(plainPassword, hashedPassword);

            Assert.Equal(PasswordVerificationStatus.Success, result);
        }

        [Theory]
        [InlineData("Sample String")]
        [InlineData("1sKQ^+@?J.Euq>|g|&lp6u$zNmWMo]RB{xe.iC!LsUqUP1o8)")]
        [InlineData("Песня про надежду")]
        public void Should_Verify_Non_Hashed_String(string plainPassword)
        {
            IPasswordHasher passwordHasher = new PasswordHasher(CommonHelpers, ValidationHelpers);

            string hashedPassword = plainPassword;
            var result = passwordHasher.VerifyHashedPassword(plainPassword, hashedPassword);

            Assert.Equal(PasswordVerificationStatus.SuccessRehashNeeded, result);
        }

        [Theory]
        [InlineData("Sample String")]
        [InlineData("1sKQ^+@?J.Euq>|g|&lp6u$zNmWMo]RB{xe.iC!LsUqUP1o8)")]
        [InlineData("Песня про надежду")]
        public void Verify_Hashed_String_Should_Fail_If_Not_The_Same(string plainPassword)
        {
            IPasswordHasher passwordHasher = new PasswordHasher(CommonHelpers, ValidationHelpers);

            string hashedPassword = passwordHasher.HashPassword("Password Verification Status Will Failed");
            var result = passwordHasher.VerifyHashedPassword(plainPassword, hashedPassword);

            Assert.Equal(PasswordVerificationStatus.Failed, result);
        }

        [Theory]
        [InlineData("Sample String")]
        [InlineData("1sKQ^+@?J.Euq>|g|&lp6u$zNmWMo]RB{xe.iC!LsUqUP1o8)")]
        [InlineData("Песня про надежду")]
        public void Hashed_Password_Never_The_Same(string plainPassword)
        {
            IPasswordHasher passwordHasher = new PasswordHasher(CommonHelpers, ValidationHelpers);

            string hashedPassword = passwordHasher.HashPassword(plainPassword);
            string hashedPassword1 = passwordHasher.HashPassword(plainPassword);
            string hashedPassword2 = passwordHasher.HashPassword(plainPassword);

            Assert.NotEqual(hashedPassword, hashedPassword1);
            Assert.NotEqual(hashedPassword1, hashedPassword2);
            Assert.Matches(new Regex(@"^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$"), hashedPassword);
            Assert.Matches(new Regex(@"^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$"), hashedPassword1);
            Assert.Matches(new Regex(@"^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$"), hashedPassword2);
        }

        [Fact]
        public void Should_Encrypt_And_Decrypt()
        {
            IPasswordHasher passwordHasher = new PasswordHasher(CommonHelpers, ValidationHelpers);
            var plainString = "Test RSA Encrypt.";

            var encrypted = passwordHasher.Encrypt(plainString);
            var encrypted1 = passwordHasher.Encrypt(plainString);
            var encrypted2 = passwordHasher.Encrypt(plainString);
            var decrypted = passwordHasher.Decrypt(encrypted);
            var decrypted1 = passwordHasher.Decrypt(encrypted1);
            var decrypted2 = passwordHasher.Decrypt(encrypted2);

            Assert.NotEqual(encrypted, encrypted1);
            Assert.NotEqual(encrypted1, encrypted2);

            Assert.Equal(plainString, decrypted);
            Assert.Equal(plainString, decrypted1);
            Assert.Equal(plainString, decrypted2);
        }

        [Fact]
        public void Should_Verify_Encrypted_Data()
        {
            IPasswordHasher passwordHasher = new PasswordHasher(CommonHelpers, ValidationHelpers);
            var plainString = "Test RSA Encrypt.";

            var encrypted = passwordHasher.Encrypt(plainString);
            var encrypted1 = passwordHasher.Encrypt(plainString);
            var encrypted2 = passwordHasher.Encrypt(plainString);

            var areSame = passwordHasher.VerifyEncyptedData(encrypted, encrypted1);
            var areSame1 = passwordHasher.VerifyEncyptedData(encrypted1, encrypted2);

            Assert.NotEqual(encrypted, encrypted1);
            Assert.NotEqual(encrypted1, encrypted2);

            Assert.True(areSame);
            Assert.True(areSame1);
        }

        [Fact]
        public void Should_Verify_Encrypted_Password()
        {
            IPasswordHasher passwordHasher = new PasswordHasher(CommonHelpers, ValidationHelpers);
            var plainString = "Test RSA Encrypt.";

            var encrypted = passwordHasher.Encrypt(plainString);

            var verificationStatus = passwordHasher.VerifyEncyptedPassword(plainString, encrypted);
            var verificationStatus1 = passwordHasher.VerifyEncyptedPassword(plainString, "Test RSA Encrypt.");
            var verificationStatus2 = passwordHasher.VerifyEncyptedPassword(plainString, "Test RSA Encrypt. ");

            Assert.Equal(PasswordVerificationStatus.Success, verificationStatus);
            Assert.Equal(PasswordVerificationStatus.SuccessRehashNeeded, verificationStatus1);
            Assert.Equal(PasswordVerificationStatus.Failed, verificationStatus2);
        }
    }
}
