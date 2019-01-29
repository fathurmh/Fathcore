using System;
using System.Net;
using System.Runtime.Serialization;

namespace Fathcore.Exceptions
{
    /// <summary>
    /// Represents errors that occur during application execution
    /// </summary>
    public class CoreException : Exception
    {
        /// <summary>
        /// Gets or sets the api exception status code
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Initializes a new instance of the Exception class
        /// </summary>
        /// <param name="statusCode">The exception http status code</param>
        /// <returns>Returns a core exception</returns>
        public CoreException(int statusCode = (int)HttpStatusCode.OK)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with a specified error message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="statusCode">The exception http status code</param>
        /// <returns>Returns a core exception</returns>
        public CoreException(string message, int statusCode = (int)HttpStatusCode.OK)
            : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with a specified error message
        /// </summary>
        /// <param name="messageFormat">The exception message format</param>
        /// <param name="statusCode">The exception http status code</param>
        /// <param name="args">The exception message arguments</param>
        /// <returns>Returns a core exception</returns>
        public CoreException(string messageFormat, int statusCode = (int)HttpStatusCode.OK, params object[] args)
            : base(string.Format(messageFormat, args))
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with serialized data
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination</param>
        /// <param name="statusCode">The exception http status code</param>
        /// <returns>Returns a core exception</returns>
        protected CoreException(SerializationInfo info, StreamingContext context, int statusCode = (int)HttpStatusCode.OK)
            : base(info, context)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with a specified error message and a reference to the inner exception that is the cause of this exception
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified</param>
        /// <param name="statusCode">The exception http status code</param>
        /// <returns>Returns a core exception</returns>
        public CoreException(string message, Exception innerException, int statusCode = (int)HttpStatusCode.OK)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
