using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using Serilog;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;

namespace DVS.Cons.Api
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
            .ConfigureAppConfiguration((context, config) =>
            {
                string commonConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../etc/appsettings.json");
                if (File.Exists(commonConfig))
                {
                    config.AddJsonFile(commonConfig);
                }
                string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../etc/dvs-cons.appsettings.json");
                Console.WriteLine("config file:" + configFile);
                if (File.Exists(configFile))
                {
                    config.AddJsonFile(configFile);
                }
            })
            .UseSerilog((context, config) =>
            {
                config.ReadFrom.Configuration(context.Configuration);
            }).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
