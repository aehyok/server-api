using DVS.Application.Schedules;
using DVS.Application.Services.Common;
using DVS.Application.Services.IPS;
using Microsoft.Extensions.DependencyInjection;
using Sunlight.Application.Schedules;

namespace DVS.IPS.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IIPSCompanyService, IPSCompanyService>();
            services.AddScoped<IIPSDeviceService, IPSDeviceService>();
            services.AddScoped<IIPSMessageService, IPSMessageService>();
            services.AddScoped<IIPSScheduleService, IPSScheduleService>();

            services.AddScoped<ISunFileInfoService, SunFileInfoService>();
            services.AddScoped<IBasicUserService, BasicUserService>();
			services.AddScoped<IBasicAreaService, BasicAreaService>();
            services.AddScoped<SyncIPSService>();


            // 定时任务相关服务
            services.AddHostedService<SyncIPSDataService>();
            services.AddHostedService<SyncIPSService>();

            return services;
        }
    }
}
