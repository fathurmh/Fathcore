using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.DependencyInjection
{
    /// <summary>
    /// Represents dependency registrar interface.
    /// </summary>
    public interface IDependencyRegistrar
    {
        /// <summary>
        /// Register services to an <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        IServiceCollection Register(IServiceCollection services);
    }
}
