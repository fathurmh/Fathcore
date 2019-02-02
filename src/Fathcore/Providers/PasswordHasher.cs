using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Fathcore.Helpers.Abstractions;
using Fathcore.Infrastructures;
using Fathcore.Providers.Abstractions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PemUtils;

namespace Fathcore.Providers
{
    /// <summary>
    /// Represents a password hasher
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        private const string _rsaPublicKeyFileName = "rsa_2048_pub.pem";
        private const string _rsaPrivateKeyFileName = "rsa_2048_priv.pem";
        private readonly int _iterCount;
        private readonly RSA _rsa;
        private readonly RandomNumberGenerator _rng;
        private readonly ICommonHelpers _commonHelpers;

        /// <summary>
        /// Returns RSA public key file name
        /// </summary>
        public string RSAPublicKeyFileName => _rsaPublicKeyFileName;

        /// <summary>
        /// Returns RSA private key file name
        /// </summary>
        public string RSAPrivateKeyFileName => _rsaPrivateKeyFileName;
        private readonly IValidationHelpers _validationHelpers;

        public PasswordHasher(ICommonHelpers commonHelpers, IValidationHelpers validationHelpers, IOptions<PasswordHasherOptions> optionsAccessor = null)
        {
            _commonHelpers = commonHelpers;
            _validationHelpers = validationHelpers;
            var options = optionsAccessor?.Value ?? new PasswordHasherOptions();

            _iterCount = options.IterationCount;
            if (_iterCount < 1)
            {
                throw new InvalidOperationException();
            }

            _rng = RandomNumberGenerator.Create();
            _rsa = ReadRSA();
        }

        /// <summary>
        /// Returns a hashed representation of the supplied <paramref name="password"/>
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <returns>A hashed representation of the supplied <paramref name="password"/></returns>
        public virtual string HashPassword(string password)
        {
            _validationHelpers.ThrowIfNull(password, nameof(password));
            return Convert.ToBase64String(HashPasswordV3(password, _rng));
        }

        /// <summary>
        /// Returns a <see cref="PasswordVerificationStatus"/> indicating the result of a password hash comparison
        /// </summary>
        /// <param name="providedPassword">The password supplied for comparison</param>
        /// <param name="hashedPassword">The hash value for a user's stored password</param>
        /// <returns>A <see cref="PasswordVerificationStatus"/> indicating the result of a password hash comparison</returns>
        /// <remarks>Implementations of this method should be time consistent</remarks>
        public virtual PasswordVerificationStatus VerifyHashedPassword(string providedPassword, string hashedPassword)
        {
            _validationHelpers.ThrowIfNull(providedPassword, nameof(providedPassword));
            _validationHelpers.ThrowIfNull(hashedPassword, nameof(hashedPassword));

            if (_commonHelpers.IsBase64Encoded(hashedPassword))
            {
                byte[] decodedHashedPassword = Convert.FromBase64String(hashedPassword);

                if (decodedHashedPassword.Length == 0)
                {
                    return PasswordVerificationStatus.Failed;
                }

                if (decodedHashedPassword[0] == 0x01)
                {
                    if (VerifyHashedPasswordV3(decodedHashedPassword, providedPassword, out int embeddedIterCount))
                    {
                        return (embeddedIterCount < _iterCount) ? PasswordVerificationStatus.SuccessRehashNeeded : PasswordVerificationStatus.Success;
                    }
                    else
                    {
                        return PasswordVerificationStatus.Failed;
                    }
                }
                else
                {
                    return hashedPassword == providedPassword ? PasswordVerificationStatus.SuccessRehashNeeded : PasswordVerificationStatus.Failed;
                }
            }
            else
            {
                return hashedPassword == providedPassword ? PasswordVerificationStatus.SuccessRehashNeeded : PasswordVerificationStatus.Failed;
            }
        }

