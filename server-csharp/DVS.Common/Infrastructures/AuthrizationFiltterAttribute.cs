using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using System.Reflection;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DVS.Common.Services.SSO;
using DVS.Models.Models.SSO;
using DVS.Common.RPC;
using DVS.Common.ModelDtos;
using DVS.Models.Dtos.RPC;
using DVS.Common.Models;
using Microsoft.Extensions.Configuration;
using Lychee.Core.Infrastructures;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Data.Common;
using System.Threading;
using System.Data;
using DVS.Common.SO;
using Z.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using DVS.Models.Enum;

namespace DVS.Common.Infrastructures
{


    public class DvsDbCommandInterceptor : Z.EntityFramework.Extensions.DbCommandInterceptor
    {
        public override void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            base.ReaderExecuted(command, interceptionContext);
        }

   
    }
    public class AuthFiltterAttribute : IAsyncActionFilter
    {
        ISSOService issoService;
        ILogger<object> logger;
        public AuthFiltterAttribute(ISSOService iSSOService, ILogger<AuthFiltterAttribute> logger)
        {
            this.issoService = iSSOService;
            this.logger = logger;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            //如果用户方位的Action带有AllowAnonymousAttribute，则不进行授权验证
            ControllerActionDescriptor descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            AllowAnonymousAttribute allowAnonymousAttribute = descriptor.MethodInfo.GetCustomAttribute<AllowAnonymousAttribute>(false);

            if (allowAnonymousAttribute != null)
            {
                await next();
                return;
            }
            else
            {
                IDVSController controllerBase = context.Controller as IDVSController;
                if (controllerBase != null)
                {
                    try
                    {
                        string token = context.HttpContext.Request.Headers["Authorization"] ;
                        if (token.IsNullOrWhiteSpace()) {
                            token = context.HttpContext.Request.Headers["Token"];
                        }
                        if (!token.IsNullOrWhiteSpace())
                        {
                            IConfiguration configuration = ServiceLocator.Current.GetService<IConfiguration>();
                            string moduleName = configuration["Module:Name"];
                            if (moduleName.IsNullOrWhiteSpace())
                            {
                                throw new ValidException("服务模块名称没有配置");
                            }

                            string tokenKey = "sso." + Utils.MD5(token);
                            // redis获取，看看是否有效，有效直接取出返回，否则调用RPC查询iang
                            string loginUserJson = await RedisHelper.GetAsync(tokenKey);
                            LoginUser loginUser=null;
                            if (!loginUserJson.IsNullOrWhiteSpace())
                            {
                                RedisSSOVerifyResult resultInfo = JsonSerializer.Deserialize<RedisSSOVerifyResult>(loginUserJson);
                                loginUser = resultInfo.LoginUser;
                            }
                            else
                            {
                                ResultModel<SSOVerifyResult> result = BasicRPC.SSOVerify(new SSOVerifyRequest() { Module = moduleName, Value = token, Extend = 60 * 10 });
                                // 获取最大的权限信息，覆写Redis
                                PermissionFilterAttribute permissionAttribute = descriptor.MethodInfo.GetCustomAttribute<PermissionFilterAttribute>(false);
                                if (permissionAttribute != null) {
                                    ResultModel<SSOVerifyWithMaxPermissionResult> resultModel =    BasicRPC.GetMaxPermissions(result.Data, permissionAttribute.ModuleCode);
                                    if (resultModel != null&&resultModel.Data!=null&& resultModel.Data.MaxPermissions!=null) {
                                        result.Data = resultModel.Data.MaxPermissions;
                                    }
                                }
                                logger.LogInformation("token验证结果:" + result.Message);

                                if (result != null)
                                {
                                    if (result.Code == -2)
                                    {
                                        throw new ValidException(result.Message, -2);
                                    }
                                    if (result.Data != null)
                                    {
                                        loginUser = new LoginUser { Account = result.Data.Account, UserId = result.Data.Uid };
                                        if (result.Data.Permissions != null)
                                        {
                                            loginUser.Permissions[PlatFormName.Console] = result.Data.Permissions.Console;
                                            loginUser.Permissions[PlatFormName.App] = result.Data.Permissions.App;
                                        }
                                        // 写入redis
                                        await RedisHelper.SetAsync(tokenKey, new RedisSSOVerifyResult { LoginUser = loginUser, VerifyResult = result.Data }, result.Data.ExpiresAt == 0 ? 2 * 60 * 60 : result.Data.ExpiresAt);
                                    }
                                }
                                else
                                {
                                    throw new ValidException("Token接口验证错误", -2);
                                }
                            }
                            controllerBase.SetLoginUser(loginUser);
                        }
                        else {
                            throw new ValidException("无效的Token", -2);
                        }
                    }
                    finally
                    {
                        if (controllerBase.LoginUser == null)
                        {
                            controllerBase.SetLoginUser(new LoginUser());
                        }
                    }
                }
                if (!context.HttpContext.Response.HasStarted)
                {
                    await next();
                }
            }
        }
    }

    public class RedisSSOVerifyResult
    {
        public LoginUser LoginUser { get; set; }
        public SSOVerifyResult VerifyResult { get; set; }
    }
}
