using Lychee.Core.Infrastructures;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Lychee.Core.Middlewares
{
    public class DependencyResolverMiddleware
    {
        private readonly RequestDelegate _next;

        public DependencyResolverMiddleware(RequestDelegate next) => this._next = next;

        public async Task InvokeAsync(HttpContext httpContext)
        {
            ServiceLocator.Current.ResolverFunc = (type) =>
            {
                return httpContext.RequestServices.GetService(type);
            };

            await _next(httpContext);
        }
    }
}