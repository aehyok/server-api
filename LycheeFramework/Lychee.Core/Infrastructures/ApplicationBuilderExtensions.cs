using Lychee.Core.Middlewares;
using Lychee.Core.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseLycheeFramework(this IApplicationBuilder app, IServiceProvider services)
        {
            app.UseMiddleware<DependencyResolverMiddleware>();

            // 执行程序启动任务
            var startupTasks = services.GetServices<IStartupTask>();
            foreach (var task in startupTasks.OrderBy(a => a.Order))
            {
                task.ExecuteAsync().GetAwaiter().GetResult();
            }

            return app;
        }
    }
}