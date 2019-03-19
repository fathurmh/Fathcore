namespace Fathcore.Security.Cryptography
{
    /// <summary>
    /// Represents verification status.
    /// </summary>
    public enum VerificationStatus
    {
        /// <summary>
        /// Indicates verification failed.
        /// </summary>
        Failed = 1,

        /// <summary>
        /// Indicates verification was successful.
        /// </summary>
        Success = 2,

        /// <summary>
        /// Indicates verification was successful however the data was encoded using a deprecated algorithm and should be rehashed and updated.
        /// </summary>
        SuccessRehashNeeded = 4
    }
}