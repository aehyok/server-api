using DVS.Application.Services.Common;
using DVS.Common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DVS.Open.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IServiceBase<>), typeof(ServiceBase<>));

            services.AddScoped<ISunFileInfoService, SunFileInfoService>();
            services.AddScoped<IBasicUserService, BasicUserService>();
            services.AddScoped<IBasicAreaService, BasicAreaService>();

            return services;
        }
    }
}
