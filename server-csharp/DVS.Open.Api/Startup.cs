using DVS.Application.Extensions;
using DVS.Application.Services.Grpc;
using DVS.Core.MapProfile;
using Lychee.EntityFramework.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DVS.Open.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddServices();
            services.AddSeaService();
            services.AddAutoMapper(Assembly.GetAssembly(typeof(AutoMapperConfiguration)), Assembly.GetAssembly(typeof(Startup)));
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

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseLycheeFramework(services);
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<HouseholdeGrpcService>();
                endpoints.MapGrpcService<ServicechannelGrpcService>();
                endpoints.MapGrpcService<ServiceguideGrpcService>();
            });

            var open = Configuration.GetValue<string>("OpenSO");
            if (open != "0")
            {
                DVS.Common.SO.BasicSOInit.InitSO(); // 初始化SO模块
            }
        }
    }
}