        /// <summary>
        /// Returns a <see cref="PasswordVerificationStatus"/> indicating the result of a encrypted data comparison.
        /// </summary>
        /// <param name="providedPassword">The password supplied for comparison</param>
        /// <param name="encryptedPassword">The encrypted value for a user's stored password</param>
        /// <returns>A <see cref="PasswordVerificationStatus"/> indicating the result of a password hash comparison</returns>
        public virtual PasswordVerificationStatus VerifyEncyptedPassword(string providedPassword, string encryptedPassword)
        {
            _validationHelpers.ThrowIfNull(providedPassword, nameof(providedPassword));
            _validationHelpers.ThrowIfNull(encryptedPassword, nameof(encryptedPassword));
            
            return VerifyEncyptedPassword(providedPassword, encryptedPassword, RSAEncryptionPadding.OaepSHA512);
        }

        /// <summary>
        /// Returns a <see cref="PasswordVerificationStatus"/> indicating the result of a encrypted data comparison.
        /// </summary>
        /// <param name="providedPassword">The password supplied for comparison</param>
        /// <param name="encryptedPassword">The encrypted value for a user's stored password</param>
        /// <param name="padding">The padding mode.</param>
        /// <returns>A <see cref="PasswordVerificationStatus"/> indicating the result of a password hash comparison</returns>
        public virtual PasswordVerificationStatus VerifyEncyptedPassword(string providedPassword, string encryptedPassword, RSAEncryptionPadding padding)
        {
            _validationHelpers.ThrowIfNull(providedPassword, nameof(providedPassword));
            _validationHelpers.ThrowIfNull(encryptedPassword, nameof(encryptedPassword));

            if (providedPassword == encryptedPassword)
            {
                return PasswordVerificationStatus.SuccessRehashNeeded;
            }
            else if (_commonHelpers.IsBase64Encoded(encryptedPassword))
            {
                var decryptedPassword = Decrypt(encryptedPassword, padding);
                if (providedPassword == decryptedPassword)
                {
                    return PasswordVerificationStatus.Success;
                }
                else
                {
                    return PasswordVerificationStatus.Failed;
                }
            }
            else
            {
                return PasswordVerificationStatus.Failed;
            }
        }

        /// <summary>
        /// Encrypts the input data.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <returns>The encrypted data.</returns>
        public virtual string Encrypt(string data)
        {
            _validationHelpers.ThrowIfNull(data, nameof(data));
            string encryptedData = Encrypt(data, RSAEncryptionPadding.OaepSHA512);
            return encryptedData;
        }

        /// <summary>
        /// Encrypts the input data using the specified padding mode.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="padding">The padding mode.</param>
        /// <returns>The encrypted data.</returns>
        public virtual string Encrypt(string data, RSAEncryptionPadding padding)
        {
            _validationHelpers.ThrowIfNull(data, nameof(data));
            _validationHelpers.ThrowIfNull(padding, nameof(padding));

            byte[] bytesData = Encoding.UTF8.GetBytes(data);
            byte[] bytesEncryptedData = Encrypt(bytesData, padding, _rsaPrivateKeyFileName);
            string encryptedData = Convert.ToBase64String(bytesEncryptedData);
            return encryptedData;
        }

        /// <summary>
        /// Encrypts the input data using the specified padding mode and private key file path.
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <param name="padding">The padding mode.</param>
        /// <param name="privateKeyFilePath">The private key file path.</param>
        /// <returns>The encrypted data.</returns>
        public virtual byte[] Encrypt(byte[] data, RSAEncryptionPadding padding, string privateKeyFilePath)
        {
            _validationHelpers.ThrowIfNull(data, nameof(data));
            _validationHelpers.ThrowIfNull(padding, nameof(padding));
            _validationHelpers.ThrowIfNull(privateKeyFilePath, nameof(privateKeyFilePath));

            RSA rsa = privateKeyFilePath == _rsaPrivateKeyFileName ? _rsa : ReadRSA(privateKeyFilePath);
            var encrypted = rsa.Encrypt(data, padding);
            return encrypted;
        }

