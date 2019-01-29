using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Fathcore.Exceptions;
using Fathcore.Filters;
using Fathcore.Infrastructures;
using Fathcore.Localization.Resources;
using Fathcore.Tests;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Fathcore.Tests.Filters
{
    public class ApiExceptionFilterAttributeTests : TestsBase
    {
        [Theory]
        [MemberData(nameof(ExceptionData))]
        public async Task Should_Filter_Exception(Exception exception, string exceptionMessage, int httpStatusCode)
        {
            var exceptionContext = GetExceptionContext(exception);
            var stringLocalizer = GetMockStringLocalizer<ApiResponseMessage>(ApiResponseMessage.Exception, ApiResponseMessage.Exception);
            var apiExceptionFilter = new ApiExceptionFilterAttribute(stringLocalizer.Object);

            await apiExceptionFilter.OnExceptionAsync(exceptionContext);
            var result = (exceptionContext.Result as JsonResult).Value as ApiResponse<object>;

            Assert.Equal(httpStatusCode, result.StatusCode);
            Assert.Equal(exceptionMessage, result.ResponseException.ExceptionMessage);
        }
        
        public static IEnumerable<object[]> ExceptionData()
        {
            return new List<object[]>
            {
                new object[] { new Exception("Exception Test"), "Exception Test", (int)HttpStatusCode.InternalServerError },
                new object[] { new CoreException("Core Exception Test"), "Core Exception Test", (int)HttpStatusCode.OK },
                new object[] { new ApiException("Api Exception Test"), "Api Exception Test", (int)HttpStatusCode.InternalServerError },
            };
        }
    }
}
