using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fathcore.ResponseWrapper
{
    /// <summary>
    /// Represents api response wrapper filter.
    /// </summary>
    public class ResponseWrapperFilter : IAsyncResultFilter
    {
        /// <summary>
        /// Asynchronously perform task after result executed.
        /// </summary>
        /// <param name="context">ResultExecutingContext.</param>
        /// <param name="next">ResultExecutionDelegate.</param>
        /// <returns>A task that represents an asynchronous operation.</returns>
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            bool skipWrap = default;
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                skipWrap = controllerActionDescriptor.MethodInfo.GetCustomAttributes(typeof(SkipResponseWrapperAttribute), true).Any();

            context.HttpContext.Items.Add(nameof(SkipResponseWrapperAttribute), skipWrap);

            await next();
        }
    }
}