        /// <summary>
        /// Decrypts the input data.
        /// </summary>
        /// <param name="data">The data to decrypt.</param>
        /// <returns>The decrypted data.</returns>
        public virtual string Decrypt(string data)
        {
            _validationHelpers.ThrowIfNull(data, nameof(data));

            string decryptedData = Decrypt(data, RSAEncryptionPadding.OaepSHA512);
            return decryptedData;
        }

        /// <summary>
        /// Decrypts the input data using the specified padding mode.
        /// </summary>
        /// <param name="data">The data to decrypt.</param>
        /// <param name="padding">The padding mode.</param>
        /// <returns>The decrypted data.</returns>
        public virtual string Decrypt(string data, RSAEncryptionPadding padding)
        {
            _validationHelpers.ThrowIfNull(data, nameof(data));
            _validationHelpers.ThrowIfNull(padding, nameof(padding));

            byte[] bytesData = Convert.FromBase64String(data);
            byte[] bytesDecryptedData = Decrypt(bytesData, padding, _rsaPrivateKeyFileName);
            string decryptedData = Encoding.UTF8.GetString(bytesDecryptedData);
            return decryptedData;
        }

        /// <summary>
        /// Decrypts the input data using the specified padding mode and private key file path.
        /// </summary>
        /// <param name="data">The data to decrypt.</param>
        /// <param name="padding">The padding mode.</param>
        /// <param name="privateKeyFilePath">The private key file path.</param>
        /// <returns>The decrypted data.</returns>
        public virtual byte[] Decrypt(byte[] data, RSAEncryptionPadding padding, string privateKeyFilePath)
        {
            _validationHelpers.ThrowIfNull(data, nameof(data));
            _validationHelpers.ThrowIfNull(padding, nameof(padding));
            _validationHelpers.ThrowIfNull(privateKeyFilePath, nameof(privateKeyFilePath));

            RSA rsa = privateKeyFilePath == _rsaPrivateKeyFileName ? _rsa : ReadRSA(privateKeyFilePath);
            var decrypted = rsa.Decrypt(data, padding);
            return decrypted;
        }

        /// <summary>
        /// Returns a <see cref="bool"/> indicating the result of a encrypted data comparison.
        /// </summary>
        /// <param name="data">The encrypted data to compare.</param>
        /// <param name="anotherData">The another encrypted data to compare.</param>
        /// <returns>A <see cref="bool"/> indicating the result of a password hash comparison</returns>
        public virtual bool VerifyEncyptedData(string data, string anotherData)
        {
            _validationHelpers.ThrowIfNull(data, nameof(data));
            _validationHelpers.ThrowIfNull(anotherData, nameof(anotherData));

            var areSame = VerifyEncyptedData(data, anotherData, RSAEncryptionPadding.OaepSHA512);
            return areSame;
        }

        /// <summary>
        /// Returns a <see cref="bool"/> indicating the result of a encrypted data comparison.
        /// </summary>
        /// <param name="data">The encrypted data to compare.</param>
        /// <param name="anotherData">The another encrypted data to compare.</param>
        /// <param name="padding">The padding mode.</param>
        /// <returns>A <see cref="bool"/> indicating the result of a password hash comparison</returns>
        public virtual bool VerifyEncyptedData(string data, string anotherData, RSAEncryptionPadding padding)
        {
            _validationHelpers.ThrowIfNull(data, nameof(data));
            _validationHelpers.ThrowIfNull(anotherData, nameof(anotherData));
            _validationHelpers.ThrowIfNull(padding, nameof(padding));

            var areSame = VerifyEncyptedData(data, anotherData, padding, _rsaPrivateKeyFileName);
            return areSame;
        }

