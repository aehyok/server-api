using DVS.Application.Services;
using DVS.Application.Services.Village;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DVS.Core.Domains.Village;
using X.PagedList;
using DVS.Models.Dtos.Village.Query;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Models.Dtos.Village;
using DVS.Application.Services.Common;
using DVS.Core.Domains.Common;
using DVS.Model.Dtos.Village;
using DVS.Models.Dtos.Common;
using DVS.Models.Enum;
using DVS.Common.SO;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace DVS.Village.Api.Controllers
{

    public class DevBaseFilter : ActionFilterAttribute
    {
        public string password = "a123456";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 执行 controller 之前
            base.OnActionExecuting(filterContext);
            var password = filterContext.HttpContext.Request.Query["password"].ToString();
            if (string.IsNullOrWhiteSpace(password) || password != this.password)
            {
                throw new ValidException("系统繁忙");
            }
            // var k = filterContext.HttpContext.Request.QueryString;
            // filterContext.HttpContext.Response.BodyWriter("<br />" + "执行OnActionExecuting：" + Message + "<br />");
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // 执行 controller 之后
            base.OnActionExecuted(filterContext);
            // filterContext.HttpContext.Response.Write("<br />" + "执行OnActionExecuted：" + Message + "<br />");
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
            // filterContext.HttpContext.Response.Write("<br />" + "执行OnResultExecuting：" + Message + "<br />");
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            // filterContext.HttpContext.Response.Write("<br />" + "执行OnResultExecuted：" + Message + "<br />");
        }
    }

    /// <summary>
    /// dev 后端专用相关接口
    /// </summary>
    [DevBaseFilter(password = "sunlight2021!@")]
    [Route("/api/village/dev")]
    public class DevController : DvsControllerBase
    {
        private readonly IBasicUserService basicUserService;
        public DevController(IBasicUserService basicUserService)
        {
            this.basicUserService = basicUserService;
        }



        [AllowAnonymousAttribute]
        [HttpGet("Encrypt")]
        public object Encrypt(string text, string password)
        {
            var res = BasicSO.Encrypt(text);
            return res;
        }

        [AllowAnonymousAttribute]
        [HttpPost("Decrypt")]
        public object Decrypt(string text, string password)
        {
            var res = BasicSO.Decrypt(text);
            return res;
        }


        [HttpGet("InitSO")]
        public object InitSO(string password)
        {

            var res = BasicSOInit.InitSO();
            return res;
        }


        [HttpGet("EncryptAllPopulation")]
        public async Task<int> EncryptAllPopulation([FromServices] IPopulationService service, string password)
        {
            var res = await service.EncryptAllPopulation();
            return res;
        }

        [HttpGet("DecryptAllPopulation")]
        public async Task<int> DecryptAllPopulation([FromServices] IPopulationService service, string password)
        {
            var res = await service.DecryptAllPopulation();
            return res;
        }

        /// <summary>
        /// 清空用户认证信息
        /// </summary>
        /// <param name="service"></param>
        /// <param name="userId"></param>
        /// <param name="mobile"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [AllowAnonymousAttribute]
        [HttpGet("ClearAuthUserInfo")]
        public async Task<int> ClearAuthUserInfo([FromServices] IUserAuthRecordService service, int userId, string mobile, string password)
        {
            // var res = await service.EncryptAllPopulation();
            var auth = new VillageUserAuthRecord();
            if (userId > 0)
            {
                //auth = await service.GetAsync(a => a.UserId == userId);
                //if (auth != null)
                //{
                //    await service.DeleteAsync(auth); // (" delete from VillageUserAuthRecord where userId=" + userId);
                //}
                await service.Context.Database.ExecuteSqlRawAsync("delete from VillageUserAuthRecord where userId=" + userId);
                var res = this.basicUserService.GetQueryable().Where(a => a.Id == userId).UpdateFromQuery(a => new BasicUser()
                {
                    AreaId = 0,
                    PopulationId = 0,
                    IsAuth = 0,
                    Mobile = "",
                    HouseholdId = 0,
                });

                return res;
            }
            else if (mobile != null)
            {
                mobile = BasicSO.Encrypt(mobile);
                auth = await service.GetAsync(a => a.Mobile == mobile);
                var res = 0;
                if (auth != null)
                {
                    await service.Context.Database.ExecuteSqlRawAsync("delete from VillageUserAuthRecord where userId=" + auth.UserId);
                    // await service.DeleteAsync(auth); // (" delete from VillageUserAuthRecord where userId=" + userId);
                    res = this.basicUserService.GetQueryable().Where(a => a.Id == auth.UserId).UpdateFromQuery(a => new BasicUser()
                    {
                        AreaId = 0,
                        PopulationId = 0,
                        IsAuth = 0,
                        Mobile = "",
                    });
                }

                if (res <= 0)
                {
                    res = this.basicUserService.GetQueryable().Where(a => a.Mobile == mobile && a.Type == 1).UpdateFromQuery(a => new BasicUser()
                    {
                        AreaId = 0,
                        PopulationId = 0,
                        IsAuth = 0,
                    });
                }
                return res;
            }
            return 1;
        }


        [HttpGet("GetHouseholderInfo")]
        public async Task<VillagePopulation> GetHouseholderInfo([FromServices] IPopulationService service, int householdId, string password)
        {
            var data = await service.GetHouseholderInfo(householdId);
            return data;
        }


    }
}
