using System.Security.Cryptography;
using Fathcore.Abstractions;

namespace Fathcore.Providers.Abstractions
{
    /// <summary>
    /// Represents a password hasher
    /// </summary>
    public interface IPasswordHasher : ISingletonService
    {
        /// <summary>
        /// Returns RSA public key file name
        /// </summary>
        string RSAPublicKeyFileName { get; }

        /// <summary>
        /// Returns RSA private key file name
        /// </summary>
        string RSAPrivateKeyFileName { get; }
        
        /// <summary>
        /// Returns a hashed representation of the supplied <paramref name="password"/>
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <returns>A hashed representation of the supplied <paramref name="password"/></returns>
        string HashPassword(string password);

        /// <summary>
        /// Encrypts the input data.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <returns>The encrypted data.</returns>
        string Encrypt(string data);

        /// <summary>
        /// Encrypts the input data using the specified padding mode.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="padding">The padding mode.</param>
        /// <returns>The encrypted data.</returns>
        string Encrypt(string data, RSAEncryptionPadding padding);

        /// <summary>
        /// Encrypts the input data using the specified padding mode and private key file path.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="padding">The padding mode.</param>
        /// <param name="privateKeyFilePath">The private key file path.</param>
        /// <returns>The encrypted data.</returns>
        byte[] Encrypt(byte[] data, RSAEncryptionPadding padding, string privateKeyFilePath);

        /// <summary>
        /// Decrypts the input data.
        /// </summary>
        /// <param name="data">The data to decrypt.</param>
        /// <returns>The decrypted data.</returns>
        string Decrypt(string data);

        /// <summary>
        /// Decrypts the input data using the specified padding mode.
        /// </summary>
        /// <param name="data">The data to decrypt.</param>
        /// <param name="padding">The padding mode.</param>
        /// <returns>The decrypted data.</returns>
        string Decrypt(string data, RSAEncryptionPadding padding);

        /// <summary>
        /// Decrypts the input data using the specified padding mode and private key file path.
        /// </summary>
        /// <param name="data">The data to decrypt.</param>
        /// <param name="padding">The padding mode.</param>
        /// <param name="privateKeyFilePath">The private key file path.</param>
        /// <returns>The decrypted data.</returns>
        byte[] Decrypt(byte[] data, RSAEncryptionPadding padding, string privateKeyFilePath);

        /// <summary>
        /// Returns a <see cref="bool"/> indicating the result of a encrypted data comparison.
        /// </summary>
        /// <param name="data">The encrypted data to compare.</param>
        /// <param name="anotherData">The another encrypted data to compare.</param>
        /// <returns>A <see cref="bool"/> indicating the result of a password hash comparison</returns>
        bool VerifyEncyptedData(string data, string anotherData);
        
        /// <summary>
        /// Returns a <see cref="bool"/> indicating the result of a encrypted data comparison.
        /// </summary>
        /// <param name="data">The encrypted data to compare.</param>
        /// <param name="anotherData">The another encrypted data to compare.</param>
        /// <param name="padding">The padding mode.</param>
        /// <returns>A <see cref="bool"/> indicating the result of a password hash comparison</returns>
        bool VerifyEncyptedData(string data, string anotherData, RSAEncryptionPadding padding);

        /// <summary>
        /// Returns a <see cref="bool"/> indicating the result of a encrypted data comparison.
        /// </summary>
        /// <param name="data">The encrypted data to compare.</param>
        /// <param name="anotherData">The another encrypted data to compare.</param>
        /// <param name="padding">The padding mode.</param>
        /// <param name="privateKeyFilePath">The private key file path.</param>
        /// <returns>A <see cref="bool"/> indicating the result of a password hash comparison</returns>
        bool VerifyEncyptedData(string data, string anotherData, RSAEncryptionPadding padding, string privateKeyFilePath);
        
        /// <summary>
        /// Returns a <see cref="bool"/> indicating the result of a encrypted data comparison.
        /// </summary>
        /// <param name="data">The encrypted data to compare.</param>
        /// <param name="anotherData">The another encrypted data to compare.</param>
        /// <returns>A <see cref="bool"/> indicating the result of a password hash comparison</returns>
        bool VerifyEncyptedData(byte[] data, byte[] anotherData);
        
        /// <summary>
        /// Returns a <see cref="bool"/> indicating the result of a encrypted data comparison.
        /// </summary>
        /// <param name="data">The encrypted data to compare.</param>
        /// <param name="anotherData">The another encrypted data to compare.</param>
        /// <param name="padding">The padding mode.</param>
        /// <returns>A <see cref="bool"/> indicating the result of a password hash comparison</returns>
        bool VerifyEncyptedData(byte[] data, byte[] anotherData, RSAEncryptionPadding padding);

        /// <summary>
        /// Returns a <see cref="bool"/> indicating the result of a encrypted data comparison.
        /// </summary>
        /// <param name="data">The encrypted data to compare.</param>
        /// <param name="anotherData">The another encrypted data to compare.</param>
        /// <param name="padding">The padding mode.</param>
        /// <param name="privateKeyFilePath">The private key file path.</param>
        /// <returns>A <see cref="bool"/> indicating the result of a password hash comparison</returns>
        bool VerifyEncyptedData(byte[] data, byte[] anotherData, RSAEncryptionPadding padding, string privateKeyFilePath);

        /// <summary>
        /// Returns a <see cref="PasswordVerificationStatus"/> indicating the result of a password hash comparison
        /// </summary>
        /// <param name="providedPassword">The password supplied for comparison</param>
        /// <param name="hashedPassword">The hash value for a user's stored password</param>
        /// <returns>A <see cref="PasswordVerificationStatus"/> indicating the result of a password hash comparison</returns>
        /// <remarks>Implementations of this method should be time consistent</remarks>
        PasswordVerificationStatus VerifyHashedPassword(string hashedPassword, string providedPassword);

        /// <summary>
        /// Returns a <see cref="PasswordVerificationStatus"/> indicating the result of a encrypted data comparison.
        /// </summary>
        /// <param name="providedPassword">The password supplied for comparison</param>
        /// <param name="encryptedPassword">The encrypted value for a user's stored password</param>
        /// <returns>A <see cref="PasswordVerificationStatus"/> indicating the result of a password hash comparison</returns>
        PasswordVerificationStatus VerifyEncyptedPassword(string providedPassword, string encryptedPassword);
        
        /// <summary>
        /// Returns a <see cref="PasswordVerificationStatus"/> indicating the result of a encrypted data comparison.
        /// </summary>
        /// <param name="providedPassword">The password supplied for comparison</param>
        /// <param name="encryptedPassword">The encrypted value for a user's stored password</param>
        /// <param name="padding">The padding mode.</param>
        /// <returns>A <see cref="PasswordVerificationStatus"/> indicating the result of a password hash comparison</returns>
        PasswordVerificationStatus VerifyEncyptedPassword(string providedPassword, string encryptedPassword, RSAEncryptionPadding padding);
    }
}