        /// <summary>
        /// Returns a <see cref="bool"/> indicating the result of a encrypted data comparison.
        /// </summary>
        /// <param name="data">The encrypted data to compare.</param>
        /// <param name="anotherData">The another encrypted data to compare.</param>
        /// <param name="padding">The padding mode.</param>
        /// <param name="privateKeyFilePath">The private key file path.</param>
        /// <returns>A <see cref="bool"/> indicating the result of a password hash comparison</returns>
        public virtual bool VerifyEncyptedData(string data, string anotherData, RSAEncryptionPadding padding, string privateKeyFilePath)
        {
            _validationHelpers.ThrowIfNull(data, nameof(data));
            _validationHelpers.ThrowIfNull(anotherData, nameof(anotherData));
            _validationHelpers.ThrowIfNull(padding, nameof(padding));
            _validationHelpers.ThrowIfNull(privateKeyFilePath, nameof(privateKeyFilePath));

            byte[] bytesData = Convert.FromBase64String(data);
            byte[] bytesAnotherData = Convert.FromBase64String(anotherData);

            var areSame = VerifyEncyptedData(bytesData, bytesAnotherData, padding, privateKeyFilePath);
            return areSame;
        }

        /// <summary>
        /// Returns a <see cref="bool"/> indicating the result of a encrypted data comparison.
        /// </summary>
        /// <param name="data">The encrypted data to compare.</param>
        /// <param name="anotherData">The another encrypted data to compare.</param>
        /// <returns>A <see cref="bool"/> indicating the result of a password hash comparison</returns>
        public virtual bool VerifyEncyptedData(byte[] data, byte[] anotherData)
        {
            _validationHelpers.ThrowIfNull(data, nameof(data));
            _validationHelpers.ThrowIfNull(anotherData, nameof(anotherData));
            
            var areSame = VerifyEncyptedData(data, anotherData, RSAEncryptionPadding.OaepSHA512);
            return areSame;
        }

        /// <summary>
        /// Returns a <see cref="bool"/> indicating the result of a encrypted data comparison.
        /// </summary>
        /// <param name="data">The encrypted data to compare.</param>
        /// <param name="anotherData">The another encrypted data to compare.</param>
        /// <param name="padding">The padding mode.</param>
        /// <returns>A <see cref="bool"/> indicating the result of a password hash comparison</returns>
        public virtual bool VerifyEncyptedData(byte[] data, byte[] anotherData, RSAEncryptionPadding padding)
        {
            _validationHelpers.ThrowIfNull(data, nameof(data));
            _validationHelpers.ThrowIfNull(anotherData, nameof(anotherData));
            _validationHelpers.ThrowIfNull(padding, nameof(padding));
            
            var areSame = VerifyEncyptedData(data, anotherData, padding, _rsaPrivateKeyFileName);
            return areSame;
        }

        /// <summary>
        /// Returns a <see cref="bool"/> indicating the result of a encrypted data comparison.
        /// </summary>
        /// <param name="data">The encrypted data to compare.</param>
        /// <param name="anotherData">The another encrypted data to compare.</param>
        /// <param name="padding">The padding mode.</param>
        /// <param name="privateKeyFilePath">The private key file path.</param>
        /// <returns>A <see cref="bool"/> indicating the result of a password hash comparison</returns>
        public virtual bool VerifyEncyptedData(byte[] data, byte[] anotherData, RSAEncryptionPadding padding, string privateKeyFilePath)
        {
            _validationHelpers.ThrowIfNull(data, nameof(data));
            _validationHelpers.ThrowIfNull(anotherData, nameof(anotherData));
            _validationHelpers.ThrowIfNull(padding, nameof(padding));
            _validationHelpers.ThrowIfNull(privateKeyFilePath, nameof(privateKeyFilePath));

            var decryptedData = Decrypt(data, padding, privateKeyFilePath);
            var decryptedAnotherData = Decrypt(anotherData, padding, privateKeyFilePath);
            var areSame = ByteArraysEqual(decryptedData, decryptedAnotherData);
            return areSame;
        }

