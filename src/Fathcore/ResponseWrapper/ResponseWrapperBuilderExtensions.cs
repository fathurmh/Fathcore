using Fathcore.ResponseWrapper;
using Microsoft.AspNetCore.Builder;

namespace Fathcore.Extensions.Builder
{
    /// <summary>
    /// Extension methods for the ResponseWrapper middleware.
    /// </summary>
    public static class ResponseWrapperBuilderExtensions
    {
        /// <summary>
        /// Using response wrapper middleware.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseApiResponseWrapper(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ResponseWrapperMiddleware>();
        }
    }
}
