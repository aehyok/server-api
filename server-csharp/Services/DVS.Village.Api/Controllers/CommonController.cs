using DVS.Common.Infrastructures;
using DVS.Core.Domains.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DVS.Application.Services.Common;
using DVS.Models.Dtos.Common;

namespace DVS.Village.Api.Controllers
{
    /// <summary>
    /// 公共接口
    /// </summary>
    [Route("/api/village/console/")]
    public class CommonController : DvsControllerBase
    {


        /// <summary>
        /// 获取字典列表
        /// </summary>
        /// <param name="service"></param>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        [HttpGet("GetBasicDictionaryList")]
        public async Task<IEnumerable<BasicDictionary>> GetBasicDictionaryList([FromServices] IBasicDictionaryService service,int typeCode) {

            var data = await service.GetBasicDictionaryList(typeCode);
            return data;
        }
        /// <summary>
        /// 获取区域树结构
        /// </summary>
        /// <param name="service"></param>
        /// <param name="areaId">父节点，传0就是获取全部的树</param>
        /// <returns></returns>
        [HttpGet("GetAreaTree")]
        public async Task<BasicAreaTreeDto> GetAreaTree([FromServices] IBasicAreaService service,int areaId = 0) {

            var data = await service.GetBasicAreaTree(areaId);
            return data;
        }

    }
}
