namespace Fathcore.Infrastructure.Enum
{
    /// <summary>
    /// Provides the interface for a strongly typed enumerations.
    /// </summary>
    public interface IErrorType : ITypeSafeEnum
    {
        /// <summary>
        /// Gets a status code value.
        /// </summary>
        int StatusCode { get; }
    }
}
