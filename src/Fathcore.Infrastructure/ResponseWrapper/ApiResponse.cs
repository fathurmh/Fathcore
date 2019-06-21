using System.Runtime.Serialization;
using Fathcore.Extensions;
using Fathcore.Infrastructure.Pagination;
using Newtonsoft.Json;

namespace Fathcore.Infrastructure.ResponseWrapper
{
    /// <summary>
    /// Represents api response.
    /// </summary>
    [DataContract]
    public class ApiResponse
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
        /// Gets or sets a paged data value.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public IPagedData Pagination { get; set; }

        /// <summary>
        /// Gets or sets a result value.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public object Result { get; set; }

        [JsonConstructor]
        public ApiResponse(int statusCode, bool isSuccess, string message, object result, IPagedData pagedData = default, IResponseException responseException = default)
        {
            StatusCode = statusCode;
            IsSuccess = isSuccess;
            Message = message.FirstLetterToUpper();
            Result = result;
            Pagination = new PagedData(pagedData);
            ResponseException = responseException;
        }
    }

    /// <summary>
    /// Represents api response.
    /// </summary>
    [DataContract]
    public class ApiResponse<TResult> : ApiResponse
    {
        [JsonConstructor]
        public ApiResponse(int statusCode, bool isSuccess, string message, TResult result, IPagedData pagedData = default, IResponseException apiError = default)
            : base(statusCode, isSuccess, message, result, pagedData, apiError) { }
    }
}
