using DVS.Application.Services.GIS;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Models.Dtos.GIS.Query;
using DVS.Models.Dtos.Village;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.GIS.Api.Controllers.Console
{
    /// <summary>
    /// 户主信息
    /// </summary>
    [Route("/api/gis/console/household")]
    public class GISHouseHoldController : DvsControllerBase
    {
        private readonly IGISHouseHoldService househouldService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        public GISHouseHoldController(IGISHouseHoldService service)
        {
            this.househouldService = service;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<IPagedList<HouseholdCodeDto>> GetListAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            model.Category = 1; // 所属类型 1 区域 ,2 园区
            return await this.househouldService.ListHouseholdCodeAsync(model);
        }
    }
}
