using DVS.Application.Services.Common;
using DVS.Application.Services.FFP;
using DVS.Application.Services.Village;
using Microsoft.Extensions.DependencyInjection;
using Sunlight.Application.Schedules;
using DVS.Common.Services;
using DVS.Application.Services.Village;
using DVS.Application.Services.Cons;

namespace DVS.FFP.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IBasicAreaService, BasicAreaService>();
            services.AddScoped<IBasicUserService, BasicUserService>();
            services.AddScoped<ISunFileInfoService, SunFileInfoService>();

            services.AddScoped<IFFPMatrixService, FFPMatrixService>();
            services.AddScoped<IFFPMatrixHouseholdService, FFPMatrixHouseholdService>();
            services.AddScoped<IModuleDictionaryService, ModuleDictionaryService>();
            services.AddScoped<IModuleDictionaryTypeService, ModuleDictionaryTypeService>();
            services.AddScoped<IFFPFeedbackService, FFPFeedbackService>();
            services.AddScoped<IFFPWorkflowService, FFPWorkflowService>();
            services.AddScoped<IFFPMoPaiLogService, FFPMoPaiLogService>();
            services.AddScoped<IFFPPublicityHouseholdService, FFPPublicityHouseholdService>();
            services.AddScoped<IFFPPublicityManageService, FFPPublicityManageService>();

            services.AddScoped<IFFPHouseholdCodeService, FFPHouseholdCodeService>();
            services.AddScoped<IFFPPopulationService, FFPPopulationService>();
            services.AddScoped<IHouseholdCodeService, HouseholdCodeService>();
            services.AddScoped<IPopulationService, PopulationService>();
            services.AddScoped<IPopulationTagService, PopulationTagService>();
            services.AddScoped<IBasicDictionaryService, BasicDictionaryService>();
            services.AddScoped<IHouseholdCodeTagService, HouseholdCodeTagService>();
            services.AddScoped<IFFPHouseholdExtraService, FFPHouseholdExtraService>();
            services.AddScoped<SyncFFPService>();

            services.AddScoped<IFFPApplicationService,FFPApplicationService>();
            services.AddScoped<IPopulationAddressService, PopulationAddressService>();
            services.AddScoped(typeof(IServiceBase<>), typeof(ServiceBase<>));
            services.AddScoped<IFFPAutoNumberService, FFPAutoNumberService>();
            services.AddScoped<IInfoPublicService, InfoPublicService>();
            services.AddScoped<IBasicDepartmentService, BasicDepartmentService>();
            // 定时任务
            services.AddHostedService<SyncFFPService>();

            return services;
        }
    }
}
