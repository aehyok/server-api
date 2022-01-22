using DVS.Application.Services;
using DVS.Application.Services.Cons;
using DVS.Application.Services.SunFSAgent;
using DVS.Application.Services.Village;
using DVS.Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DVS.SunFSAgent.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IServiceBase<>), typeof(ServiceBase<>));
            return services;
        }
    }
}
