namespace Fathcore.Providers
{
    /// <summary>
    /// Represents a password verification status enum
    /// </summary>
    public enum PasswordVerificationStatus
    {
        /// <summary>
        /// Represents a password verification was failed
        /// </summary>
        Failed = 0,

        /// <summary>
        /// Represents a password verification was success
        /// </summary>
        Success = 1,

        /// <summary>
        /// Represents a password verification was success but rehash is needed
        /// </summary>
        SuccessRehashNeeded = 2
    }
}
