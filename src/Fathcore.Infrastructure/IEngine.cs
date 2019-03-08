using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.Infrastructure
{
    /// <summary>
    /// Classes implementing this interface can serve as a portal for the various services composing the engine. 
    /// Edit functionality, modules and implementations access most functionality through this interface.
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Populating service collection to DI container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        IEngine Populate(IServiceCollection services);
    }
}
