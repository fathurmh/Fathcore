using System.Collections.Generic;
using System.Linq;
using Fathcore.Localization.Resources;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace Fathcore.Infrastructures
{
    /// <summary>
    /// Represents an api error
    /// </summary>
    public class ApiError
    {
        /// <summary>
        /// Gets or sets the api error is error
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// Gets or sets the api error exception message
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// Gets or sets the api error details
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// Gets or sets the api error reference error code
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ReferenceErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the api error reference document link
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ReferenceDocumentLink { get; set; }

        /// <summary>
        /// Gets or sets the api error validation errors
        /// </summary>
        public IEnumerable<ModelValidation> ValidationErrors { get; set; }

        [JsonConstructor]
        public ApiError(string message)
        {
            ExceptionMessage = message;
            IsError = true;
        }

        public ApiError(string message, string details)
            : this(message)
        {
            Details = details;
        }

        public ApiError(ModelStateDictionary modelState)
        {
            IsError = true;
            if (modelState != null && modelState.Any(m => m.Value.Errors.Count > 0))
            {
                var stringLocalizer = EngineContext.Current.Resolve<IStringLocalizer<ApiResponseMessage>>();
                ExceptionMessage = stringLocalizer[ApiResponseMessage.ValidationException];
                ValidationErrors = modelState.Keys
                    .SelectMany(key => modelState[key].Errors.Select(x => new ModelValidation(key, x.ErrorMessage)))
                    .ToList();

            }
        }
        
        public ApiError(ModelStateDictionary modelState, string details)
            : this(modelState)
        {
            Details = details;
        }
    }
}
