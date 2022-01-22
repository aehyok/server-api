using DVS.Application.Services.Common;
using DVS.Application.Services.FFP;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Core.Domains.Common;
using DVS.Models.Dtos.FFP.Query;
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
    [ApiController]
    [Route("api/ffp/console/dictionary")]
    public class DictionaryController : DvsControllerBase
    {
        IModuleDictionaryService moduleDictionaryService;
        IModuleDictionaryTypeService moduleDictionaryTypeService;
        public DictionaryController(
            IModuleDictionaryService moduleDictionaryService,
            IModuleDictionaryTypeService moduleDictionaryTypeService) {
            this.moduleDictionaryService = moduleDictionaryService;
            this.moduleDictionaryTypeService = moduleDictionaryTypeService;
        }
        /// <summary>
        /// 获取字典类型列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("types")]
        [PermissionFilter(ModuleCodes.防返贫字典, PermissionCodes.List)]
        public async Task<List<ModuleDictionaryType>> Types([FromBody] ModuleDictionaryTypeListReq req) {
            return await moduleDictionaryTypeService.GetMdouleDictionaryTypeAsync(req.Keyword,"100");
        }
        /// <summary>
        /// 获取字典类型详情
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("typeDetail")]
        [PermissionFilter(ModuleCodes.防返贫字典, PermissionCodes.View)]
        public async Task<ModuleDictionaryType> TypeDetail([FromBody] ModuleDictionaryTypeDetailReq req)
        {
            return await moduleDictionaryTypeService.GetModuleDictionaryTypeDetailAsync(req.TypeCode);
        }
        /// <summary>
        /// 获取字典的详情
        /// </summary>
        /// <param name="req">typeCode:字典类型编码，moduleCode模块的编码</param>
        /// <returns></returns>
        [HttpPost("detail")]
        [PermissionFilter(ModuleCodes.防返贫字典, PermissionCodes.View)]
        public async Task<ModuleDictionary> Detail([FromBody] ModuleDictionaryReq req)  {
            return await moduleDictionaryService.GetModuleDictionaryDetailAsync(req.Code);
        }

        /// <summary>
        /// 获取字典列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("page")]
        [PermissionFilter(ModuleCodes.防返贫字典, PermissionCodes.List)]
        public async Task<IPagedList<ModuleDictionary>> PageList([FromBody] ModuleDictionaryPageListReq req)
        {
            return await moduleDictionaryService.GetPageModuleDictionaryAsync(req.TypeCode,req.Keyword,req.Page,req.Limit);
        }
        /// <summary>
        /// 获取指定类型下的全部字典项
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("list")]
        [PermissionFilter(ModuleCodes.防返贫字典, PermissionCodes.List)]
        public async Task<List<ModuleDictionary>> List([FromBody] ModuleDictionaryListReq req)
        {
            return await moduleDictionaryService.GetModuleDictionaryAsync(req.TypeCodes, req.Keyword);
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("status")]
        [PermissionFilter(ModuleCodes.防返贫字典, PermissionCodes.Edit)]
        public async Task<bool> ChangeStatus([FromBody] ModuleDictionaryStatusReq req) {
            return await moduleDictionaryService.ChangeStatus(req.Code,req.Status);
        }

        /// <summary>
        /// 编辑字典信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("edit")]
        [PermissionFilter(ModuleCodes.防返贫字典, PermissionCodes.Edit)]
        public async Task<bool> Edit([FromBody] ModuleDictionaryEditReq req)
        {
            if (req.Id <= 0) {
                throw new Exception("id必须大于0");
            }
            return await moduleDictionaryService.Save(req);
        }

        [HttpGet("export")]
        public async Task<FileContentResult> Export([FromQuery] string typeCode, [FromQuery] string ids, string keyword) {
            if (typeCode.IsNullOrWhiteSpace()) {
                throw new ValidException("请选择一个类型进行导出");
            }
            List<int> dictionaryIds=new List<int>();
            if (!ids.IsNullOrWhiteSpace())
            {
                dictionaryIds = ids.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(id => Convert.ToInt32(id)).ToList();
            }
            byte[] farmlandBytes = await moduleDictionaryService.GetExcelData(typeCode, dictionaryIds,  keyword);
            return File(farmlandBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
