using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Fathcore.Localization.Resources;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace Fathcore.Infrastructures
{
    /// <summary>
    /// Represents an response exception
    /// </summary>
    [DataContract]
    public class ResponseException
    {
        /// <summary>
        /// Gets or sets the response exception error type
        /// </summary>
        [DataMember]
        public string ErrorType { get; set; }

        /// <summary>
        /// Gets or sets the response exception error message
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the response exception details
        /// </summary>
        [DataMember]
        public string Details { get; set; }

        /// <summary>
        /// Gets or sets the response exception validation errors
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public IEnumerable<ModelValidation> ValidationErrors { get; set; }

        [JsonConstructor]
        public ResponseException(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public ResponseException(string errorMessage, string errorType)
            : this(errorMessage)
        {
            ErrorType = errorType;
        }

        public ResponseException(string errorMessage, string errorType, string details)
            : this(errorMessage, errorType)
        {
            Details = details;
        }

        public ResponseException(ModelStateDictionary modelState)
        {
            if (modelState != null && modelState.Any(m => m.Value.Errors.Count > 0))
            {
                var stringLocalizer = EngineContext.Current.Resolve<IStringLocalizer<ApiResponseMessage>>();
                ErrorMessage = stringLocalizer[ApiResponseMessage.ValidationException];
                ValidationErrors = modelState.Keys
                    .SelectMany(key => modelState[key].Errors.Select(x => new ModelValidation(key, x.ErrorMessage)))
                    .ToList();
                ErrorType = ErrorTypes.InvalidRequest;
            }
        }
        
        public ResponseException(ModelStateDictionary modelState, string errorMessage)
            : this(modelState)
        {
            ErrorMessage = errorMessage;
        }
        
        public ResponseException(ModelStateDictionary modelState, string errorMessage, string details)
            : this(modelState, errorMessage)
        {
            Details = details;
        }
    }
}
