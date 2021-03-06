using DVS.Application.Services.FFP;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.FFP.Query;
using DVS.Models.Dtos.FFP.Submit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DVS.FFP.Api.Controllers
{
    /// <summary>
    /// 户详情扩展信息
    /// </summary>
    [ApiController]
    [Route("api/ffp/console/householdExtra")]
    public class HouseholdExtraController : DvsControllerBase
    {
        IFFPHouseholdExtraService ffpHouseholdExtraService;
        public HouseholdExtraController(IFFPHouseholdExtraService ffpHouseholdExtraService)
        {
            this.ffpHouseholdExtraService = ffpHouseholdExtraService;
        }

        /// <summary>
        /// 获取农户的详情
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("info")]
        public async Task<List<FFPHouseholdExtraInfo>> InfoAsync([FromBody] FFPHouseholdExtraReq req)
        {
            return await ffpHouseholdExtraService.Info(req.HouseholdId, req.TypeCodes);
        }
        /// <summary>
        /// 保存农户的扩展信息
        /// </summary>
        /// <param name="extraInfos">农户的扩展信息</param>
        /// <returns></returns>
        [HttpPost("save")]
        public async Task<bool> SaveAsync([FromBody] HouseholdeExtraSaveReq extraInfos)
        {
            return await ffpHouseholdExtraService.Save(extraInfos, this.LoginUser);
        }
    }
}
