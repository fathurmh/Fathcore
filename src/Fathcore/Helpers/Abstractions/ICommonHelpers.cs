using Fathcore.Abstractions;
using Fathcore.Providers.Abstractions;

namespace Fathcore.Helpers.Abstractions
{
    /// <summary>
    /// Represents a common helpers
    /// </summary>
    public interface ICommonHelpers : ISingletonService
    {
        
        /// <summary>
        /// Gets or sets the default file provider
        /// </summary>
        ICoreFileProvider DefaultFileProvider { get; }

        /// <summary>
        /// Verifies that a string is endoded format
        /// </summary>
        /// <param name="providedString">String to verify</param>
        /// <returns>Returns true if the string is endoded and false if it's not</returns>
        bool IsBase64Encoded(string providedString);
    }
}
