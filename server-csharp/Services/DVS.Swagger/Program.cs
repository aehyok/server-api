using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace DVS.Swagger
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
                Log.Error(ex, ex.Message);
            }

            Log.CloseAndFlush();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../etc/dvs-swagger.appsettings.json");
                Console.WriteLine("config file:" + configFile);
                if (File.Exists(configFile))
                {
                    config.AddJsonFile(configFile);
                }
            })
            .UseSerilog((context, config) =>
            {
                config.ReadFrom.Configuration(context.Configuration, AppConstants.SerilogSettingsKey);
            }).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}