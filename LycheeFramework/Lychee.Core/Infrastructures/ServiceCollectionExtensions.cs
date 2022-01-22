using Lychee.Core.Tasks;
using Lychee.TypeFinder;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLycheeFramework(this IServiceCollection services)
        {
            return services;
        }

        public static IServiceCollection AddStartupTask(this IServiceCollection services)
        {
            var tasks = FindTypes.InAllAssemblies.ThatInherit<IStartupTask>().ToList();

            foreach(var task in tasks)
            {
                services.Add(new ServiceDescriptor(typeof(IStartupTask), task, ServiceLifetime.Singleton));
            }

            return services;
        }
    }
}