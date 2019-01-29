using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using Fathcore.Extensions;
using Xunit;

namespace Fathcore.Tests.Extensions
{
    public class PrincipalExtensionsTests
    {
        [Fact]
        public void Should_Get_Identity_Name()
        {
            IPrincipal principal = new GenericPrincipal(new GenericIdentity("TestIdentity"), null);

            Assert.Equal("TestIdentity", principal.Identity.Name);
        }
        
        [Theory]
        [InlineData("Name Claim", ClaimTypes.Name)]
        [InlineData("Role Claim", ClaimTypes.Role)]
        [InlineData("Custom Claim", "customClaim")]
        public void Should_Get_Claim_Value(string value, string claimTypes)
        {
            IPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{new Claim(claimTypes, value)}));
            
            var result = principal.Identity.GetClaimValue(claimTypes);

            Assert.Equal(value, result[0]);
        }

        [Fact]
        public void Should_Get_Role_Claim_Value()
        {
            IPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{new Claim(ClaimTypes.Role, "Role Test Identity")}));

            var result = principal.Identity.GetRoleTypeClaimValue();

            Assert.Equal("Role Test Identity", result[0]);
        }
    }
}
