using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HouseholdQRCodeGenerator
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
          
                 string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../etc/dvs-qrcode.appsettings.json");
                 Console.WriteLine("config file:" + configFile);
                 if (File.Exists(configFile))
                 {
                     config.AddJsonFile(configFile);
                 }
             })
          .ConfigureLogging(a =>
          {
              a.SetMinimumLevel(LogLevel.Information);
          }).UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
