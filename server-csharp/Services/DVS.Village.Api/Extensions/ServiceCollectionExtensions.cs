using DVS.Application.Services;
using DVS.Application.Services.Common;
using DVS.Application.Services.Cons;
using DVS.Application.Services.Village;
using DVS.Application.SMS;
using DVS.Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DVS.Common.SO;
using DVS.Application.Services.GIS;
using DVS.Application.Services.Warning;

namespace DVS.Village.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IServiceBase<>), typeof(ServiceBase<>));
            services.AddScoped<IHouseholdCodeService, HouseholdCodeService>();
            services.AddScoped<IPopulationService, PopulationService>();


            services.AddScoped<IEpidemicService, EpidemicService>();
            services.AddScoped<IIncomeService, IncomeService>();
            services.AddScoped<IWorkService, WorkService>();
            services.AddScoped<IUserAuthRecordService, UserAuthRecordService>();
            services.AddScoped<IBasicDictionaryService, BasicDictionaryService>();
            services.AddScoped<IBasicUserService, BasicUserService>();
            services.AddScoped<IPopulationAddressService, PopulationAddressService>();
            services.AddScoped<IHouseholdCodeTagService, HouseholdCodeTagService>();
            services.AddScoped<IPopulationTagService, PopulationTagService>();
            services.AddScoped<ISunFileInfoService, SunFileInfoService>();
            services.AddScoped<IBasicAreaService, BasicAreaService>();
            services.AddScoped<ISendSMSService, SendSMSService>();
            services.AddScoped<IVillageFarmlandService, VillageFarmlandService>();
            services.AddScoped<IGISPlotItemService, GISPlotItemService>();
            services.AddScoped<IWarningMessageService, WarningMessageService>();
            services.AddScoped<IPushService, PushService>();

            services.AddScoped<IVaccinationService, VaccinationService>();
            return services;
        }
    }
}
