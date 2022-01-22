using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DVS.Open.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {

            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Log.Error(ex, ex.Message);
            }

            Log.CloseAndFlush();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
                       Host.CreateDefaultBuilder(args)
              .ConfigureAppConfiguration((context, config) =>//在Build()的时候执行,ConfigureAppConfiguration可以调用多次，相同的配置项，以最后执行的为准
              {
                  string commonConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../etc/appsettings.json");
                  if (File.Exists(commonConfig))
                  {
                      config.AddJsonFile(commonConfig);
                  }
                  string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../etc/dvs-open.appsettings.json");
                  if (File.Exists(configFile))
                  {
                      config.AddJsonFile(configFile);
                  }

              })
            
            .UseSerilog((context, config) =>
            {
                config.ReadFrom.Configuration(context.Configuration);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
