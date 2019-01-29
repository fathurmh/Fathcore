using System.Collections.Generic;
using Newtonsoft.Json;

namespace Fathcore.Infrastructures
{
    /// <summary>
    /// Represents a model validation
    /// </summary>
    public class ModelValidation
    {
        /// <summary>
        /// Gets or sets the model validation error field
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; set; }

        /// <summary>
        /// Gets or sets the model validation error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the model validation error trace id
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, List<string>> Errors { get; set; }

        public ModelValidation(string field, string message)
        {
            Field = field != string.Empty ? field : null;
            Message = message;
        }

        [JsonConstructor]
        public ModelValidation(Dictionary<string, List<string>> errors, string traceId)
        {
            Errors = errors;
            Message = traceId;
        }
    }

}
