namespace Fathcore.Security.SecurityHeaders
{
    /// <summary>
    /// Represents security headers defaults.
    /// </summary>
    public static class SecurityHeadersDefaults
    {
        /// <summary>
        /// The header key for X-Powered-By.
        /// </summary>
        public static string XPoweredByKey => "X-Powered-By";

        /// <summary>
        /// The header key for Server.
        /// </summary>
        public static string ServerKey => "Server";

        /// <summary>
        /// Header key for Content-Security-Policy.
        /// </summary>
        public static string ContentSecurityPolicyKey => "Content-Security-Policy";

        /// <summary>
        /// Header key for Feature-Policy.
        /// </summary>
        public static string FeaturePolicyKey => "Feature-Policy";

        /// <summary>
        /// Represents X-Frame-Options defaults.
        /// </summary>
        public static class XFrameOptions
        {
            /// <summary>
            /// The header key for X-Frame-Options.
            /// </summary>
            public static string Key => "X-Frame-Options";

            /// <summary>
            /// The page cannot be displayed in a frame, regardless of the site attempting to do so.
            /// </summary>
            public static string Deny => "DENY";

            /// <summary>
            /// The page can only be displayed in a frame on the same origin as the page itself.
            /// </summary>
            public static string SameOrigin => "SAMEORIGIN";

            /// <summary>
            /// The page can only be displayed in a frame on the specified origin.
            /// <para>
            /// {0} : Specifies the format string.
            /// </para>
            /// </summary>
            public static string AllowFromUri => "ALLOW-FROM {0}";
        }

        /// <summary>
        /// Represents X-XSS-Protection defaults.
        /// </summary>
        public static class XXssProtection
        {
            /// <summary>
            /// Header key for X-XSS-Protection.
            /// </summary>
            public static string Key => "X-XSS-Protection";

            /// <summary>
            /// Enables the XSS Protections.
            /// </summary>
            public static string Enabled => "1";

            /// <summary>
            /// Disables the XSS Protections offered by the user-agent.
            /// </summary>
            public static string Disabled => "0";

            /// <summary>
            /// Enables XSS protections and instructs the user-agent to block the response in the event that script has been inserted from user input, instead of sanitizing.
            /// </summary>
            public static string Block => "1; mode=block";

            /// <summary>
            /// A partially supported directive that tells the user-agent to report potential XSS attacks to a single URL. Data will be POST'd to the report URL in JSON format. 
            /// <para>
            /// {0} : Specifies the report url, including protocol
            /// </para>
            /// </summary>
            public static string Report => "1; report={0}";
        }

        /// <summary>
        /// Represents X-Content-Type-Options defaults.
        /// </summary>
        public static class XContentTypeOptions
        {
            /// <summary>
            /// Header key for X-Content-Type-Options.
            /// </summary>
            public static string Key => "X-Content-Type-Options";

            /// <summary>
            /// Disables content sniffing.
            /// </summary>
            public static string NoSniff => "nosniff";
        }

        /// <summary>
        /// Represents Strict-Transport-Security defaults.
        /// </summary>
        public static class StrictTransportSecurity
        {
            /// <summary>
            /// Header key for Strict-Transport-Security.
            /// </summary>
            public static string Key => "Strict-Transport-Security";

            /// <summary>
            /// Tells the user-agent to cache the domain in the STS list.
            /// <para>
            /// {0} : Provided number of seconds.
            /// </para>
            /// </summary>
            public static string MaxAge => "max-age={0}";

            /// <summary>
            /// Tells the user-agent to cache the domain in the STS list include any sub-domains.
            /// <para>
            /// {0} : Provided number of seconds.
            /// </para>
            /// </summary>
            public static string MaxAgeIncludeSubdomains => "max-age={0}; includeSubDomains";

            /// <summary>
            /// Tells the user-agent to remove, or not cache the host in the STS cache.
            /// </summary>
            public static string NoCache => "max-age=0";
        }

        /// <summary>
        /// Represents Referrer-Policy defaults.
        /// </summary>
        public static class ReferrerPolicy
        {
            /// <summary>
            /// Header key for Referrer-Policy.
            /// </summary>
            public static string Key => "Referrer-Policy";

            /// <summary>
            /// The no-referrer value instructs the browser to never send the referer header with requests that are made from your site.
            /// This also include links to pages on your own site.
            /// </summary>
            public static string NoReferrer => "no-referrer";

            /// <summary>
            /// The browser will not send the referrer header when navigating from HTTPS to HTTP, but will always send the full URL in the referrer header when navigating from HTTP to any origin.
            /// It doesn't matter whether the source and destination are the same site or not, only the scheme.
            /// </summary>
            public static string NoReferrerWhenDowngrade => "no-referrer-when-downgrade";

            /// <summary>
            /// The browser will only set the referrer header on requests to the same origin. If the destination is another origin then no referrer information will be sent.
            /// </summary>
            public static string SameOrigin => "same-origin";

            /// <summary>
            /// The browser will always set the referrer header to the origin from which the request was made.This will strip any path information from the referrer information.
            /// </summary>
            public static string Origin => "origin";

            /// <summary>
            /// This value is similar to <see cref="Origin"/> but will not allow the secure origin to be sent on a HTTP request, only HTTPS.
            /// </summary>
            public static string StrictOrigin => "strict-origin";

            /// <summary>
            /// The browser will send the full URL to requests to the same origin but only send the origin when requests are cross-origin.
            /// </summary>
            public static string OriginWhenCrossOrigin => "origin-when-cross-origin";

            /// <summary>
            /// Similar to <see cref="OriginWhenCrossOrigin"/> but will not allow any information to be sent when a scheme downgrade happens (the user is navigating from HTTPS to HTTP).
            /// </summary>
            public static string StrictOriginWhenCrossOrigin => "strict-origin-when-cross-origin";

            /// <summary>
            /// The browser will always send the full URL with any request to any origin.
            /// </summary>
            public static string UnsafeUrl => "unsafe-url";
        }
    }
}
