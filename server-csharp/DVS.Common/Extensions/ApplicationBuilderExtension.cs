using Microsoft.AspNetCore.Builder;
using System;

namespace DVS.Common.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseRedis(this IApplicationBuilder app, Action<RedisOption> configure)
        {
            RedisOption options = new RedisOption();
            configure(options);
            string redisConnectionString = options.ConnectionString;
            if (string.IsNullOrWhiteSpace(redisConnectionString))
            {
                throw new Exception("Redis连接字符串不能为空");
            }
            var csredis = new CSRedis.CSRedisClient(redisConnectionString);
            RedisHelper.Initialization(csredis);
            //RedisHelper.Subscribe(("__keyevents@0__:expired",(message) => {
            //    string msg= message.Body;
            //}));
            return app;
        }

        public static IApplicationBuilder UseSeaSwaggerUI(this IApplicationBuilder app,Action<SwaggerUIOption> configure)
        {
            SwaggerUIOption swaggerUIOption = new SwaggerUIOption();
            configure(swaggerUIOption);
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/api/{swaggerUIOption.ModuleName.ToLower()}/swagger.json", "数字乡村2.0接口");
                options.RoutePrefix = "docs";
                options.DocumentTitle = "盛阳科技";
            });
            return app;
        }
    }

    public class SwaggerUIOption { 
        public string ModuleName { get; set; }
    }
    public class RedisOption
    {
        public string ConnectionString { get; set; }
        public bool UseKeyEventNotify { get; set; } = false;
    }
}