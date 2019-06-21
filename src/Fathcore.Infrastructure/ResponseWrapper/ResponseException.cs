using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Fathcore.Infrastructure.Enum;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Fathcore.Infrastructure.ResponseWrapper
{
    /// <summary>
    /// Represents response exception.
    /// </summary>
    [DataContract]
    public class ResponseException : IResponseException
    {
        private readonly IErrorType _errorType;

        /// <summary>
        /// Gets a status code value.
        /// </summary>
        public int StatusCode => _errorType.StatusCode;

        /// <summary>
        /// Gets error type value.
        /// </summary>
        [DataMember]
        public string ErrorType => _errorType.Description;

        /// <summary>
        /// Gets error message value.
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; }

        /// <summary>
        /// Gets details value.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Details { get; }

        /// <summary>
        /// Gets validation errors value.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public IEnumerable<IModelValidation> ValidationErrors { get; }

        [JsonConstructor]
        public ResponseException(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public ResponseException(string errorMessage, IErrorType errorType)
            : this(errorMessage)
        {
            _errorType = errorType;
        }

        public ResponseException(string errorMessage, IErrorType errorType, string details)
            : this(errorMessage, errorType)
        {
            Details = details;
        }

        public ResponseException(string errorMessage, IErrorType errorType, string details, ModelStateDictionary modelState)
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
