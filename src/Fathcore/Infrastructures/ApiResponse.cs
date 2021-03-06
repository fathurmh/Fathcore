﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Fathcore.Infrastructures
{
    /// <summary>
    /// Represents an api response
    /// </summary>
    [DataContract]
    public class ApiResponse<TModel>
    {
        /// <summary>
        /// Gets or sets the api response status code
        /// </summary>
        [DataMember]
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the api response is success
        /// </summary>
        [DataMember]
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the api response message
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the api response exception
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public ResponseException ResponseException { get; set; }

        /// <summary>
        /// Gets or sets the api response result
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public TModel Result { get; set; }

        [JsonConstructor]
        public ApiResponse(int statusCode, TModel result, string message, ResponseException apiError = null)
        {
            StatusCode = statusCode;
            IsSuccess = apiError == null;
            Message = message;
            Result = result;
            ResponseException = apiError;
        }
    }

}
