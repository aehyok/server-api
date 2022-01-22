using DVS.Application.Extensions;
using DVS.Common.Extensions;
using DVS.Common.Infrastructures;
using DVS.Common.MQ;
using DVS.Common.RPC;
using DVS.Core.MapProfile;
using Lychee.EntityFramework.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DVS.GIS.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private readonly string moduleName = "GIS";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {       
            //services.AddControllers();
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

            services.AddControllers(options =>
            {
                options.Filters.Add<AuthFiltterAttribute>();
                options.Filters.Add<ResourceFilter>();
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddSeaSwagger(Configuration, moduleName);

            services.AddAutoMapper(Assembly.GetAssembly(typeof(AutoMapperConfiguration)), Assembly.GetAssembly(typeof(Startup)));

            services.AddSeaService();
            services.AddServices();
            services.AddScheduleService(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
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
            app.UseLycheeFramework(services);
            app.UseRabbitMQ(option =>
            {
                option.VirtualHost = Configuration["RabbitMQ:VirtualHost"];
                option.Host = Configuration["RabbitMQ:Host"];
                option.Port = int.Parse(Configuration["RabbitMQ:Port"]);
                option.UserName = Configuration["RabbitMQ:UserName"];
                option.Password = Configuration["RabbitMQ:Password"];
            });
            app.UseRedis(option => { option.ConnectionString = Configuration.GetSection("Redis:ConnectionString").Value; });
            app.UseRpc(option =>
            {
                option.Host = Configuration["BasicRPC:HOST"];
                string port = Configuration["BasicRPC:PORT"];
                if (!string.IsNullOrWhiteSpace(port))
                {
                    option.Port = Convert.ToInt32(port);
                }
                else
                {
                    throw new Exception("请配置RPC");
                }
            });


            app.UseCors(a => a.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseRouting();

            app.UseSwagger(options =>
            {
                options.RouteTemplate = "api/{documentName}/swagger.json";
            });

            app.UseAuthentication();
            app.UseAuthorization();

            var staticFilePath = Configuration.GetValue<string>("StaticFilePath");

            //if (staticFilePath.StartsWith("."))
            //{
            //    staticFilePath = Path.GetFullPath(staticFilePath, AppContext.BaseDirectory);
            //}

            //if (!Directory.Exists(staticFilePath))
            //{
            //    Directory.CreateDirectory(staticFilePath);
            //}

            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(staticFilePath),
            //    RequestPath = "/static"
            //});
            var open = Configuration.GetValue<string>("OpenSO");
            if (open != "0")
            {
                DVS.Common.SO.BasicSOInit.InitSO(); // 初始化SO模块
            }
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
