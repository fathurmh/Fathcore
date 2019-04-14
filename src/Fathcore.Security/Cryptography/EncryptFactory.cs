using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Fathcore.Extensions;
using Fathcore.Infrastructure;
using PemUtils;

namespace Fathcore.Security.Cryptography
{
    /// <summary>
    /// Represents RSA encryptor.
    /// </summary>
    public class EncryptFactory : IEncryptFactory
    {
        private readonly RSA _defaultRsa;

        /// <summary>
        /// Gets default RSA encryption padding.
        /// </summary>
        public RSAEncryptionPadding DefaultRsaEncryptionPadding => RSAEncryptionPadding.OaepSHA512;

        /// <summary>
        /// Gets default RSA public key file name;
        /// </summary>
        public string DefaultRsaPublicKeyFileName { get; } = "rsa_2048_pub.pem";

        /// <summary>
        /// Gets default RSA private key file name;
        /// </summary>
        public string DefaultRsaPrivateKeyFileName { get; } = "rsa_2048_priv.pem";

        public EncryptFactory()
        {
            string filePath = Path.Combine(HelperContext.Current.DefaultFileProvider.BaseDirectory, DefaultRsaPrivateKeyFileName);
            _defaultRsa = ReadRsaFile(filePath);
        }

        /// <summary>
        /// Verify specified password with encrypted password.
        /// </summary>
        /// <param name="providedPassword">Specified password</param>
        /// <param name="encryptedPassword">Encrypted password.</param>
        /// <returns><see cref="VerificationStatus"/>.</returns>
        public VerificationStatus RsaVerify(string providedPassword, string encryptedPassword)
        {
            if (string.IsNullOrWhiteSpace(providedPassword))
                throw new ArgumentNullException(nameof(providedPassword));

            if (string.IsNullOrWhiteSpace(encryptedPassword))
                throw new ArgumentNullException(nameof(encryptedPassword));

            if (providedPassword.Equals(encryptedPassword, StringComparison.Ordinal))
                return VerificationStatus.SuccessRehashNeeded;

            if (encryptedPassword.IsBase64String())
            {
                var decryptedPassword = RsaDecrypt(encryptedPassword);
                if (providedPassword.Equals(decryptedPassword, StringComparison.Ordinal))
                    return VerificationStatus.Success;

                return VerificationStatus.Failed;
            }

            return VerificationStatus.Failed;
        }

        /// <summary>
        /// Encrypt data using RSA algorithm.
        /// </summary>
        /// <param name="data">Data being encrypted.</param>
        /// <returns>Encrypted data.</returns>
        public string RsaEncrypt(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException(nameof(data));

            RSA rsa = _defaultRsa;
            byte[] bytesData = Encoding.UTF8.GetBytes(data);
            byte[] bytesEncryptedData = rsa.Encrypt(bytesData, DefaultRsaEncryptionPadding);
            string encrypted = Convert.ToBase64String(bytesEncryptedData);

            return encrypted;
        }

        /// <summary>
        /// Decrypt data using RSA algorithm.
        /// </summary>
        /// <param name="data">Data being decrypted.</param>
        /// <returns>Decrypted data.</returns>
        public string RsaDecrypt(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException(nameof(data));

            RSA rsa = _defaultRsa;
            byte[] bytesData = Convert.FromBase64String(data);
            byte[] bytesDecryptedData = rsa.Decrypt(bytesData, DefaultRsaEncryptionPadding);
            string decrypted = Encoding.UTF8.GetString(bytesDecryptedData);

            return decrypted;
        }

        private RSA ReadRsaFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            RSA rsa = null;
            if ((SingletonDictionary<string, RSA>.Instance).ContainsKey(filePath))
            {
                rsa = (SingletonDictionary<string, RSA>.Instance)[filePath];
            }
            else
            {
                if (HelperContext.Current.DefaultFileProvider.FileExists(filePath))
                {
                    using (FileStream stream = File.OpenRead(filePath))
                    {
                        using (var reader = new PemReader(stream))
                        {
                            RSAParameters rsaParameters = reader.ReadRsaKey();
                            rsa = RSA.Create();
                            rsa.ImportParameters(rsaParameters);
                        }
                    }
                    (SingletonDictionary<string, RSA>.Instance).Add(filePath, rsa);
                }
                else
                {
                    rsa = RSA.Create();
                    (SingletonDictionary<string, RSA>.Instance).Add(filePath, rsa);

                    using (FileStream stream = File.OpenWrite(filePath))
                    {
                        using (var writer = new PemWriter(stream))
                        {
                            writer.WritePrivateKey(rsa);
                        }
                    }

                    using (FileStream stream = File.OpenWrite(Path.Combine(HelperContext.Current.DefaultFileProvider.GetParentDirectory(filePath), DefaultRsaPublicKeyFileName)))
                    {
                        using (var writer = new PemWriter(stream))
                        {
                            writer.WritePublicKey(rsa);
                        }
                    }
                }
            }

            return rsa;
        }
    }
}
