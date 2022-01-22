using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace DVS.IPS.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
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
                string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../etc/dvs-ips.appsettings.json");
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
