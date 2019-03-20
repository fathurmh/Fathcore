using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Fathcore.Infrastructure;

namespace Fathcore
{
    /// <summary>
    /// Represents a helper.
    /// </summary>
    public class Helper
    {
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const string _textDoubleQuotedPattern = "(\"(?:[^\"]+|\"\")*\"|\\\".*?\\\"*)";

        private static readonly Random s_random;
        private static readonly Regex s_extractTextDoubleQuoted;

        /// <summary>
        /// Gets or sets the default file provider.
        /// </summary>
        public static IFileProvider DefaultFileProvider { get; set; }

        static Helper()
        {
            s_random = new Random();
            s_extractTextDoubleQuoted = new Regex(_textDoubleQuotedPattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// Generate random string.
        /// </summary>
        /// <param name="length">String length being generated.</param>
        /// <returns>Generated string.</returns>
        public static string GenerateRandomString(int length)
        {
            return new string(Enumerable.Repeat(_chars, length)
              .Select(s => s[s_random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Generate random digit code.
        /// </summary>
        /// <param name="length">Length of digit.</param>
        /// <returns>Result string.</returns>
        public static string GenerateRandomDigitCode(int length)
        {
            var random = new Random();
            var str = string.Empty;
            for (var i = 0; i < length; i++)
                str = string.Concat(str, random.Next(10).ToString());

            return str;
        }

        /// <summary>
        /// Returns an random integer number within a specified rage.
        /// </summary>
        /// <param name="min">Minimum number.</param>
        /// <param name="max">Maximum number.</param>
        /// <returns>Result.</returns>
        public static int GenerateRandomInteger(int min = 0, int max = int.MaxValue)
        {
            var randomNumberBuffer = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);

            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }

        /// <summary>
        /// Ensure that a string doesn't exceed maximum allowed length.
        /// </summary>
        /// <param name="str">Input string.</param>
        /// <param name="maxLength">Maximum length.</param>
        /// <param name="postfix">A string to add to the end if the original string was shorten.</param>
        /// <returns>Input string if its length is OK; otherwise, truncated input string.</returns>
        public static string EnsureMaximumLength(string str, int maxLength, string postfix = null)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            if (str.Length <= maxLength)
                return str;

            var pLen = postfix?.Length ?? 0;

            var result = str.Substring(0, maxLength - pLen);
            if (!string.IsNullOrEmpty(postfix))
            {
                result += postfix;
            }

            return result;
        }

        /// <summary>
        /// Ensures that a string only contains numeric values.
        /// </summary>
        /// <param name="str">Input string.</param>
        /// <returns>Input string with only numeric values, empty string if input is null/empty.</returns>
        public static string EnsureNumericOnly(string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : new string(str.Where(char.IsDigit).ToArray());
        }

        /// <summary>
        /// Ensure that a string is not null.
        /// </summary>
        /// <param name="str">Input string.</param>
        /// <returns>Result.</returns>
        public static string EnsureNotNull(string str)
        {
            return str ?? string.Empty;
        }

        /// <summary>
        /// Compare two arrays.
        /// </summary>
        /// <typeparam name="T">Type of array.</typeparam>
        /// <param name="a1">Array 1.</param>
        /// <param name="a2">Array 2.</param>
        /// <returns>Result.</returns>
        public static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            //also see Enumerable.SequenceEqual(a1, a2);
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            var comparer = EqualityComparer<T>.Default;

            return !a1.Where((t, i) => !comparer.Equals(t, a2[i])).Any();
        }

        /// <summary>
        /// Get difference in years.
        /// </summary>
        /// <param name="startDate">Start date.</param>
        /// <param name="endDate">End date.</param>
        /// <returns></returns>
        public static int GetDifferenceInYears(DateTime startDate, DateTime endDate)
        {
            //source: http://stackoverflow.com/questions/9/how-do-i-calculate-someones-age-in-c
            //this assumes you are looking for the western idea of age and not using East Asian reckoning.
            var age = endDate.Year - startDate.Year;
            if (startDate > endDate.AddYears(-age))
                age--;

            return age;
        }

        /// <summary>
        /// Get text within quotation marks.
        /// </summary>
        /// <param name="text">Input string.</param>
        /// <returns>String collection.</returns>
        public static IEnumerable<string> GetTextWithinQuotes(string text)
        {
            return s_extractTextDoubleQuoted.Matches(text).Select(p => p.ToString());
        }

        /// <summary>
        /// Remove special character from string.
        /// </summary>
        /// <param name="text">The given string.</param>
        /// <param name="except">Except this character.</param>
        /// <returns>String without special character.</returns>
        public static string RemoveSpecialCharacters(string text, params char[] except)
        {
            var sb = new StringBuilder();
            foreach (char c in text)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || except.Contains(c))
                    sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Executes an async <see cref="Task{T}"/> method which has a void return value synchronously.
        /// </summary>
        /// <param name="task"><see cref="Task{T}"/> method to execute.</param>
        public static void RunSync(Func<Task> task)
        {
            var oldSynchronizationContext = SynchronizationContext.Current;
            var synchronizationContext = new ExclusiveSynchronizationContext();

            SynchronizationContext.SetSynchronizationContext(synchronizationContext);
            synchronizationContext.Post(async _ =>
            {
                try
                {
                    await task();
                }
                catch (Exception e)
                {
                    synchronizationContext.InnerException = e;
                    throw;
                }
                finally
                {
                    synchronizationContext.EndMessageLoop();
                }
            }, null);

            synchronizationContext.BeginMessageLoop();
            SynchronizationContext.SetSynchronizationContext(oldSynchronizationContext);
        }

        /// <summary>
        /// Execute's an async <see cref="Task{T}"/> method which has a T return type synchronously.
        /// </summary>
        /// <typeparam name="T">Return Type.</typeparam>
        /// <param name="task"><see cref="Task{T}"/> method to execute.</param>
        /// <returns></returns>
        public static T RunSync<T>(Func<Task<T>> task)
        {
            var oldSynchronizationContext = SynchronizationContext.Current;
            var synchronizationContext = new ExclusiveSynchronizationContext();

            T result = default;
            SynchronizationContext.SetSynchronizationContext(synchronizationContext);
            synchronizationContext.Post(async _ =>
            {
                try
                {
                    result = await task();
                }
                catch (Exception e)
                {
                    synchronizationContext.InnerException = e;
                    throw;
                }
                finally
                {
                    synchronizationContext.EndMessageLoop();
                }
            }, null);

            synchronizationContext.BeginMessageLoop();
            SynchronizationContext.SetSynchronizationContext(oldSynchronizationContext);
            return result;
        }
    }
}
