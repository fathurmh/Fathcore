namespace Fathcore.Security.Cryptography
{
    /// <summary>
    /// Represents hash factory.
    /// </summary>
    public interface IHashFactory
    {
        /// <summary>
        /// Hash data using the PBKDF2 algorithm. 
        /// </summary>
        /// <param name="data">Data being hashed.</param>
        /// <returns>Hashed data.</returns>
        string Pbkdf2Hash(string data);

        /// <summary>
        /// Verify specified data with hashed data.
        /// </summary>
        /// <param name="data">Specified password.</param>
        /// <param name="hashedData">Hashed password.</param>
        /// <returns><see cref="VerificationStatus"/>.</returns>
        VerificationStatus Pbkdf2Verify(string data, string hashedData);

        /// <summary>
        /// Hash data using the MD5 algorithm. 
        /// </summary>
        /// <param name="data">Data being hashed.</param>
        /// <returns>Hashed data.</returns>
        string Md5Hash(byte[] data);

        /// <summary>
        /// Hash data using the MD5 algorithm. 
        /// </summary>
        /// <param name="data">Data being hashed.</param>
        /// <returns>Hashed data.</returns>
        string Md5Hash(string data);
    }
}