using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Fathcore.Exceptions;
using Fathcore.Infrastructures;
using Fathcore.Localization.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace Fathcore.Filters
{
    /// <summary>
    /// Represents an api exception filter attribute
    /// </summary>
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute, IAsyncExceptionFilter
    {
        private readonly IStringLocalizer<ApiResponseMessage> _stringLocalizer;

        public ApiExceptionFilterAttribute(IStringLocalizer<ApiResponseMessage> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        /// <summary>
        /// Handling response if request got an exception
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            await base.OnExceptionAsync(context);

            ApiError apiError;
            ApiResponse<object> apiResponse;
            List<string> apiMessage = new List<string>();
            int code = 0;
            string responseMessage = string.Empty;

            if (context.Exception is ApiException)
            {
                var ex = context.Exception as ApiException;
                apiError = new ApiError(ex.Message)
                {
                    ValidationErrors = ex.Errors,
                    ReferenceErrorCode = ex.ReferenceErrorCode,
                    ReferenceDocumentLink = ex.ReferenceDocumentLink
                };
                code = ex.StatusCode;

            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                responseMessage = _stringLocalizer[ApiResponseMessage.Unauthorized];
                apiError = new ApiError(responseMessage);
                code = (int)HttpStatusCode.Unauthorized;
            }
            else if (context.Exception is CoreException)
            {
                var ex = context.Exception as CoreException;
                apiError = new ApiError(ex.Message);
                code = ex.StatusCode;
            }
            else
            {
#if !DEBUG
                var msg = _stringLocalizer[ApiResponseMessage.Unhandled];
                string stack = null;
#else
                var msg = context.Exception.GetBaseException().Message;
                string stack = context.Exception.StackTrace;
#endif

                apiError = new ApiError(msg)
                {
                    Details = stack
                };
                code = (int)HttpStatusCode.InternalServerError;

            }

            responseMessage = _stringLocalizer[ApiResponseMessage.Exception];
            apiMessage.Add(responseMessage);
            apiResponse = new ApiResponse<object>(code, null, apiMessage, apiError);

            context.HttpContext.Response.StatusCode = code;
            context.Result = new JsonResult(apiResponse);
        }
    }
}
