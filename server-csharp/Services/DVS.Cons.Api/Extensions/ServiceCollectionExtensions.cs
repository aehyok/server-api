using DVS.Application.Services;
using DVS.Application.Services.Common;
using DVS.Application.Services.Cons;
using DVS.Application.Services.GIS;
using DVS.Application.Services.Village;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace  DVS.Cons.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IInfoPublicService, InfoPublicService>();
            services.AddScoped<IPhotoAnywhereService, PhotoAnywhereService>();
            services.AddScoped<IProduceSaleService, ProduceSaleService>();
            services.AddScoped<IServiceChannelService, ServiceChannelService>();
            services.AddScoped<IServiceGuideService, ServiceGuideService>();
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
            services.AddScoped<IGISPlotItemService, GISPlotItemService>();

            return services;
        }
    }
}
