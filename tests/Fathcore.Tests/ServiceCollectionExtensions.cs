using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.Tests
{
    public static class ServiceCollectionExtensions
    {
        public static ServiceDescriptor GetDescriptor<T>(this IServiceCollection services)
        {
            return services.GetDescriptors<T>().Single();
        }

        public static ServiceDescriptor[] GetDescriptors<T>(this IServiceCollection services)
        {
            return services.GetDescriptors(typeof(T));
        }

        public static ServiceDescriptor[] GetDescriptors(this IServiceCollection services, Type serviceType)
        {
            return services.Where(x => x.ServiceType == serviceType).ToArray();
        }

        public static ServiceDescriptor[] GetDescriptors(this IServiceCollection services, ServiceLifetime lifetime)
        {
            return services.Where(x => x.Lifetime == lifetime).ToArray();
        }
    }
}
