using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Specifies the lifetime of a service in an <see cref="IServiceCollection"/>.
    /// </summary>
    public enum Lifetime
    {
        /// <summary>
        /// Specifies that a single instance of the service will be created.
        /// </summary>
        Singleton = 0,

        /// <summary>
        /// Specifies that a new instance of the service will be created for each scope.
        /// <remarks>In ASP.NET Core applications a scope is created around each server request.</remarks>
        /// </summary>
        Scoped = 1,

        /// <summary>
        /// Specifies that a new instance of the service will be created every time it is requested.
        /// </summary>
        Transient = 2
    }
}
