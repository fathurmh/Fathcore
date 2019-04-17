using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Fathcore.Extensions
{
    /// <summary>
    /// Extension methods for String.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Verifies that a string is endoded format.
        /// </summary>
        /// <param name="source">Specified string being verify.</param>
        /// <returns>Returns true if the string is encoded and false if it's not.</returns>
        public static bool IsBase64String(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return false;

            source = source.Trim();

            return (source.Length % 4 == 0) && Regex.IsMatch(source, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.Compiled | RegexOptions.Singleline);
        }

        /// <summary>
        /// Verifies that a string is in valid e-mail format.
        /// </summary>
        /// <param name="source">Specified string being verify.</param>
        /// <returns>true if the string is a valid e-mail address and false if it's not.</returns>
        public static bool IsValidEmail(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return false;

            source = source.Trim();

            return Regex.IsMatch(source, @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-||_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+([a-z]+|\d|-|\.{0,1}|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])?([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$",
                RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Verifies that a string is in valid json format.
        /// </summary>
        /// <param name="source">Specified string being verify.</param>
        /// <returns>true if the string is a valid json format and false if it's not</returns>
        public static bool IsValidJson(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return false;

            source = source.Trim();
            if ((source.StartsWith("{") && source.EndsWith("}")) || //For object
                (source.StartsWith("[") && source.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(source);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Converts the first character of a Unicode character to its uppercase equivalent.
        /// </summary>
        /// <param name="source">The string whose the first character value will be converted.</param>
        /// <returns>The uppercase equivalent of the current fisrt character.</returns>
        public static string FirstLetterToUpper(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            return source.Length > 1 ? char.ToUpper(source[0]) + source.Substring(1) : source.ToUpper();
        }

        /// <summary>
        /// Parse string to enum.
        /// </summary>
        /// <typeparam name="T">Type of enum.</typeparam>
        /// <param name="source">String value being converted.</param>
        /// <param name="defaultValue">Default value if string cannot be converted.</param>
        /// <returns>Enum.</returns>
        public static T ToEnum<T>(this string source, T defaultValue) where T : struct, Enum
        {
            if (string.IsNullOrEmpty(source))
                return defaultValue;

            return Enum.TryParse(source, true, out T result) ? result : defaultValue;
        }

        /// <summary>
        /// Remove whitespace.
        /// </summary>
        /// <param name="source">String value being whitespace removed.</param>
        /// <returns></returns>
        public static string RemoveWhitespace(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            return Regex.Replace(source, @"\s+", string.Empty);
        }
    }
}
