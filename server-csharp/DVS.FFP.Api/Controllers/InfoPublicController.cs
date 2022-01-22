using DVS.Application.Services.Cons;
using DVS.Common.Models;
using DVS.Models.Dtos.Cons;
using DVS.Models.Dtos.Cons.Query;
using DVS.Models.Enum;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DVS.FFP.Api.Controllers
{
    /// <summary>
    /// 信息公开
    /// </summary>
    [Route("api/ffp")]
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
        [HttpPost("app/infopublic/list")]
        public async Task<IEnumerable<ConsInfoPublicDto>> GetListAsync(ConsListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return await this._service.ListConsInfoPublic(model.Type, model.Keyword, model.Startdate, model.Enddate, loginuser.UserId, model.Page, model.Limit, 0, PlatFormCode.APP);
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("app/infopublic/detail")]
        public Task<ConsInfoPublicDto> DetailPublicAsync(int id )
        {
            return this._service.DetailConsInfoPublic(id, PlatFormCode.APP);
        }


    }
}
