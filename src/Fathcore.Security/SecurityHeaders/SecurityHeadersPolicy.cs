using System.Collections.Generic;

namespace Fathcore.Security.SecurityHeaders
{
    /// <summary>
    /// Represents security headers policy.
    /// </summary>
    public sealed class SecurityHeadersPolicy
    {
        /// <summary>
        /// Set security headers.
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> SetHeaders { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Remove security headers.
        /// </summary>
        /// <returns></returns>
        public ISet<string> RemoveHeaders { get; } = new HashSet<string>();
    }
}
