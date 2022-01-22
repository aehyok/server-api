using EasyCaching.Core.Configurations;
using Lychee.Extension.Cache.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Lychee.Extension.Cache.EasyCaching
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 注册 EasyCaching 缓存服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddLycheeEasyCaching(this IServiceCollection services, Action<EasyCachingOptions> configAction)
        {
            services.AddScoped<ICache, CacheManager>();
            services.AddEasyCaching(configAction);

            return services;
        }
    }
}