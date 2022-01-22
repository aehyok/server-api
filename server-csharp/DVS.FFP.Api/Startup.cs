using DVS.Application.Extensions;
using DVS.Application.Services.Grpc;
using DVS.Application.Validators;
using DVS.Common.Extensions;
using DVS.Common.Infrastructures;
using DVS.Common.ModelDtos;
using DVS.Common.RPC;
using DVS.Core.MapProfile;
using DVS.FFP.Api.Extensions;
using FluentValidation.AspNetCore;
using Lychee.EntityFramework.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

namespace DVS.FFP.Api
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
                throw new Exception("���ݿ������ַ�������Ϊ��");
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

            services.AddSeaSwagger(Configuration, "FFP");

            services.AddAutoMapper(Assembly.GetAssembly(typeof(AutoMapperConfiguration)), Assembly.GetAssembly(typeof(Startup)));

            services.AddHttpClient("Moji", options =>
            {
                options.BaseAddress = new Uri(Configuration.GetValue<string>("WeatherServer"));
                options.DefaultRequestHeaders.Add("Authorization", $"APPCODE {Configuration.GetValue<string>("WeatherAppCode")}");
            });

            services.AddSeaService();
            services.AddServices();
            //services.AddScheduleService(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        {

            if (Configuration["Log:Debug"] == "true")
            {
                StreamWriter textWriter = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs.log"), true, Encoding.UTF8);
                textWriter.AutoFlush = true;
                Console.SetOut(textWriter);
            }

            app.UseLycheeFramework(services);



            app.UseCors("cors");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRedis(option => { option.ConnectionString = Configuration.GetSection("Redis:ConnectionString").Value; });
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
                    throw new Exception("������RPC");
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
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapGrpcService<HouseholdeGrpcService>();
            });

            if (env.IsDevelopment())
            {
                app.UseSeaSwaggerUI((option) =>
                {
                    option.ModuleName = "FFP";
                });
            }

            var open = Configuration.GetValue<string>("OpenSO");
            if (open != "0")
            {
                DVS.Common.SO.BasicSOInit.InitSO(); // ��ʼ��SOģ��
            }
        }
    }
}
 
