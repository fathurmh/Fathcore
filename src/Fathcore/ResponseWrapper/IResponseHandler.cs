using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Fathcore.ResponseWrapper
{
    /// <summary>
    /// Represents response handler.
    /// </summary>
    public interface IResponseHandler
    {
        /// <summary>
        /// Deteminte whether response is success or not.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// Collection of status code those will returned with no content.
        /// </summary>
        IEnumerable<int> NoContentStatusCode { get; }

        /// <summary>
        /// Handle response.
        /// </summary>
        /// <param name="context">HttpContext.</param>
        /// <param name="body">Response body.</param>
        /// <param name="code">Status code.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task HandleResponseAsync(HttpContext context, object body, int code);

        /// <summary>
        /// Handle response that marked as exception status.
        /// </summary>
        /// <param name="context">HttpContext.</param>
        /// <param name="exception">Exception.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task HandleExceptionAsync(HttpContext context, Exception exception);

        /// <summary>
        /// Format response body stream to string.
        /// </summary>
        /// <param name="response">Body stream.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<string> FormatResponse(Stream response);
    }
}