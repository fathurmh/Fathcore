using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Fathcore.Infrastructure.Tests")]
namespace Fathcore.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Represents dependency registrar.
    /// </summary>
    internal class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Register services to an <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public IServiceCollection Register(IServiceCollection services)
        {
            services.AddSingleton<ITypeFinder, TypeFinder>();

            return services;
        }
    }
}
