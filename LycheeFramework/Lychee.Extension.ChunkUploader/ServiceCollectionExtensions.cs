using Microsoft.Extensions.DependencyInjection;
using System;

namespace Lychee.Extension.ChunkUploader
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加分片上传服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IServiceCollection AddChunkUploader(this IServiceCollection services, Action<UploaderOptions> action = null)
        {
            services.Configure(action);
            return services;
        }
    }
}