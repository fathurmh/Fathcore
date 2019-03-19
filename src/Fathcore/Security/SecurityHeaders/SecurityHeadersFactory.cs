using System;

namespace Fathcore.Security.SecurityHeaders
{
    /// <summary>
    /// Exposes methods to build a policy.
    /// </summary>
    public class SecurityHeadersFactory
    {
        private readonly SecurityHeadersPolicy _policy = new SecurityHeadersPolicy();

        /// <summary>
        /// The number of seconds in one year.
        /// </summary>
        public const int OneYearInSeconds = 60 * 60 * 24 * 365;

        /// <summary>
        /// Add default headers in accordance with most secure approach.
        /// </summary>
        public SecurityHeadersFactory AddDefaultSecurePolicy()
        {
            AddFrameOptionsDeny();
            AddXssProtectionBlock();
            AddContentTypeOptionsNoSniff();
            AddStrictTransportSecurityMaxAge();
            AddContentSecurityPolicy("default-src 'self'");
            AddReferrerPolicy(SecurityHeadersDefaults.ReferrerPolicy.SameOrigin);
            AddFeaturePolicy("vibrate 'self'; sync-xhr 'self'");
            RemoveServerHeader();
            RemoveXPoweredByHeader();

            return this;
        }

        /// <summary>
        /// Add X-Frame-Options DENY to all requests.
        /// The page cannot be displayed in a frame, regardless of the site attempting to do so.
        /// </summary>
        public SecurityHeadersFactory AddFrameOptionsDeny()
        {
            _policy.SetHeaders[SecurityHeadersDefaults.XFrameOptions.Key] = SecurityHeadersDefaults.XFrameOptions.Deny;
            return this;
        }

        /// <summary>
        /// Add X-XSS-Protection 1; mode=block to all requests.
        /// Enables XSS protections and instructs the user-agent to block the response in the event that script has been inserted from user input, instead of sanitizing.
        /// </summary>
        public SecurityHeadersFactory AddXssProtectionBlock()
        {
            _policy.SetHeaders[SecurityHeadersDefaults.XXssProtection.Key] = SecurityHeadersDefaults.XXssProtection.Block;
            return this;
        }

        /// <summary>
        /// Add Strict-Transport-Security max-age=<see cref="OneYearInSeconds"/> to all requests.
        /// Tells the user-agent to cache the domain in the STS list for the number of seconds provided.
        /// </summary>
        /// <param name="maxAge"></param>
        /// <returns></returns>
        public SecurityHeadersFactory AddStrictTransportSecurityMaxAge(int maxAge = OneYearInSeconds)
        {
            _policy.SetHeaders[SecurityHeadersDefaults.StrictTransportSecurity.Key] = string.Format(SecurityHeadersDefaults.StrictTransportSecurity.MaxAge, maxAge);
            return this;
        }

        /// <summary>
        /// Add X-Content-Type-Options nosniff to all requests.
        /// Can be set to protect against MIME type confusion attacks.
        /// </summary>
        public SecurityHeadersFactory AddContentTypeOptionsNoSniff()
        {
            _policy.SetHeaders[SecurityHeadersDefaults.XContentTypeOptions.Key] = SecurityHeadersDefaults.XContentTypeOptions.NoSniff;
            return this;
        }

        /// <summary>
        /// Add Referrer-Policy to all requests.
        /// </summary>
        /// <returns></returns>
        public SecurityHeadersFactory AddReferrerPolicy(string policy)
        {
            _policy.SetHeaders[SecurityHeadersDefaults.ReferrerPolicy.Key] = policy;
            return this;
        }

        /// <summary>
        /// Add Feature-Policy to all requests.
        /// </summary>
        /// <returns></returns>
        public SecurityHeadersFactory AddFeaturePolicy(string policy)
        {
            _policy.SetHeaders[SecurityHeadersDefaults.FeaturePolicyKey] = policy;
            return this;
        }

        /// <summary>
        /// Add Content-Security-Policy to all requests.
        /// </summary>
        /// <returns></returns>
        public SecurityHeadersFactory AddContentSecurityPolicy(string policy)
        {
            _policy.SetHeaders[SecurityHeadersDefaults.ContentSecurityPolicyKey] = policy;
            return this;
        }

        /// <summary>
        /// Removes the Server header from all responses.
        /// </summary>
        public SecurityHeadersFactory RemoveServerHeader()
        {
            _policy.RemoveHeaders.Add(SecurityHeadersDefaults.ServerKey);
            return this;
        }

        /// <summary>
        /// Removes the X-Powered-By header from all responses.
        /// </summary>
        public SecurityHeadersFactory RemoveXPoweredByHeader()
        {
            _policy.RemoveHeaders.Add(SecurityHeadersDefaults.XPoweredByKey);
            return this;
        }

        /// <summary>
        /// Adds a custom header to all requests.
        /// </summary>
        /// <param name="header">The header name.</param>
        /// <param name="value">The value for the header.</param>
        /// <returns></returns>
        public SecurityHeadersFactory AddCustomHeader(string header, string value)
        {
            if (string.IsNullOrEmpty(header))
                throw new ArgumentNullException(nameof(header));

            _policy.SetHeaders[header] = value;
            return this;
        }

        /// <summary>
        /// Remove a header from all requests.
        /// </summary>
        /// <param name="header">The to remove</param>
        /// <returns></returns>
        public SecurityHeadersFactory RemoveHeader(string header)
        {
            if (string.IsNullOrEmpty(header))
                throw new ArgumentNullException(nameof(header));

            _policy.RemoveHeaders.Add(header);
            return this;
        }

        /// <summary>
        /// Builds a new <see cref="SecurityHeadersPolicy"/> using the entries added.
        /// </summary>
        /// <returns>The constructed <see cref="SecurityHeadersPolicy"/>.</returns>
        public SecurityHeadersPolicy Build()
        {
            return _policy;
        }
    }
}
