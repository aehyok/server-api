using DVS.Common.Extensions;
using DVS.Common.Infrastructures;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace DVS.Swagger
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(options=> {
                options.DescribeAllParametersInCamelCase();
                options.DocInclusionPredicate((name,descrition) =>  true);
            });
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger(options => {
                options.RouteTemplate = "api/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(options =>
            {
                var endpoints = Configuration.GetSection(AppConstants.SwaggerDocsSettingsKey).Get<SwaggerEndpoint[]>();

                
                if (endpoints != null && endpoints.Length > 0)
                {
                    foreach (var endpoint in endpoints)
                    {
                        options.SwaggerEndpoint(endpoint.Url, endpoint.Name);
                    }
                }

                options.RoutePrefix = Configuration.GetValue<string>(AppConstants.RoutePrefixSettingsKey);
                options.DocumentTitle = Configuration.GetValue<string>(AppConstants.DocumentTitleSerttingsKey);
            });
        }
    }
}