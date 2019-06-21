using System.Collections.Generic;

namespace Fathcore.Infrastructure.ResponseWrapper
{
    /// <summary>
    /// Represents response exception.
    /// </summary>
    public interface IResponseException
    {
        /// <summary>
        /// Gets a status code value.
        /// </summary>
        int StatusCode { get; }

        /// <summary>
        /// Gets error type value.
        /// </summary>
        string ErrorType { get; }

        /// <summary>
        /// Gets error message value.
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// Gets details value.
        /// </summary>
        string Details { get; }

        /// <summary>
        /// Gets validation errors value.
        /// </summary>
        IEnumerable<IModelValidation> ValidationErrors { get; }
    }
}
