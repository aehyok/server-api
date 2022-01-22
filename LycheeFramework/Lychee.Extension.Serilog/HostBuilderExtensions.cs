using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace Lychee.Extension.Serilog
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseLycheeSerilog(this IHostBuilder builder, IConfiguration configuration)
        {
            builder.UseSerilog((context, config) =>
            {
                config.MinimumLevel.Debug()
                .Enrich.FromLogContext();
            });

            /*
             * 
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Exceptionless("8wcS1XK0lHfDJuA2nunEct1i8vn8PMPHZZf3Zkfx", "http://172.18.4.253:8000")
                .WriteTo.File("logs\\log.txt")
                .WriteTo.Console()
                .CreateLogger();
             */

            return builder;
        }

        public static IHostBuilder UseLycheeSerilog(this IHostBuilder builder, Action<HostBuilderContext, LoggerConfiguration> configureLogger)
        {
            builder.UseSerilog(configureLogger);
            return builder;
        }
    }
}