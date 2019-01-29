using Fathcore.Abstractions;

namespace Fathcore.Providers.Abstractions
{
    /// <summary>
    /// Represents a password hasher
    /// </summary>
    public interface IPasswordHasher : ISingletonService
    {
        /// <summary>
        /// Returns a hashed representation of the supplied <paramref name="password"/>
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <returns>A hashed representation of the supplied <paramref name="password"/></returns>
        string HashPassword(string password);

        /// <summary>
        /// Returns a <see cref="PasswordVerificationStatus"/> indicating the result of a password hash comparison
        /// </summary>
        /// <param name="hashedPassword">The hash value for a user's stored password</param>
        /// <param name="providedPassword">The password supplied for comparison</param>
        /// <returns>A <see cref="PasswordVerificationStatus"/> indicating the result of a password hash comparison</returns>
        /// <remarks>Implementations of this method should be time consistent</remarks>
        PasswordVerificationStatus VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}
