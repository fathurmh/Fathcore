using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Fathcore.Extensions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;

namespace Fathcore.Security.Cryptography
{
    /// <summary>
    /// Represents hash factory.
    /// </summary>
    public class HashFactory : IHashFactory
    {
        private readonly int _iterCount;
        private readonly RandomNumberGenerator _rng;

        public HashFactory()
        {
            var passwordHasherOptions = new PasswordHasherOptions();
            _iterCount = passwordHasherOptions.IterationCount;
            if (_iterCount < 1)
                throw new InvalidOperationException();

            _rng = RandomNumberGenerator.Create();
        }

        /// <summary>
        /// Hash data using the PBKDF2 algorithm. 
        /// </summary>
        /// <param name="data">Data being hashed.</param>
        /// <returns>Hashed data.</returns>
        public string Pbkdf2Hash(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException(nameof(data));

            return Convert.ToBase64String(HashPasswordV3(data, _rng));
        }

        /// <summary>
        /// Verify specified data with hashed data.
        /// </summary>
        /// <param name="data">Specified password.</param>
        /// <param name="hashedData">Hashed password.</param>
        /// <returns><see cref="VerificationStatus"/>.</returns>
        public VerificationStatus Pbkdf2Verify(string data, string hashedData)
        {
            if (string.IsNullOrWhiteSpace(data))
                throw new ArgumentNullException(nameof(data));

            if (string.IsNullOrWhiteSpace(hashedData))
                throw new ArgumentNullException(nameof(hashedData));

            if (data.Equals(hashedData, StringComparison.Ordinal))
                return VerificationStatus.SuccessRehashNeeded;

            if (hashedData.IsBase64String())
            {
                byte[] decodedHashedPassword = Convert.FromBase64String(hashedData);

                if (decodedHashedPassword.Length == 0)
                    return VerificationStatus.Failed;

                if (decodedHashedPassword[0] == 0x01)
                {
                    if (VerifyHashedPasswordV3(decodedHashedPassword, data, out int embeddedIterCount))
                        return (embeddedIterCount < _iterCount) ? VerificationStatus.SuccessRehashNeeded : VerificationStatus.Success;
                }

                return VerificationStatus.Failed;
            }

            return VerificationStatus.Failed;
        }

        /// <summary>
        /// Hash data using the MD5 algorithm. 
        /// </summary>
        /// <param name="data">Data being hashed.</param>
        /// <returns>Hashed data.</returns>
        public string Md5Hash(byte[] data)
        {
            using (var md5 = MD5.Create())
            {
                var sb = new StringBuilder();

                byte[] retVal = md5.ComputeHash(data);
                for (int i = 0; i < retVal.Length; i++)
                    sb.Append(retVal[i].ToString("x2"));

                return sb.ToString();
            }
        }

        /// <summary>
        /// Hash data using the MD5 algorithm. 
        /// </summary>
        /// <param name="data">Data being hashed.</param>
        /// <returns>Hashed data.</returns>
        public string Md5Hash(string data)
        {
            return Md5Hash(Encoding.Unicode.GetBytes(data));
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null)
                return true;

            if (a == null || b == null || a.Length != b.Length)
                return false;

            var areSame = true;
            for (var i = 0; i < a.Length && areSame; i++)
                areSame = (a[i] == b[i]);

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

        private byte[] HashPasswordV3(string password, RandomNumberGenerator rng, KeyDerivationPrf prf, int iterCount, int saltSize, int numBytesRequested)
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

        private uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint)(buffer[offset + 0]) << 24)
                | ((uint)(buffer[offset + 1]) << 16)
                | ((uint)(buffer[offset + 2]) << 8)
                | buffer[offset + 3];
        }

        private bool VerifyHashedPasswordV3(byte[] hashedPassword, string password, out int iterCount)
        {
            iterCount = default;

            try
            {
                var prf = (KeyDerivationPrf)ReadNetworkByteOrder(hashedPassword, 1);
                iterCount = (int)ReadNetworkByteOrder(hashedPassword, 5);
                int saltLength = (int)ReadNetworkByteOrder(hashedPassword, 9);

                if (saltLength < 128 / 8)
                {
                    return false;
                }
                byte[] salt = new byte[saltLength];
                Buffer.BlockCopy(hashedPassword, 13, salt, 0, salt.Length);

                int subkeyLength = hashedPassword.Length - 13 - salt.Length;
                if (subkeyLength < 128 / 8)
                {
                    return false;
                }
                byte[] expectedSubkey = new byte[subkeyLength];
                Buffer.BlockCopy(hashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

                byte[] actualSubkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, subkeyLength);
                return ByteArraysEqual(actualSubkey, expectedSubkey);
            }
            catch
            {
                return false;
            }
        }

        private void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
        }
    }
}
