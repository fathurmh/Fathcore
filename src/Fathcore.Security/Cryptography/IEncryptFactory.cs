using System.Security.Cryptography;

namespace Fathcore.Security.Cryptography
{
    public interface IEncryptFactory
    {

        /// <summary>
        /// Gets default RSA encryption padding.
        /// </summary>
        RSAEncryptionPadding DefaultRsaEncryptionPadding { get; }

        /// <summary>
        /// Gets default RSA public key file name;
        /// </summary>
        string DefaultRsaPublicKeyFileName { get; }

        /// <summary>
        /// Gets default RSA private key file name;
        /// </summary>
        string DefaultRsaPrivateKeyFileName { get; }

        /// <summary>
        /// Verify specified password with encrypted password.
        /// </summary>
        /// <param name="providedPassword">Specified password</param>
        /// <param name="encryptedPassword">Encrypted password.</param>
        /// <returns><see cref="VerificationStatus"/>.</returns>
        VerificationStatus RsaVerify(string providedPassword, string encryptedPassword);

        /// <summary>
        /// Encrypt data using RSA algorithm.
        /// </summary>
        /// <param name="data">Data being encrypted.</param>
        /// <returns>Encrypted data.</returns>
        string RsaEncrypt(string data);

        /// <summary>
        /// Decrypt data using RSA algorithm.
        /// </summary>
        /// <param name="data">Data being decrypted.</param>
        /// <returns>Decrypted data.</returns>
        string RsaDecrypt(string data);
    }
}