using System.Runtime.Serialization;
using Fathcore.Extensions;
using Newtonsoft.Json;

namespace Fathcore.Infrastructure.ResponseWrapper
{
    /// <summary>
    /// Represents api response.
    /// </summary>
    [DataContract]
    public class Response
    {
        /// <summary>
        /// Gets or sets a status code value.
        /// </summary>
        [DataMember]
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets a value whether is success.
        /// </summary>
        [DataMember]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets a display message value.
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets a response exception value.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public IResponseException ResponseException { get; set; }

        /// <summary>
        /// Gets or sets a result value.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public object Result { get; set; }

        [JsonConstructor]
        public Response(int statusCode, bool isSuccess, string message, object result, IResponseException responseException = null)
        {
            StatusCode = statusCode;
            IsSuccess = isSuccess;
            Message = message.FirstLetterToUpper();
            Result = result;
            ResponseException = responseException;
        }
    }

    /// <summary>
    /// Represents api response.
    /// </summary>
    [DataContract]
    public class Response<TResult> : Response
    {
        [JsonConstructor]
        public Response(int statusCode, bool isSuccess, string message, TResult result, IResponseException apiError = null)
            : base(statusCode, isSuccess, message, result, apiError) { }
    }
}
