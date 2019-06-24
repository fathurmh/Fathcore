namespace Fathcore.Infrastructure.Localization.Resources
{
    /// <summary>
    /// Represents validation message
    /// </summary>
    public sealed class ValidationMessage
    {
        /// <summary>
        /// The default value used for argument null validation message.
        /// <para>
        /// Message : The {0} field cannot be null.
        /// </para>
        /// </summary>
        public const string ArgumentNull = "ArgumentNull";

        /// <summary>
        /// The default value used for not found validation message.
        /// <para>
        /// Message : The {0} not found.
        /// </para>
        /// </summary>
        public const string NotFound = "NotFound";

        /// <summary>
        /// The default value used for unauthorized validation message.
        /// <para>
        /// Message : Request denied.
        /// </para>
        /// </summary>
        public const string Unauthorized = "Unauthorized";

        /// <summary>
        /// The default value used for does not match validation message.
        /// <para>
        /// Message : The {0} and {1} doesn't match.
        /// </para>
        /// </summary>
        public const string DoesNotMatch = "DoesNotMatch";

        /// <summary>
        /// The default value used for not found or does not match validation message.
        /// <para>
        /// Message : The {0} not found or {1} and {2} doesn't match.
        /// </para>
        /// </summary>
        public const string NotFoundOrDoesNotMatch = "NotFoundOrDoesNotMatch";

        /// <summary>
        /// The default value used for not recognized validation message.
        /// <para>
        /// Message : The {0} not recognized.
        /// </para>
        /// </summary>
        public const string NotRecognized = "NotRecognized";

        /// <summary>
        /// The default value used for already exist validation message.
        /// <para>
        /// Message : {0} is already exist.
        /// </para>
        /// </summary>
        public const string AlreadyExist = "AlreadyExist";

        /// <summary>
        /// The default value used for required message.
        /// <para>
        /// Message: The {0} field is required.
        /// </para>
        /// </summary>
        public const string Required = "Required";

        /// <summary>
        /// The default value used for minimum length message.
        /// <para>
        /// Message: The length of '{0}' must be at least {1} characters. You entered {2} characters.
        /// </para>
        /// </summary>
        public const string MinimumLength = "MinimumLength";


    }
}
