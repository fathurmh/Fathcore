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
                bool ensureSkip = context.ActionDescriptor.FilterDescriptors
                    .Where(prop => prop.Filter.GetType()
                    .Equals(typeof(ApiWrapperAttribute)))
                    .Select(prop => (ApiWrapperAttribute)prop.Filter)
                    .Any(prop => prop.Skip);

                if (ensureSkip)
                {
                    await next();
                }
                else
                {
                    if (IsSwagger(context.HttpContext))
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
            ApiResponse<object> apiResponse;
            List<string> apiMessage = new List<string>();

            string responseMessage = _stringLocalizer[ApiResponseMessage.Success];
            apiMessage.Add(responseMessage);

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
                    apiResponse = new ApiResponse<object>(code, bodyContent, apiMessage, null);
                }
            }
            else
            {
                apiResponse = new ApiResponse<object>(code, bodyContent, apiMessage, null);
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
            string responseMessage;
            string jsonString;
            ApiError apiError;
            ApiResponse<object> apiResponse;
            List<string> apiMessage = new List<string>();

            if (!body.ToString().IsValidJson())
            {
                bodyText = JsonConvert.SerializeObject(body);
            }
            else
            {
                bodyText = body.ToString();
            }

            if (code == (int)HttpStatusCode.NotFound)
            {
                responseMessage = _stringLocalizer[ApiResponseMessage.NotFound];
                apiError = new ApiError(responseMessage);
            }
            else if (code == (int)HttpStatusCode.Unauthorized || code == (int)HttpStatusCode.Forbidden)
            {
                responseMessage = _stringLocalizer[ApiResponseMessage.Unauthorized];
                apiMessage.Add(responseMessage);
                apiError = new ApiError(_stringLocalizer[ApiResponseMessage.ContactSupport]);
            }
            else if (code == (int)HttpStatusCode.BadRequest)
            {
                ModelValidation modelValidation = JsonConvert.DeserializeObject<ModelValidation>(bodyText);
                ModelStateDictionary modelState = new ModelStateDictionary();
                foreach (var item in modelValidation.Errors)
                {
                    modelState.AddModelError(item.Key, string.Join(" ", item.Value.ToArray()));
                }

                responseMessage = _stringLocalizer[ApiResponseMessage.ValidationError];
                apiMessage.Add(responseMessage);
                apiError = new ApiError(modelState, modelValidation.Message);
            }
            else
            {
                responseMessage = _stringLocalizer[ApiResponseMessage.ContactSupport];
                apiError = new ApiError(responseMessage);
            }

            responseMessage = _stringLocalizer[ApiResponseMessage.Failure];
            apiMessage.Add(responseMessage);
            apiResponse = new ApiResponse<object>(code, null, apiMessage, apiError);
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
