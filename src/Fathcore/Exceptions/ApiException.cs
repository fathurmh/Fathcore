using System;
using System.Collections.Generic;
using System.Net;
using Fathcore.Infrastructures;

namespace Fathcore.Exceptions
{
    /// <summary>
    /// Represents an api exception
    /// </summary>
    [Serializable]
    public class ApiException : Exception
    {
        /// <summary>
        /// Gets or sets the api exception status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the api exception validation errors
        /// </summary>
        public IEnumerable<ModelValidation> Errors { get; set; }

        /// <summary>
        /// Initializes a new instance of the Exception class with a specified error message
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="statusCode">Http status code</param>
        /// <param name="errors">Validation errors</param>
        /// <returns>Returns an api exception</returns>
        public ApiException(string message, int statusCode = (int)HttpStatusCode.InternalServerError, IEnumerable<ModelValidation> errors = null) : base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with a specified exception
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="statusCode">Http status code</param>
        /// <returns>Returns an api exception</returns>
        public ApiException(Exception ex, int statusCode = 500) : base(ex.Message)
        {
            StatusCode = statusCode;
        }
    }

}
