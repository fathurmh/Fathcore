using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Fathcore.Extensions
{
    public static class PrincipalExtensions
    {
        /// <summary>
        /// Get claim value from specified identity
        /// </summary>
        /// <param name="identity">IIDentity object</param>
        /// <param name="claimTypes">ClaimTypes</param>
        /// <returns>Returns claim value from specified identity</returns>
        public static string[] GetClaimValue(this IIdentity identity, string claimTypes)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            string [] claimValue = claimsIdentity.Claims
                .Where(prop => prop.Type == claimTypes)
                .Select(prop => prop.Value)
                .ToArray();

            return claimValue;
        }

        /// <summary>
        /// Get role type claim value from specified identity
        /// </summary>
        /// <param name="identity">IIDentity object</param>
        /// <returns>Returns role type claim value from specified identity</returns>
        public static string[] GetRoleTypeClaimValue(this IIdentity identity)
        {
            return identity.GetClaimValue(ClaimTypes.Role);
        }
    }
}