        private RSA ReadRSA(string filePath = _rsaPrivateKeyFileName)
        {
            _validationHelpers.ThrowIfNull(filePath, nameof(filePath));
            
            RSA rsa = null;
            string fileDir = Path.Combine(_commonHelpers.DefaultFileProvider.BaseDirectory, filePath);
            if ((SingletonDictionary<string, RSA>.Instance).ContainsKey(filePath))
            {
                rsa = (SingletonDictionary<string, RSA>.Instance)[filePath];
            }
            else
            {
                if (_commonHelpers.DefaultFileProvider.FileExists(fileDir))
                {
                    using (var stream = File.OpenRead(fileDir))
                    {
                        using (var reader = new PemReader(stream))
                        {
                            var rsaParameters = reader.ReadRsaKey();
                            rsa = RSA.Create(rsaParameters);
                        }
                    }
                    (SingletonDictionary<string, RSA>.Instance).Add(filePath, rsa);
                }
                else
                {
                    rsa = RSA.Create();
                    (SingletonDictionary<string, RSA>.Instance).Add(filePath, rsa);

                    using (var stream = File.OpenWrite(fileDir))
                    {
                        using (var writer = new PemWriter(stream))
                        {
                            writer.WritePrivateKey(rsa);
                        }
                    }

                    using (var stream = File.OpenWrite(Path.Combine(_commonHelpers.DefaultFileProvider.GetParentDirectory(fileDir), _rsaPublicKeyFileName)))
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

        /// <summary>
        /// Compares two byte arrays for equality. The method is specifically written so that the loop is not optimized.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }
            var areSame = true;
            for (var i = 0; i < a.Length && areSame; i++)
            {
                areSame = (a[i] == b[i]);
            }
            return areSame;
        }

        private byte[] HashPasswordV3(string password, RandomNumberGenerator rng)
        {
            return HashPasswordV3(password, rng,
                prf: KeyDerivationPrf.HMACSHA256,
                iterCount: _iterCount,
                saltSize: 128 / 8,
                numBytesRequested: 256 / 8);
        }

        private static byte[] HashPasswordV3(string password, RandomNumberGenerator rng, KeyDerivationPrf prf, int iterCount, int saltSize, int numBytesRequested)
        {
            byte[] salt = new byte[saltSize];
            rng.GetBytes(salt);
            byte[] subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);

            var outputBytes = new byte[13 + salt.Length + subkey.Length];
            outputBytes[0] = 0x01; // format marker
            WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
            WriteNetworkByteOrder(outputBytes, 5, (uint)iterCount);
            WriteNetworkByteOrder(outputBytes, 9, (uint)saltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
            return outputBytes;
        }

        private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint)(buffer[offset + 0]) << 24)
                | ((uint)(buffer[offset + 1]) << 16)
                | ((uint)(buffer[offset + 2]) << 8)
                | buffer[offset + 3];
        }

        private static bool VerifyHashedPasswordV3(byte[] hashedPassword, string password, out int iterCount)
        {
            iterCount = default(int);

            try
            {
                // Read header information
                KeyDerivationPrf prf = (KeyDerivationPrf)ReadNetworkByteOrder(hashedPassword, 1);
                iterCount = (int)ReadNetworkByteOrder(hashedPassword, 5);
                int saltLength = (int)ReadNetworkByteOrder(hashedPassword, 9);

                // Read the salt: must be >= 128 bits
                if (saltLength < 128 / 8)
                {
                    return false;
                }
                byte[] salt = new byte[saltLength];
                Buffer.BlockCopy(hashedPassword, 13, salt, 0, salt.Length);

                // Read the subkey (the rest of the payload): must be >= 128 bits
                int subkeyLength = hashedPassword.Length - 13 - salt.Length;
                if (subkeyLength < 128 / 8)
                {
                    return false;
                }
                byte[] expectedSubkey = new byte[subkeyLength];
                Buffer.BlockCopy(hashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

                // Hash the incoming password and verify it
                byte[] actualSubkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, subkeyLength);
                return ByteArraysEqual(actualSubkey, expectedSubkey);
            }
            catch
            {
                // This should never occur except in the case of a malformed payload, where
                // we might go off the end of the array. Regardless, a malformed payload
                // implies verification failed.
                return false;
            }
        }

        private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
        }
    }
}
