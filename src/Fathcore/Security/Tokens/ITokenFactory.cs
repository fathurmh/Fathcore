using System;
using System.Security.Claims;

namespace Fathcore.Security.Tokens
{
    /// <summary>
    /// Represents a token factory.
    /// </summary>
    public interface ITokenFactory
    {
        /// <summary>
        /// Gets token descriptor expires value.
        /// </summary>
        DateTime Expires { get; }

        /// <summary>
        /// Generate token with customable claims.
        /// </summary>
        /// <param name="claims">Given claims.</param>
        /// <returns>Generated token.</returns>
        string GenerateToken(params Claim[] claims);
    }
}