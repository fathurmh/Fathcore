using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Fathcore.ResponseWrapper
{
    /// <summary>
    /// Represents api exception middleware.
    /// </summary>
    public class ResponseWrapperMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseWrapperMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IResponseHandler responseHandler)
        {
            if (responseHandler is null)
                return;

            using (var responseBody = new MemoryStream())
            {
                Stream originalBodyStream = context.Response.Body;
                context.Response.Body = responseBody;

                try
                {
                    await _next(context);

                    bool skipWrap = false;
                    if (context.Items.TryGetValue(nameof(SkipResponseWrapperAttribute), out var value))
                        skipWrap = (bool)value;

                    var statusCode = context.Response.StatusCode;
                    if (skipWrap || responseHandler.NoContentStatusCode.Contains(statusCode))
                        return;

                    string body = await responseHandler.FormatResponse(context.Response.Body);
                    await responseHandler.HandleResponseAsync(context, body, statusCode);
                }
                catch (Exception ex)
                {
                    await responseHandler.HandleExceptionAsync(context, ex);
                }
                finally
                {
                    if (responseHandler.NoContentStatusCode.Contains(context.Response.StatusCode))
                    {
                        context.Response.Headers[HeaderNames.ContentLength] = "0";
                        context.Response.ContentType = null;
                        context.Response.Body = null;
                    }
                    else
                    {
                        responseBody.Seek(0, SeekOrigin.Begin);
                        await responseBody.CopyToAsync(originalBodyStream);
                    }
                }
            }
        }
    }
}
