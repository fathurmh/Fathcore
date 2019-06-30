using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fathcore.Infrastructure.Helpers
{
    public partial interface IHelper
    {
        /// <summary>
        /// Generate random string.
        /// </summary>
        /// <param name="length">String length being generated.</param>
        /// <returns>Generated string.</returns>
        string GenerateRandomString(int length);

        /// <summary>
        /// Generate random digit code.
        /// </summary>
        /// <param name="length">Length of digit.</param>
        /// <returns>Result string.</returns>
        string GenerateRandomDigitCode(int length);

        /// <summary>
        /// Returns an random integer number within a specified rage.
        /// </summary>
        /// <param name="min">Minimum number.</param>
        /// <param name="max">Maximum number.</param>
        /// <returns>Result.</returns>
        int GenerateRandomInteger(int min = 0, int max = int.MaxValue);

        /// <summary>
        /// Ensure that a string doesn't exceed maximum allowed length.
        /// </summary>
        /// <param name="str">Input string.</param>
        /// <param name="maxLength">Maximum length.</param>
        /// <param name="postfix">A string to add to the end if the original string was shorten.</param>
        /// <returns>Input string if its length is OK; otherwise, truncated input string.</returns>
        string EnsureMaximumLength(string str, int maxLength, string postfix = null);

        /// <summary>
        /// Ensures that a string only contains numeric values.
        /// </summary>
        /// <param name="str">Input string.</param>
        /// <returns>Input string with only numeric values, empty string if input is null/empty.</returns>
        string EnsureNumericOnly(string str);

        /// <summary>
        /// Ensure that a string is not null.
        /// </summary>
        /// <param name="str">Input string.</param>
        /// <returns>Result.</returns>
        string EnsureNotNull(string str);

        /// <summary>
        /// Compare two arrays.
        /// </summary>
        /// <typeparam name="T">Type of array.</typeparam>
        /// <param name="a1">Array 1.</param>
        /// <param name="a2">Array 2.</param>
        /// <returns>Result.</returns>
        bool ArraysEqual<T>(T[] a1, T[] a2);

        /// <summary>
        /// Get difference in years.
        /// </summary>
        /// <param name="startDate">Start date.</param>
        /// <param name="endDate">End date.</param>
        /// <returns></returns>
        int GetDifferenceInYears(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get text within quotation marks.
        /// </summary>
        /// <param name="text">Input string.</param>
        /// <returns>String collection.</returns>
        IEnumerable<string> GetTextWithinQuotes(string text);

        /// <summary>
        /// Remove special character from string.
        /// </summary>
        /// <param name="text">The given string.</param>
        /// <param name="except">Except this character.</param>
        /// <returns>String without special character.</returns>
        string RemoveSpecialCharacters(string text, params char[] except);

        /// <summary>
        /// Executes an async <see cref="Task{T}"/> method which has a void return value synchronously.
        /// </summary>
        /// <param name="task"><see cref="Task{T}"/> method to execute.</param>
        void RunSync(Func<Task> task);

        /// <summary>
        /// Execute's an async <see cref="Task{T}"/> method which has a T return type synchronously.
        /// </summary>
        /// <typeparam name="T">Return Type.</typeparam>
        /// <param name="task"><see cref="Task{T}"/> method to execute.</param>
        /// <returns></returns>
        T RunSync<T>(Func<Task<T>> task);
    }
}
