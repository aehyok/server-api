using DVS.Application.Services.FFP;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.FFP.Query;
using DVS.Models.Dtos.FFP.Submit;
using DVS.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;


namespace DVS.FFP.Api.Controllers
{

    /// <summary>
    /// 户码管理
    /// </summary>
    [ApiController]
    [Route("api/ffp")]
    public class HouseholdCodeController : DvsControllerBase
    {
        private readonly IFFPHouseholdCodeService service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public HouseholdCodeController(IFFPHouseholdCodeService service)
        {
            this.service = service;
        }



        /// <summary>
        /// 获取户码详情
        /// </summary>
        /// <param name="householdId"></param>
        /// <param name="isConvertDictionary">字典字段是否查询对象，1查，0不查</param>
        /// <returns></returns>
        [HttpGet("app/householdCode/GetFFPHouseholdCodeDetailAsync")]
        [AllowAnonymous]
        //[PermissionFilter("区域网格管理", 128)]
        public async Task<FFPHouseholdCodeDetailDto> GetFFPHouseholdCodeDetailAsync(int householdId, int isConvertDictionary = 0)
        {
            var data = await this.service.GetFFPHouseholdCodeDetail(householdId, isConvertDictionary);
            return data;
        }

        /// <summary>
        /// 保存户码
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("app/householdCode/SaveFFPHouseholdCodeInfoAsync")]
        //[PermissionFilter("区域网格管理", 128)]
        [AllowAnonymous]
        public async Task<bool> SaveFFPHouseholdCodeInfoAsync(FFPHouseholdCodeInfoPostDto body)
        {
            int createdBy = 297; // LoginUser.UserId;
            var data = await this.service.SaveFFPHouseholdCodeInfo(body, createdBy);
            if (!data.Flag)
            {
                throw new ValidException(data.Message);
            }
            return true;
        }

    }
}
