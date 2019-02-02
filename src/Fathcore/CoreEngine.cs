using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Fathcore.Abstractions;
using Fathcore.Extensions;
using Fathcore.Filters;
using Fathcore.Infrastructures;
using Fathcore.Infrastructures.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Fathcore
{
    /// <summary>
    /// Represents Core engine
    /// </summary>
    public class CoreEngine : IEngine
    {
        private IServiceProvider _serviceProvider { get; set; }
        private IServiceCollection _serviceCollection { get; set; }
        private IHttpContextAccessor _httpContextAccessor { get; set; }
        private IPrincipal _principal { get; set; }

        /// <summary>
        /// Get service provider
        /// </summary>
        public virtual IServiceProvider ServiceProvider => _httpContextAccessor?.HttpContext?.RequestServices ?? _serviceProvider;

        /// <summary>
        /// Get service collection
        /// </summary>
        public virtual IServiceCollection ServiceCollection => _serviceCollection;

        /// <summary>
        /// Get http context accessor
        /// </summary>
        public virtual IHttpContextAccessor HttpContextAccessor => ServiceProvider.GetService<IHttpContextAccessor>() ?? _httpContextAccessor;

        /// <summary>
        /// Get current principal
        /// </summary>
        public virtual IPrincipal Principal
        {
            get
            {
                IIdentity identity = HttpContextAccessor?.HttpContext?.User?.Identity ?? new GenericIdentity("Anonymous");
                _principal = new GenericPrincipal(identity, identity.GetRoleTypeClaimValue());
                Thread.CurrentPrincipal = _principal;
                return _principal;
            }
        }

        /// <summary>
        /// Get http client
        /// </summary>
        public virtual HttpClient HttpClient => Singleton<HttpClient>.Instance ?? (Singleton<HttpClient>.Instance = new HttpClient());   

        /// <summary>
        /// Initialize engine
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public CoreEngine(IServiceCollection services = null)
        {
            _serviceCollection = services ?? new ServiceCollection();
            _serviceProvider = ServiceCollection.BuildServiceProvider();
            _httpContextAccessor = ServiceProvider.GetService<IHttpContextAccessor>();
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //check for assembly already loaded
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (assembly != null)
                return assembly;
            return assembly;
        }

        /// <summary>
        /// Add and configure services
        /// </summary>
        /// <returns>Service provider</returns>
        public virtual IServiceCollection ConfigureServices(IConfiguration configuration = null)
        {
            Assembly.GetExecutingAssembly().GetTypes()
                    .Where(type => type.GetInterfaces().Contains(typeof(IDependencyRegistrar))).ToList()
                    .ForEach(registrar => ResolveUnregistered(registrar));

            _serviceCollection.AddHttpContextAccessor();
            _serviceCollection.AddLocalization();
            _serviceCollection.AddResponseCaching();
            _serviceCollection.AddResponseCompression(options => 
            {
                options.EnableForHttps = true;
            });
            
            _serviceCollection.AddMvcCore(options =>
            {
                options.Filters.Add(typeof(ApiExceptionFilterAttribute));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddJsonFormatters(opt =>
            {
                opt.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                opt.ContractResolver = new DefaultContractResolver();
            });

            _serviceProvider = _serviceCollection.BuildServiceProvider();
            
            return _serviceCollection;
        }

        /// <summary>
        /// Configure HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public virtual void ConfigureRequestPipeline(IApplicationBuilder application)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">Type of resolved service</typeparam>
        /// <returns>Resolved service</returns>
        public virtual T Resolve<T>() where T : class
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                return (T)scope.ServiceProvider.GetRequiredService(typeof(T));
            }
        }

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <param name="type">Type of resolved service</param>
        /// <returns>Resolved service</returns>
        public virtual object Resolve(Type type)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                return scope.ServiceProvider.GetRequiredService(type);
            }
        }

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        /// <typeparam name="T">Type of resolved services</typeparam>
        /// <returns>Collection of resolved services</returns>
        public virtual IEnumerable<T> ResolveAll<T>()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                return (IEnumerable<T>)scope.ServiceProvider.GetServices(typeof(T));
            }
        }

        /// <summary>
        /// Resolve unregistered service
        /// </summary>
        /// <param name="type">Type of service</param>
        /// <returns>Resolved service</returns>
        public virtual object ResolveUnregistered(Type type)
        {
            Exception innerException = null;
            foreach (var constructor in type.GetConstructors())
            {
                try
                {
                    //try to resolve constructor parameters
                    var parameters = constructor.GetParameters().Select(parameter =>
                    {
                        var service = Resolve(parameter.ParameterType);
                        if (service == null)
                            throw new Exception("Unknown dependency");
                        return service;
                    });

                    //all is ok, so create instance
                    return Activator.CreateInstance(type, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    innerException = ex;
                }
            }

            throw new Exception("No constructor was found that had all the dependencies satisfied.", innerException);
        }

        /// <summary>
        /// Post configure web host
        /// </summary>
        /// <param name="webHost"></param>
        /// <returns></returns>
        public virtual async Task<IWebHost> PostConfigureAsync(IWebHost webHost)
        {
            await webHost.DatabaseMigrateAsync();
            return webHost;
        }
    }
}
