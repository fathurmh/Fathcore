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

            ResponseException apiError;
            ApiResponse<object> apiResponse;
            List<string> apiMessage = new List<string>();
            int code;
            string displayMessage;

            if (context.Exception is ApiException apiException)
            {
                apiError = new ResponseException(apiException.Message, ErrorTypes.ApiError)
                {
                    ValidationErrors = apiException.Errors
                };
                code = apiException.StatusCode;
            }
            else if (context.Exception is UnauthorizedAccessException unauthorizedException)
            {
                apiError = new ResponseException(unauthorizedException.Message, ErrorTypes.UnauthorizedAccess, unauthorizedException.InnerException.Message);
                code = (int)HttpStatusCode.Unauthorized;
            }
            else if (context.Exception is CoreException coreException)
            {
                apiError = new ResponseException(coreException.Message, coreException.ErrorType);
                code = coreException.StatusCode;
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

                apiError = new ResponseException(msg, ErrorTypes.SystemError, stack);
                code = (int)HttpStatusCode.InternalServerError;
            }

            displayMessage = _stringLocalizer[ApiResponseMessage.Exception];
            apiResponse = new ApiResponse<object>(code, null, displayMessage, apiError);

            context.HttpContext.Response.StatusCode = code;
            context.Result = new JsonResult(apiResponse);
        }
    }
}
