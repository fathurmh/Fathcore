using Fathcore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for FileProvider.
    /// </summary>
    public static class FileProviderServicesExtensions
    {
        /// <summary>
        /// Add file provider service to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddFileProvider(this IServiceCollection services)
        {
            services.AddSingleton<FileProvider>();
            services.AddSingleton<IFileProvider>(provider => provider.GetRequiredService<FileProvider>());

            return services;
        }
    }
}
