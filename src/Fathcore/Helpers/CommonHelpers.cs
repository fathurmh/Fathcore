using System.Text.RegularExpressions;
using Fathcore.Helpers.Abstractions;
using Fathcore.Providers.Abstractions;

namespace Fathcore.Helpers
{
    /// <summary>
    /// Represents a common helpers
    /// </summary>
    public class CommonHelpers : ICommonHelpers
    {
        private const string _encodedPattern = @"^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$";

        private static readonly Regex _encodedRegex;
        
        /// <summary>
        /// Gets or sets the default file provider
        /// </summary>
        public static ICoreFileProvider DefaultFileProvider { get; set; }

        static CommonHelpers()
        {
            _encodedRegex = new Regex(_encodedPattern);
        }

        /// <summary>
        /// Verifies that a string is endoded format
        /// </summary>
        /// <param name="providedString">String to verify</param>
        /// <returns>Returns true if the string is endoded and false if it's not</returns>
        public virtual bool IsBase64Encoded(string providedString)
        {
            if (string.IsNullOrEmpty(providedString))
                return false;

            return (providedString.Length % 4 == 0) && _encodedRegex.IsMatch(providedString);
        }
    }
}
