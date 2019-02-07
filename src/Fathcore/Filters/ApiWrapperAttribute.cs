using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Fathcore.Extensions;
using Fathcore.Infrastructures;
using Fathcore.Localization.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace Fathcore.Filters
{
    /// <summary>
    /// Represents an api wrapper attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ApiWrapperAttribute : Attribute, IAsyncResultFilter
    {
        private readonly IStringLocalizer<ApiResponseMessage> _stringLocalizer;

        public bool Skip { get; set; }

        public ApiWrapperAttribute()
        {
            _stringLocalizer = EngineContext.Current.Resolve<IStringLocalizer<ApiResponseMessage>>();
        }

        /// <summary>
        /// Handling result as wrap as possible to ApiResponse
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            try
            {
                bool ensureSkip = false;           
                
                var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
                if (controllerActionDescriptor != null)
                {
                    var actionAttribute = controllerActionDescriptor.MethodInfo.GetCustomAttributes(typeof(ApiWrapperAttribute), true).LastOrDefault();
                    if (actionAttribute is ApiWrapperAttribute apiWrapperAttribute)
                    {
                        ensureSkip = apiWrapperAttribute.Skip;
                    }
                }

                if (ensureSkip || IsSwagger(context.HttpContext))
                {
                    await next();
                }
                else
                {
                    using (MemoryStream responseBody = new MemoryStream())
                    {
                        Stream originalBodyStream = context.HttpContext.Response.Body;
                        context.HttpContext.Response.Body = responseBody;

                        await next();

                        if (context.HttpContext.Response.StatusCode != (int)HttpStatusCode.NoContent)
                        {
                            context.HttpContext.Response.ContentType = "application/json";
                            if (context.HttpContext.Response.StatusCode.ToString().Substring(0, 1) == ((int)HttpStatusCode.OK).ToString().Substring(0, 1))
                            {
                                string body = await FormatResponse(context.HttpContext.Response.Body);
                                await HandleSuccessRequestAsync(context.HttpContext, body, context.HttpContext.Response.StatusCode);
                            }
                            else
                            {
                                string body = await FormatResponse(context.HttpContext.Response.Body);
                                await HandleNotSuccessRequestAsync(context.HttpContext, body, context.HttpContext.Response.StatusCode);
                            }
                        }

                        responseBody.Seek(0, SeekOrigin.Begin);
                        await responseBody.CopyToAsync(originalBodyStream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Task for handling request if success
        /// </summary>
        /// <param name="context"></param>
        /// <param name="body"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        private Task HandleSuccessRequestAsync(HttpContext context, object body, int code)
        {
            string bodyText;
            string jsonString;
            string displayMessage = _stringLocalizer[ApiResponseMessage.Success];
            ApiResponse<object> apiResponse;

            if (!body.ToString().IsValidJson())
            {
                bodyText = JsonConvert.SerializeObject(body);
            }
            else
            {
                bodyText = body.ToString();
            }

            dynamic bodyContent = JsonConvert.DeserializeObject<dynamic>(bodyText);

            Type type = bodyContent?.GetType();
            if (type.Equals(typeof(Newtonsoft.Json.Linq.JObject)))
            {
                apiResponse = JsonConvert.DeserializeObject<ApiResponse<object>>(bodyText);

                if (apiResponse.Result == null)
                {
                    apiResponse = new ApiResponse<object>(code, bodyContent, displayMessage);
                }
            }
            else
            {
                apiResponse = new ApiResponse<object>(code, bodyContent, displayMessage);
            }

            jsonString = JsonConvert.SerializeObject(apiResponse);

            return context.Response.WriteAsync(jsonString);
        }

        /// <summary>
        /// Task for handling request if failure
        /// </summary>
        /// <param name="context"></param>
        /// <param name="body"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        private Task HandleNotSuccessRequestAsync(HttpContext context, object body, int code)
        {
            string bodyText;
            string displayMessage;
            string jsonString;
            ResponseException responseException;
            ApiResponse<object> apiResponse;
            List<string> apiMessage = new List<string>();

            if (code == (int)HttpStatusCode.NotFound)
            {
                displayMessage = _stringLocalizer[ApiResponseMessage.NotFound];
                responseException = new ResponseException(displayMessage, ErrorTypes.ResourceNotFound);
            }
            else if (code == (int)HttpStatusCode.Unauthorized || code == (int)HttpStatusCode.Forbidden)
            {
                displayMessage = _stringLocalizer[ApiResponseMessage.Unauthorized];
                responseException = new ResponseException(displayMessage, ErrorTypes.UnauthorizedAccess);
            }
            else if (code == (int)HttpStatusCode.BadRequest)
            {
                bodyText = body.ToString().IsValidJson() ? body.ToString() : JsonConvert.SerializeObject(body);
                
                ModelValidation modelValidation = JsonConvert.DeserializeObject<ModelValidation>(bodyText);
                ModelStateDictionary modelState = new ModelStateDictionary();
                foreach (var item in modelValidation.Errors)
                {
                    modelState.AddModelError(item.Key, string.Join(" ", item.Value.ToArray()));
                }

                displayMessage = _stringLocalizer[ApiResponseMessage.ValidationError];
                responseException = new ResponseException(modelState, modelValidation.Message);
            }
            else
            {
                displayMessage = _stringLocalizer[ApiResponseMessage.ContactSupport];
                responseException = new ResponseException(displayMessage, ErrorTypes.SystemError);
            }

            apiResponse = new ApiResponse<object>(code, null, displayMessage, responseException);
            jsonString = JsonConvert.SerializeObject(apiResponse);

            context.Response.StatusCode = code;
            return context.Response.WriteAsync(jsonString);
        }

        /// <summary>
        /// Format http response body to plain text
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task<string> FormatResponse(Stream response)
        {
            response.Seek(0, SeekOrigin.Begin);
            string plainBodyText = await new StreamReader(response).ReadToEndAsync();
            response.Seek(0, SeekOrigin.Begin);

            return plainBodyText;
        }

        /// <summary>
        /// Determine an url endpoint is swagger or not
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool IsSwagger(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments("/swagger");
        }
    }
}
