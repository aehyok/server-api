using DVS.Common.Infrastructures;
using DVS.Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DVS.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSeaSwagger(this IServiceCollection services, IConfiguration configuration,string moduleName)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(moduleName.ToLower(), new OpenApiInfo { Title = "数字乡村2.2", Version = "v1.0" });

                // 为文档中的枚举值添加描述信息
                options.DocumentFilter<SwaggerEnumDescriptions>();

                var apiCommentPath = Path.Combine(AppContext.BaseDirectory, $"DVS.{moduleName.ToLower()}.Api.xml");
                if (File.Exists(apiCommentPath))
                {
                    options.IncludeXmlComments(apiCommentPath,true);
                }

                var domainCommentPath = Path.Combine(AppContext.BaseDirectory, "DVS.Models.xml");
                var dvsCoreModelPath = Path.Combine(AppContext.BaseDirectory, "DVS.Core.xml");
                if (File.Exists(domainCommentPath))
                {
                    options.IncludeXmlComments(domainCommentPath);
                }

                if (File.Exists(dvsCoreModelPath))
                {
                    options.IncludeXmlComments(dvsCoreModelPath);
                }

                options.OperationFilter<SwaggerOperationFilter>();

            });

            services.AddSwaggerGenNewtonsoftSupport();

            return services;
        }

        public static IServiceCollection AddScheduleService(this IServiceCollection services, IConfiguration configuration)
        {

            return services;
        }
    }
}
