using System.Threading.Tasks;
using Fathcore.Infrastructure.Pagination;
using Microsoft.AspNetCore.Http;

namespace Fathcore.Infrastructure.ResponseWrapper
{
    public static class ApiResponses
    {
        public static ApiResponse Success(string displayMessage)
        {
            var response = new ApiResponse(StatusCodes.Status200OK, true, displayMessage, default, default, default);
            return response;
        }

        public static ApiResponse<TResult> Success<TResult>(string displayMessage)
        {
            return Success<TResult>(default, displayMessage);
        }

        public static ApiResponse<TResult> Success<TResult>(TResult result, string displayMessage)
        {
            if (typeof(IPagedData).IsAssignableFrom(typeof(TResult)))
                return Success(result, (IPagedData)result, displayMessage);
            else
                return Success(result, default, displayMessage);
        }

        public static ApiResponse<TResult> Success<TResult>(TResult result, IPagedData pagedList, string displayMessage)
        {
            var response = new ApiResponse<TResult>(StatusCodes.Status200OK, true, displayMessage, result, pagedList, default);
            return response;
        }

        public static ApiResponse Fail(string displayMessage)
        {
            return Fail(default, displayMessage);
        }

        public static ApiResponse Fail(IResponseException exception, string displayMessage)
        {
            var response = new ApiResponse(exception.StatusCode, false, displayMessage, default, default, exception);
            return response;
        }

        public static ApiResponse<TResult> Fail<TResult>(string displayMessage)
        {
            return Fail<TResult>(default, displayMessage);
        }

        public static ApiResponse<TResult> Fail<TResult>(IResponseException exception, string displayMessage)
        {
            var response = new ApiResponse<TResult>(exception.StatusCode, false, displayMessage, default, default, exception);
            return response;
        }

        public static ApiResponse WithStatusCode(this ApiResponse apiResponseTask, int statusCode)
        {
            apiResponseTask.StatusCode = statusCode;
            return apiResponseTask;
        }

        //public static async Task<ApiResponse> SuccessAsync()
        //{
        //    var localizer = EngineContext.Current.Resolve<IStringLocalizer<CommonMessage>>();
        //    return await SuccessAsync(localizer[CommonMessage.Success]);
        //}

        public static async Task<ApiResponse> SuccessAsync(string displayMessage)
        {
            var response = new ApiResponse(StatusCodes.Status200OK, true, displayMessage, default, default, default);
            return await Task.FromResult(response);
        }

        //public static async Task<ApiResponse<TResult>> SuccessAsync<TResult>()
        //{
        //    var localizer = EngineContext.Current.Resolve<IStringLocalizer<CommonMessage>>();
        //    return await SuccessAsync<TResult>(default, default, localizer[CommonMessage.Success]);
        //}

        public static async Task<ApiResponse<TResult>> SuccessAsync<TResult>(string displayMessage)
        {
            return await SuccessAsync<TResult>(default, displayMessage);
        }

        //public static async Task<ApiResponse<TResult>> SuccessAsync<TResult>(TResult result)
        //{
        //    var localizer = EngineContext.Current.Resolve<IStringLocalizer<CommonMessage>>();
        //    return await SuccessAsync(result, localizer[CommonMessage.Success]);
        //}

        //public static async Task<ApiResponse<TResult>> SuccessAsync<TResult>(TResult result, IPagedData pagedList)
        //{
        //    var localizer = EngineContext.Current.Resolve<IStringLocalizer<CommonMessage>>();
        //    return await SuccessAsync(result, pagedList, localizer[CommonMessage.Success]);
        //}

        public static async Task<ApiResponse<TResult>> SuccessAsync<TResult>(TResult result, string displayMessage)
        {
            if (typeof(IPagedData).IsAssignableFrom(typeof(TResult)))
                return await SuccessAsync(result, (IPagedData)result, displayMessage);
            else
                return await SuccessAsync(result, default, displayMessage);
        }

        public static async Task<ApiResponse<TResult>> SuccessAsync<TResult>(TResult result, IPagedData pagedList, string displayMessage)
        {
            var response = new ApiResponse<TResult>(StatusCodes.Status200OK, true, displayMessage, result, pagedList, default);
            return await Task.FromResult(response);
        }

        //public static async Task<ApiResponse> FailAsync()
        //{
        //    var localizer = EngineContext.Current.Resolve<IStringLocalizer<CommonMessage>>();
        //    return await FailAsync(localizer[CommonMessage.Failure]);
        //}

        public static async Task<ApiResponse> FailAsync(string displayMessage)
        {
            return await FailAsync(default, displayMessage);
        }

        //public static async Task<ApiResponse> FailAsync(IResponseException exception)
        //{
        //    var localizer = EngineContext.Current.Resolve<IStringLocalizer<CommonMessage>>();
        //    return await FailAsync(exception, localizer[CommonMessage.Failure]);
        //}

        public static async Task<ApiResponse> FailAsync(IResponseException exception, string displayMessage)
        {
            var response = new ApiResponse(exception.StatusCode, false, displayMessage, default, default, exception);
            return await Task.FromResult(response);
        }

        //public static async Task<ApiResponse<TResult>> FailAsync<TResult>()
        //{
        //    var localizer = EngineContext.Current.Resolve<IStringLocalizer<CommonMessage>>();
        //    return await FailAsync<TResult>(localizer[CommonMessage.Failure]);
        //}

        public static async Task<ApiResponse<TResult>> FailAsync<TResult>(string displayMessage)
        {
            return await FailAsync<TResult>(default, displayMessage);
        }

        //public static async Task<ApiResponse<TResult>> FailAsync<TResult>(IResponseException exception)
        //{
        //    var localizer = EngineContext.Current.Resolve<IStringLocalizer<CommonMessage>>();
        //    return await FailAsync<TResult>(exception, localizer[CommonMessage.Failure]);
        //}

        public static async Task<ApiResponse<TResult>> FailAsync<TResult>(IResponseException exception, string displayMessage)
        {
            var response = new ApiResponse<TResult>(exception.StatusCode, false, displayMessage, default, default, exception);
            return await Task.FromResult(response);
        }

        public static async Task<T> WithStatusCodeAsync<T>(this Task<T> apiResponseTask, int statusCode) where T : ApiResponse
        {
            (await apiResponseTask).StatusCode = statusCode;
            return await apiResponseTask;
        }
    }
}
