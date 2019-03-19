using System.Collections.Generic;

namespace Fathcore.ResponseWrapper
{
    /// <summary>
    /// Represents model validation.
    /// </summary>
    public interface IModelValidation
    {
        /// <summary>
        /// Gets or sets field name.
        /// </summary>
        string Field { get; }

        /// <summary>
        /// Gets or sets message value.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Gets or sets details value.
        /// </summary>
        string Details { get; }

        /// <summary>
        /// Gets or sets errors value.
        /// </summary>
        Dictionary<string, IEnumerable<string>> Errors { get; }
    }
}