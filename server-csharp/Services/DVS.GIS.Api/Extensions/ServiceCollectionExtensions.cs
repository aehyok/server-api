using DVS.Application.Services.Common;
using DVS.Application.Services.GIS;
using DVS.Application.Services.Village;
using Microsoft.Extensions.DependencyInjection;
using Sunlight.Application.Schedules;

namespace DVS.GIS.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IGISPlotItemService, GISPlotItemService>();
            services.AddScoped<ISunFileInfoService, SunFileInfoService>();
            services.AddScoped<IBasicUserService, BasicUserService>();
            services.AddScoped<IBasicDepartmentService, BasicDepartmentService>();
            services.AddScoped<IBasicCategoryService, BasicCategoryService>();
            services.AddScoped<IBasicDictionaryService, BasicDictionaryService>();
            services.AddScoped<IHouseholdCodeService, HouseholdCodeService>();
            services.AddScoped<IPopulationService, PopulationService>();
            services.AddScoped<IPopulationAddressService, PopulationAddressService>();
            services.AddScoped<IHouseholdCodeTagService, HouseholdCodeTagService>();
            services.AddScoped<IPopulationTagService, PopulationTagService>();
            services.AddScoped<IBasicAreaService, BasicAreaService>();

            services.AddScoped<IGISGreenHouseService, GISGreenHouseService>();
            services.AddScoped<IGISCustomService, GISCustomService>();
            services.AddScoped<IGISCollectivePropertyService, GISCollectivePropertyService>();
            services.AddScoped<IGISCameraService, GISCameraService>();
            services.AddScoped<IVillageFarmlandService, VillageFarmlandService>();
            services.AddScoped<IGISFarmLandService, GISFarmLandService>();
            services.AddScoped<IGISHouseHoldService, GISHouseHoldService>();
            services.AddScoped<IParkAreaService, ParkAreaService>();

            services.AddScoped<IEpidemicService, EpidemicService>();
            services.AddScoped<IIncomeService, IncomeService>();
            services.AddScoped<IWorkService, WorkService>();

            // 定时任务相关服务
            services.AddHostedService<SyncPlotItemService>();
            services.AddScoped<IGISPlotItemSyncService, GISPlotItemSyncService>();

            services.AddHostedService<SyncVillageService>();
            services.AddScoped<IVillageSyncService, VillageSyncService>();

            services.AddHostedService<SyncVillageSurveyService>();
            services.AddScoped<IVillageSyncSurveyService, VillageSyncSurveyService>();

            return services;
        }
    }
}
