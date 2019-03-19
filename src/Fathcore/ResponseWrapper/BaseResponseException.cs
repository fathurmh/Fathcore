using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Fathcore.ResponseWrapper
{
    /// <summary>
    /// Represents response exception.
    /// </summary>
    [DataContract]
    public class BaseResponseException : IResponseException
    {
        /// <summary>
        /// Gets or sets error type value.
        /// </summary>
        [DataMember]
        public string ErrorType { get; }

        /// <summary>
        /// Gets or sets error message value.
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; }

        /// <summary>
        /// Gets or sets details value.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Details { get; }

        /// <summary>
        /// Gets or sets validation errors value.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public IEnumerable<IModelValidation> ValidationErrors { get; }

        public BaseResponseException()
        {

        }

        [JsonConstructor]
        public BaseResponseException(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public BaseResponseException(string errorMessage, string errorType)
            : this(errorMessage)
        {
            ErrorType = errorType;
        }

        public BaseResponseException(string errorMessage, string errorType, string details)
            : this(errorMessage, errorType)
        {
            Details = details;
        }

        public BaseResponseException(string errorMessage, string errorType, string details, ModelStateDictionary modelState)
            : this(errorMessage, errorType, details)
        {
            if (modelState != null && modelState.Any(m => m.Value.Errors.Count > 0))
            {
                ValidationErrors = modelState.Keys
                    .SelectMany(key => modelState[key].Errors.Select(x => new ModelValidation(key, x.ErrorMessage)))
                    .ToList();
            }
        }
    }
}
