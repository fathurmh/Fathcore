using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Fathcore.ResponseWrapper
{
    /// <summary>
    /// Represents model validation.
    /// </summary>
    [DataContract]
    public class ModelValidation : IModelValidation
    {
        /// <summary>
        /// Gets or sets field name.
        /// </summary>
        [DataMember]
        public string Field { get; set; }

        /// <summary>
        /// Gets or sets message value.
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets details value.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Details { get; set; }

        /// <summary>
        /// Gets or sets errors value.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public Dictionary<string, IEnumerable<string>> Errors { get; set; }

        public ModelValidation(string field, string message)
        {
            Field = field != string.Empty ? field : null;
            Message = message;
        }

        [JsonConstructor]
        public ModelValidation(Dictionary<string, IEnumerable<string>> errors, string title, string traceId)
        {
            Errors = errors;
            Message = title;
            Details = string.Concat("Trace Id: ", traceId);
        }
    }
}
