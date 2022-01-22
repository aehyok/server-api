using DVS.Application.Extensions;
using DVS.Application.Services.Common;
using DVS.Application.Services.Grpc;
using DVS.Common.Extensions;
using DVS.Common.SO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Common.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            try
            {
                BasicSOInit.InitSO(); // ³õÊ¼»¯SOÄ£¿é
            }
            catch (Exception ex)
            {

            }
        }
        private string moduleName = "Common";
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {            
            services.AddMvcCore().AddNewtonsoftJson(options => {
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            services.AddControllers();
            services.AddSeaService();
            services.AddScoped<IHWService, HWService>();
            services.AddSeaSwagger(Configuration, moduleName);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (Configuration["Log:Debug"] == "true")
            {
                StreamWriter textWriter = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs.log"), true, Encoding.UTF8);
                textWriter.AutoFlush = true;
                Console.SetOut(textWriter);
            }
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseRedis((option) =>
            {
                option.ConnectionString = Configuration["Redis:ConnectionString"];
            });
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "api/{documentName}/swagger.json";
            });
            if (env.IsDevelopment())
            {
                app.UseSeaSwaggerUI((option) =>
                {
                    option.ModuleName = moduleName;
                });
            }
        }
    }
}
