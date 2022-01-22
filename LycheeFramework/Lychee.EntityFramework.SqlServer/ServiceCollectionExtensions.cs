using Microsoft.Extensions.DependencyInjection;

namespace Lychee.EntityFramework.SqlServer
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLycheeSqlServer(this IServiceCollection services, string connectionString)
        {
            return services;
        }
    }
}