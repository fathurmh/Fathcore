namespace Fathcore.Localization.Resources
{
    /// <summary>
    /// Represents an api response message
    /// </summary>
    public sealed class ApiResponseMessage
    {
        /// <summary>
        /// Gets the success api response message
        /// </summary>
        public const string Success = nameof(Success);

        /// <summary>
        /// Gets the exception api response message
        /// </summary>
        public const string Exception = nameof(Exception);

        /// <summary>
        /// Gets the unauthorized api response message
        /// </summary>
        public const string Unauthorized = nameof(Unauthorized);

        /// <summary>
        /// Gets the validation error api response message
        /// </summary>
        public const string ValidationError = nameof(ValidationError);

        /// <summary>
        /// Gets the failure api response message
        /// </summary>
        public const string Failure = nameof(Failure);

        /// <summary>
        /// Gets the not found api response message
        /// </summary>
        public const string NotFound = nameof(NotFound);

        /// <summary>
        /// Gets the contact support api response message
        /// </summary>
        public const string ContactSupport = nameof(ContactSupport);

        /// <summary>
        /// Gets the validation exception api response message
        /// </summary>
        public const string ValidationException = nameof(ValidationException);

        /// <summary>
        /// Gets the unhandled exception api response message
        /// </summary>
        public const string Unhandled = nameof(Unhandled);
    }
}
