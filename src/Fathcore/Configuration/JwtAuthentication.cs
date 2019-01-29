using System;

namespace Fathcore.Configuration
{
    /// <summary>
    /// Represents a jwt authentication
    /// </summary>
    public class JwtAuthentication
    {
        /// <summary>
        /// Gets or sets the value of the 'audience' claim
        /// </summary>
        /// <value></value>
        public string Audience { get; set; }
        
        /// <summary>
        /// Gets or sets the value of the 'expiration' claim in minutes
        /// </summary>
        /// <value></value>
        public double Expires { get; set; }
        
        /// <summary>
        /// Gets or sets the issuer of this Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
        /// </summary>
        /// <value></value>
        public string Issuer { get; set; }
        
        /// <summary>
        /// Gets or sets the time the security token was issued
        /// </summary>
        /// <value></value>
        public DateTime? IssuedAt { get; set; }

        /// <summary>
        /// Gets or sets the jwt authentication key
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// Gets or sets the notbefore time for the security token
        /// </summary>
        /// <value></value>
        public DateTime? NotBefore { get; set; }
    }
}
