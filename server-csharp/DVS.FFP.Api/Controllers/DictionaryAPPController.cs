using DVS.Application.Services.Common;
using DVS.Core.Domains.Common;
using DVS.FFP.Api.Controllers;
using DVS.Models.Dtos.FFP.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DVS.Application.Services.FFP
{
    /// <summary>
    /// APP端字典接口
    /// </summary>
    [ApiController]
    [Route("api/ffp/app/dictionary")]
    public class DictionaryAPPController : DvsControllerBase
    {
        IModuleDictionaryService moduleDictionaryService;
        IModuleDictionaryTypeService moduleDictionaryTypeService;
        public DictionaryAPPController(
            IModuleDictionaryService moduleDictionaryService,
            IModuleDictionaryTypeService moduleDictionaryTypeService)
        {
            this.moduleDictionaryService = moduleDictionaryService;
            this.moduleDictionaryTypeService = moduleDictionaryTypeService;
        }

        /// <summary>
        /// 获取字典类型列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("types")]
        public async Task<List<ModuleDictionaryType>> Types([FromBody] ModuleDictionaryTypeListReq req)
        {
            return await moduleDictionaryTypeService.GetMdouleDictionaryTypeAsync(req.Keyword,"100");
        }

        /// <summary>
        /// 获取指定类型下的全部字典项
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<List<ModuleDictionary>> List([FromBody] ModuleDictionaryListReq req)
        {
            return await moduleDictionaryService.GetModuleDictionaryAsync(req.TypeCodes, req.Keyword);
        }
    }
}
