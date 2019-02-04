using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.Abstractions
{
    /// <summary>
    /// Classes implementing this interface can serve as a portal for the various services composing the Core engine. 
    /// Edit functionality, modules and implementations access most Core functionality through this interface.
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Get service provider
        /// </summary>
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Get service collection
        /// </summary>
        IServiceCollection ServiceCollection { get; }

        /// <summary>
        /// Get principal
        /// </summary>
        IPrincipal Principal { get; }
        
        /// <summary>
        /// Get http context accessor
        /// </summary>
        IHttpContextAccessor HttpContextAccessor { get; }
        
        /// <summary>
        /// Get http context
        /// </summary>
        HttpContext HttpContext { get; }

        /// <summary>
        /// Get http client
        /// </summary>
        HttpClient HttpClient { get; }

        /// <summary>
        /// Add and configure services
        /// </summary>
        /// <param name="configuration">Configuration of the application</param>
        /// <returns>Returns service provider</returns>
        IServiceCollection ConfigureServices(IConfiguration configuration = null);

        /// <summary>
        /// Post configure web host
        /// </summary>
        /// <param name="webHost"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        Task<IWebHost> PostConfigureAsync(IWebHost webHost, Assembly assembly);

        /// <summary>
        /// Configure an application's request pipeline.
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        void ConfigureRequestPipeline(IApplicationBuilder application);

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">Type of resolved service</typeparam>
        /// <returns>Returns resolved service</returns>
        T Resolve<T>() where T : class;

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <param name="type">Type of resolved service</param>
        /// <returns>Returns resolved service</returns>
        object Resolve(Type type);

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        /// <typeparam name="T">Type of resolved services</typeparam>
        /// <returns>Returns collection of resolved services</returns>
        IEnumerable<T> ResolveAll<T>();

        /// <summary>
        /// Resolve unregistered service
        /// </summary>
        /// <param name="type">Type of service</param>
        /// <returns>Returns resolved service</returns>
        object ResolveUnregistered(Type type);
    }
}
