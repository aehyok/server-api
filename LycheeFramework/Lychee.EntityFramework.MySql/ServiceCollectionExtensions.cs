using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Lychee.EntityFramework.MySql
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLycheeMySql(this IServiceCollection services, string connectionString, Action<LycheeMySqlConfiguration> mysqlConfiguration, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            var configuration = new LycheeMySqlConfiguration();
            mysqlConfiguration?.Invoke(configuration);

            if (string.IsNullOrWhiteSpace(configuration.MigrationsAssembly))
            {
                var assembly = Assembly.GetCallingAssembly().GetName();
                configuration.MigrationsAssembly = assembly.Name;
            }

            services.AddDbContext<DbContext, LycheeObjectContext>((sp, options) =>
            {
                options.UseLazyLoadingProxies(configuration.UseLazyLoadingProxies).UseMySql(connectionString, mysqlOptions =>
                {
                    if (!string.IsNullOrWhiteSpace(configuration.Version))
                    {
                        mysqlOptions.ServerVersion(new Version(configuration.Version), configuration.ServerType);
                    }

                    mysqlOptions.MigrationsAssembly(configuration.MigrationsAssembly);
                }).AddInterceptors(configuration.Interceptors);

                if (configuration.IgonreForeignKeys)
                {
                    options.ReplaceService<IMigrationsSqlGenerator, RemoveForeignKeyMigrationsSqlGenerator>();
                }
            }, serviceLifetime);

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            return services;
        }
        
        public static IServiceCollection AddLycheeMySql(this IServiceCollection services, string connectionString, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            return services.AddLycheeMySql(connectionString, (options) =>
            {
                options.UseLazyLoadingProxies = false;
            }, serviceLifetime);
        }
    }
}