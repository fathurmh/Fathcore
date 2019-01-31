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

        private readonly Regex _encodedRegex;
        private readonly ICoreFileProvider _coreFileProvider;
        
        /// <summary>
        /// Gets or sets the default file provider
        /// </summary>
        public ICoreFileProvider DefaultFileProvider => _coreFileProvider;

        public CommonHelpers(ICoreFileProvider coreFileProvider)
        {
            _encodedRegex = new Regex(_encodedPattern);
            _coreFileProvider = coreFileProvider;
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
