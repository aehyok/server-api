using AutoMapper;
using DVS.Application.Services.Cons;
using DVS.Common.Models;
using DVS.Core.Domains.Cons;
using DVS.Models.Dtos.Cons;
using DVS.Models.Dtos.Cons.Query;
using DVS.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DVS.Cons.Api.Controllers.Wechat
{
    /// <summary>
    /// 公开消息
    /// </summary>
    [Route("/api/cons/wechat/infopublic")]
    public class InfoPublicController : DvsControllerBase
    {

        private readonly IInfoPublicService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public InfoPublicController(IInfoPublicService service)
        {
            this._service = service;
        }

        /// <summary>
        /// 查询公开信息列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<IEnumerable<ConsInfoPublicDto>> GetList(ConsListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return  await this._service.ListConsInfoPublic(model.Type, model.Keyword, model.Startdate, model.Enddate, loginuser.UserId, model.Page, model.Limit, 0, PlatFormCode.WECHAT);
        }

        /// <summary>
        /// 查询最新动态列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("latestinfo")]
        public async Task<IEnumerable<ConsInfoPublicDto>> GetListAsync(ConsListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return await this._service.ListInfoPublic(model.Types, model.Keyword, model.Startdate, model.Enddate, loginuser.UserId, model.Page, model.Limit, 0, PlatFormCode.WECHAT);
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("detail")]
        public Task<ConsInfoPublicDto> DetailPublic(DetailQueryModel model)
        {
            return this._service.DetailConsInfoPublic(model.Id, PlatFormCode.WECHAT,LoginUser);
        }
    }
}
