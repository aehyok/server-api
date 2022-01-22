using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Common.Api
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
                   //StreamWriter textWriter = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs.log"), true, Encoding.UTF8);
                   //textWriter.AutoFlush = true;
                   //Console.SetOut(textWriter);
                   string commonConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../etc/appsettings.json");
                   if (File.Exists(commonConfig))
                   {
                       config.AddJsonFile(commonConfig);
                   }
                   string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../etc/dvs-common.appsettings.json");
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
