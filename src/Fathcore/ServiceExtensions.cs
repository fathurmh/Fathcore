using Fathcore.Infrastructure.Caching;
using Fathcore.Security.Tokens;
using Microsoft.Extensions.DependencyInjection;

namespace Fathcore.Extensions.DependencyInjection
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddFathcoreService(this IServiceCollection services, ITokenSetting tokenSetting, ICacheSetting cacheSetting)
        {
            services.AddTypeFinder();
            services.AddFileProvider();
            services.AddEncryptFactory();
            services.AddHashFactory();
            services.AddTokenFactory(tokenSetting);
            services.AddScopedMemoryCacheManager(cacheSetting);
            services.AddMemoryCacheManager(cacheSetting);
            services.AddEtagFactory();
            services.AddAuditHandler();
            services.AddGenericRepository();
            services.AddGenericCachedRepository(cacheSetting);

            return services;
        }
    }
}
