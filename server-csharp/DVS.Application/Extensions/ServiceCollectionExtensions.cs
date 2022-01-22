using DVS.Application.Services.SSO;
using DVS.Common.Services;
using DVS.Common.Services.SSO;
using DVS.Models.Models.SSO;
using Microsoft.Extensions.DependencyInjection;

namespace DVS.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSeaService(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddHttpClient<SSOService >();
            services.AddScoped(typeof(IServiceBase<>), typeof(ServiceBase<>));
            services.AddScoped(typeof(ISSOService), typeof(SSOService ));
            return services;
        }
    }
}
