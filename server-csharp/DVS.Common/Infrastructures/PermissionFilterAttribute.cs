using DVS.Common.ModelDtos;
using DVS.Common.Models;
using DVS.Common.RPC;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.RPC;
using DVS.Models.Enum;
using Lychee.Core.Infrastructures;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DVS.Common.Infrastructures
{
    public class PermissionFilterAttribute : ActionFilterAttribute
    {
        private readonly string moduleCode;
        private readonly int operateCode;
        private readonly string platform;

        public string ModuleCode => this.moduleCode;
        public PermissionFilterAttribute(string moduleCode, int operateCode, string platform = PlatFormName.Console)
        {
            this.moduleCode = moduleCode;
            this.operateCode = operateCode;
            this.platform = platform;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            IConfiguration configuration = ServiceLocator.Current.GetService<IConfiguration>();
            string openPermissionAuth = configuration[$"Permission:{platform}:OpenAuth"];
            if (!openPermissionAuth.IsNullOrWhiteSpace()&& openPermissionAuth.ToLower() == "false")
            {
                await next();
                return;
            }

            if (moduleCode.IsNullOrWhiteSpace() || operateCode == 0)
            {
                await next();
                return;
            }
            string token = context.HttpContext.Request.Headers["Authorization"];
            if (token.IsNullOrWhiteSpace())
            {
                token = context.HttpContext.Request.Headers["Token"];
            }
            string noPermissionTip = "您的账号没有此操作权限";
            if (!token.IsNullOrWhiteSpace())
            {
                string moduleName = configuration["Module:Name"];
                if (moduleName.IsNullOrWhiteSpace())
                {
                    throw new ValidException("服务模块名称没有配置");
                }

                string tokenKey = "sso." + Utils.MD5(token);
                string loginUserJson = await RedisHelper.GetAsync(tokenKey);
                LoginUser loginUser = null;
                if (!loginUserJson.IsNullOrWhiteSpace())
                {
                    RedisSSOVerifyResult resultInfo = JsonSerializer.Deserialize<RedisSSOVerifyResult>(loginUserJson);
                    loginUser = resultInfo.LoginUser;
                }

                if (loginUser == null)
                {
                    throw new ValidException("请先登录");
                }
                List<Permission> permissions = loginUser.Permissions[platform];

                if (permissions == null || permissions.Count == 0)
                {
                    throw new ValidException(noPermissionTip);
                }
                Permission permission = permissions.Find(p => p.ModuleCode == moduleCode);
                // 判断是否有权限
                if (permission != null &&   (  (permission.OwnPermission & operateCode) != 0 ||   (operateCode & permission.MaxPermission) == 0  ) )
                {
                    await next();
                }
                else
                {
                    throw new ValidException(noPermissionTip);
                }
            }
            else
            {
                throw new ValidException(noPermissionTip);
            }
        }
    }
}
