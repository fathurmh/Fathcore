using System;
using System.Security.Claims;
using System.Security.Principal;
using Fathcore.Abstractions;

namespace Fathcore.Providers.Abstractions
{
    public interface ITokenProvider : IScopedService
    {
        /// <summary>
        /// Return generated token
        /// </summary>
        /// <value></value>
        string Token { get; }

        /// <summary>
        /// Return expiry of generated token
        /// </summary>
        DateTime Expires { get; }

        /// <summary>
        /// Generate a token
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        string GenerateToken(params Claim[] claims);
    }
}
