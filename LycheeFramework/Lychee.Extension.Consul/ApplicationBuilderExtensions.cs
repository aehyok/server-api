using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;

namespace Lychee.Extension.Consul
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
        {
            var assemblyName = Assembly.GetCallingAssembly().GetName();

            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            var serviceOptions = app.ApplicationServices.GetRequiredService<IOptions<ConsulServiceOptions>>().Value;

            // 服务 ID 必须保证唯一
            serviceOptions.ServiceId = Guid.NewGuid().ToString("N");

            var consulClient = new ConsulClient(config =>
            {
                config.Address = new Uri(serviceOptions.ConsulAddress);
            });

            var registration = new AgentServiceRegistration
            {
                ID = serviceOptions.ServiceId,
                Name = serviceOptions.ServiceName,
                Address = serviceOptions.ServiceAddress,
                Port = serviceOptions.ServicePort,
                Check = new AgentServiceCheck
                {
                    // 注册超时
                    Timeout = TimeSpan.FromSeconds(serviceOptions.Timeout),
                    // 服务停止多久后注销服务
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(serviceOptions.DeregisterCriticalServiceAfter),
                    // 健康检查地址
                    HTTP = serviceOptions.HealthCheck,
                    // 健康检查时间间隔
                    Interval = TimeSpan.FromSeconds(serviceOptions.Interval)
                }
            };

            consulClient.Agent.ServiceRegister(registration).Wait();

            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(serviceOptions.ServiceId).Wait();
            });

            return app;
        }
    }
}