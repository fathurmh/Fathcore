using Microsoft.IdentityModel.Tokens;

namespace Fathcore.Security.Tokens
{
    /// <summary>
    /// Represents a token setting.
    /// </summary>
    public interface ITokenSetting
    {
        /// <summary>
        /// Gets the value of key for <see cref="SymmetricSecurityKey"/>.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets the value of the 'audience' claim.
        /// </summary>
        string Audience { get; }

        /// <summary>
        /// Gets the value of the 'expiration' claim.
        /// </summary>
        int Expires { get; }

        /// <summary>
        /// Gets the issuer of this <see cref="ITokenSetting"/>.
        /// </summary>
        string Issuer { get; set; }
    }
}
