using DVS.Application.Services.Common;
using DVS.Application.Services.FFP;
using DVS.Common.Infrastructures;
using DVS.Core.Domains.Common;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.FFP.Query;
using DVS.Models.Dtos.FFP.Submit;
using DVS.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.FFP.Api.Controllers
{
    /// <summary>
    /// 申请书
    /// </summary>
    [ApiController]
    [Route("api/ffp/app/application")]
    public class ApplicationAppController : DvsControllerBase
    {
        IFFPApplicationService applicationService;
 
        public ApplicationAppController(
            IFFPApplicationService applicationService
         )
        {
            this.applicationService = applicationService;
        }

        /// <summary>
        /// 获取申请书的详情
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("detail")]
        public async Task<FFPApplicationDto> Detail([FromBody] FFApplicationDetailReq req)
        {
            return await applicationService.Detail(req.WorkflowId);
        }

        /// <summary>
        /// 编辑申请书
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("edit")]
        public async Task<int> Edit([FromBody] FFPApplicationEditReq req)
        {
            return await applicationService.SaveApplication(req,LoginUser.UserId);
        }

        /// <summary>
        /// 获取申请书的修改日志
        /// </summary>
        /// <param name="applicationId">申请书的id</param>
        /// <returns></returns>
        [HttpGet("logs")]
        public async Task<List<FFPApplicationLogDto>> ApplicationLogs([FromQuery] int applicationId) {
            return await applicationService.GetApplicationLogs(applicationId);
        }
    }
}
