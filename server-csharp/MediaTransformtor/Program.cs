using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MediaTransformtor
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
                    string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../etc/dvs-mediaTransformtor.appsettings.json");
                    Console.WriteLine("config file:" + configFile);
                    if (File.Exists(configFile))
                    {
                        config.AddJsonFile(configFile);
                    }

                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
