using DVS.Application.Extensions;
using DVS.Application.Services.Grpc;
using DVS.Application.Validators;
using DVS.Common.Extensions;
using DVS.Common.Infrastructures;
using DVS.Common.ModelDtos;
using DVS.Common.MQ;
using DVS.Common.RPC;
using DVS.Core.MapProfile;
using DVS.Models.Models.SunFile;
using FluentValidation.AspNetCore;
using Lychee.EntityFramework.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DVS.Village.Api
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
            services.AddGrpc();
            services.AddSwaggerGen(optios => {
                optios.OperationFilter<SwaggerOperationFilter>();
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

            services.AddControllers(options =>
            {
                options.Filters.Add<AuthFiltterAttribute>();
               
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }).AddFluentValidation(o => o.RegisterValidatorsFromAssembly(Assembly.GetAssembly(typeof(ValidatorBase<>))));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (context) =>
                {
                    var errors = context.ModelState.Values.SelectMany(a => a.Errors.Select(c => c.ErrorMessage));

                    var resultModel = new ResultModel<object>();
                    resultModel.Code = -1;
                    resultModel.Message = errors.FirstOrDefault();
                    resultModel.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    resultModel.Data = errors;

                    return new DvsResult(resultModel);
                };
            });

            services.AddSeaSwagger(Configuration, "Village");

            services.AddAutoMapper(Assembly.GetAssembly(typeof(AutoMapperConfiguration)), Assembly.GetAssembly(typeof(Startup)));

            services.AddHttpClient("Moji", options =>
            {
                options.BaseAddress = new Uri(Configuration.GetValue<string>("WeatherServer"));
                options.DefaultRequestHeaders.Add("Authorization", $"APPCODE {Configuration.GetValue<string>("WeatherAppCode")}");
            });

            services.AddSeaService();
            services.AddServices();
            services.AddScheduleService(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        {

            if (Configuration["Log:Debug"] == "true") {
                StreamWriter textWriter = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs.log"), true, Encoding.UTF8);
                textWriter.AutoFlush = true;
                Console.SetOut(textWriter);
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

            app.UseCors("cors");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRedis(option => { option.ConnectionString = Configuration.GetSection("Redis:ConnectionString").Value; });

   

            
            app.UseCors(a => a.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseRouting();
            app.UseRpc((option) =>
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

            app.UseSwagger(options =>
            {
                options.RouteTemplate = "api/{documentName}/swagger.json";
            });

            app.UseAuthentication();
            app.UseAuthorization();

            //var staticFilePath = Configuration.GetValue<string>("StaticFilePath");

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


            //FileCleanTask task = new FileCleanTask() { FilePath = "/usr/local" };

            //task.SendDelayMessage(option=> {
            //    option.VirtualHost = Configuration["RabbitMQ:VirtualHost"];
            //    option.Host = Configuration["RabbitMQ:Host"];
            //    option.Port = int.Parse(Configuration["RabbitMQ:Port"]);
            //    option.UserName = Configuration["RabbitMQ:UserName"];
            //    option.Password = Configuration["RabbitMQ:Password"];
            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<HouseholdeGrpcService>();
            });

            if (env.IsDevelopment())
            {
                app.UseSeaSwaggerUI((option) =>
                {
                    option.ModuleName = "Village";
                });
            }
 
            var open = Configuration.GetValue<string>("OpenSO");
            if (open != "0")
            {
                DVS.Common.SO.BasicSOInit.InitSO(); // 初始化SO模块
            }
        }
    }
}