using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.Extensions.Tests
{
    internal static class ServiceCollectionExtensions
    {
        public static ServiceDescriptor GetDescriptor<T>(this IServiceCollection services)
        {
            return services.GetDescriptor(typeof(T));
        }

        public static ServiceDescriptor GetDescriptor(this IServiceCollection services, Type serviceType)
        {
            return services.GetDescriptors(serviceType).Single();
        }

        public static ServiceDescriptor[] GetDescriptors<T>(this IServiceCollection services)
        {
            return services.GetDescriptors(typeof(T));
        }

        public static ServiceDescriptor[] GetDescriptors(this IServiceCollection services, Type serviceType)
        {
            return services.Where(x => x.ServiceType == serviceType).ToArray();
        }
    }
}
