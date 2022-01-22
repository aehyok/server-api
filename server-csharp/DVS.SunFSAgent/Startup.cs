using DVS.Application.Extensions;
using DVS.Application.Schedules;
using DVS.Application.Services.SunFSAgent;
using DVS.Common.Extensions;
using DVS.Core.MapProfile;
using DVS.SunFSAgent.Api;
using Lychee.EntityFramework.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DVS.SunFSAgent.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private readonly string moduleName = "SunFSAgent";
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FormOptions>(options => {
                options.MultipartBodyLengthLimit = int.MaxValue;
                options.MultipartBoundaryLengthLimit = int.MaxValue;
                options.ValueLengthLimit = int.MaxValue;
            });

            services.AddLycheeFramework();

            var connectionString = Configuration.GetConnectionString("MySQL");

            if (connectionString.IsNullOrWhiteSpace())
            {
                throw new Exception("数据库连接字符串不能为空");
            }

            services.AddLycheeMySql(connectionString, options =>
            {
                options.ServerType = Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MariaDb;
                options.UseLazyLoadingProxies = false;
                options.MigrationsAssembly = typeof(Startup).Assembly.GetName().Name;
                options.IgonreForeignKeys = true;
            });

            services.AddHostedService<SunfileMediaTransform>();
            services.AddAutoMapper(Assembly.GetAssembly(typeof(AutoMapperConfiguration)), Assembly.GetAssembly(typeof(Startup)));
            services.AddControllers();
            services.AddSeaSwagger(Configuration, moduleName);
            services.AddSeaService();
            services.AddServices();
            string store = Configuration["File:Store"];
            switch (store.ToLower())
            {
                case "sundfs":
                    services.AddScoped<IFileService, SunFileUploadService>();
                    break;
                case "localstorge":
                    services.AddScoped<IFileService, LocalStoreFileService>();
                    break;
                case "hwobs":
                    services.AddScoped<IFileService, IHWOBSService>();
                    break;
                default:
                    services.AddScoped<IFileService, SunFileUploadService>();
                    break;
            }

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider service)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRedis(option => { option.ConnectionString = Configuration.GetSection("Redis:ConnectionString").Value; });
            app.UseLycheeFramework(service);

            app.UseSwagger(options =>
            {
                options.RouteTemplate = "api/{documentName}/swagger.json";
            });

            string store = Configuration["File:Store"];
            if (store == "LocalStorge")
            {
                string fileDir = Path.Combine(Configuration[$"File:{store}:UploadDir"]);
                if (!Directory.Exists(fileDir)) {
                    Directory.CreateDirectory(fileDir);
                }
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(fileDir),
                    RequestPath = Configuration[$"File:{store}:AccessPath"]
                });
                //开启目录流量
                app.UseDirectoryBrowser(new DirectoryBrowserOptions
                {
                    FileProvider = new PhysicalFileProvider(fileDir),
                    RequestPath = Configuration[$"File:{store}:AccessPath"]
                });
            }

 

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
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
