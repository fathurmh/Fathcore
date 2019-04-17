using System.Collections.Generic;

namespace Fathcore.Infrastructure.ResponseWrapper
{
    /// <summary>
    /// Represents response exception.
    /// </summary>
    public interface IResponseException
    {
        /// <summary>
        /// Gets or sets error message value.
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// Gets or sets details value.
        /// </summary>
        string Details { get; }

        /// <summary>
        /// Gets or sets validation errors value.
        /// </summary>
        IEnumerable<IModelValidation> ValidationErrors { get; }
    }
}
