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

namespace DVS.Cons.Api.Controllers.App
{
    /// <summary>
    /// 公开消息
    /// </summary>
    [Route("/api/cons/app/infopublic")]
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
        public async Task<IEnumerable<ConsInfoPublicDto>> GetListAsync(ConsListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return  await this._service.ListConsInfoPublic(model.Type, model.Keyword, model.Startdate, model.Enddate, loginuser.UserId, model.Page, model.Limit, 0, PlatFormCode.APP);
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("detail")]
        public Task<ConsInfoPublicDto> DetailPublicAsync(DetailQueryModel model)
        {
            return this._service.DetailConsInfoPublic(model.Id, 0);
        }
    }
}
