using AutoMapper;
using DVS.Application.Services.GIS;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Core.Domains.GIS;
using DVS.Models.Dtos.Cons.Query;
using DVS.Models.Dtos.GIS;
using DVS.Models.Dtos.GIS.Query;
using DVS.Models.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.GIS.Api.Controllers.Console
{
    /// <summary>
    /// 自定义打点信息
    /// </summary>
    [Route("/api/gis/console/custom")]
    public class GISCustomController : DvsControllerBase
    {
        private readonly IMapper mapper;
        private readonly IGISCustomService customService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        public GISCustomController(IMapper mapper, IGISCustomService service)
        {
            this.mapper = mapper;
            this.customService = service;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("list")]
        [PermissionFilter("自定义标记管理", 128)]
        public async Task<IPagedList<GISCustomDto>> GetListAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            model.Category = GetCategory(); // 所属类型 1 区域 ,2 园区
            return await this.customService.ListCustomAsync(model);
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <returns></returns>
        [HttpPost("add")]
        [PermissionFilter("自定义标记管理", 1)]
        public async Task<int> AddAsync([FromBody] GISCustom model)
        {
            model.Category = GetCategory(); // 所属类型 1 区域 ,2 园区
            LoginUser loginuser = this.LoginUser;
            int userid = loginuser != null ? loginuser.UserId : 0;
            if (model.Id == 0)
            {
                model.CreatedBy = model.CreatedBy != 0 ? model.CreatedBy : userid;
                model.UpdatedBy = model.CreatedBy;
            }
            else
            {
                throw new ValidException("参数无效");
            }
            var ret = await this.customService.SaveCustomAsync(model);

            return ret;
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <returns></returns>
        [HttpPost("edit")]
        [PermissionFilter("自定义标记管理", 4)]
        public async Task<int> UpdateAsync([FromBody] GISCustom model)
        {
            model.Category = GetCategory(); // 所属类型 1 区域 ,2 园区
            LoginUser loginuser = this.LoginUser;
            int userid = loginuser != null ? loginuser.UserId : 0;
            if (model.Id == 0)
            {
                throw new ValidException("参数无效");
            }
            else
            {
                model.UpdatedBy = model.UpdatedBy != 0 ? model.UpdatedBy : userid;
            }
            var ret = await this.customService.SaveCustomAsync(model);

            return ret;
        }
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <returns></returns>
        [HttpPost("detail")]
        [PermissionFilter("自定义标记管理", 8)]
        public async Task<GISCustomDto> GetDetailtAsync([FromBody] GISDetailQueryModel model)
        {
            return await this.customService.DetailCustomAsync(model.Id);

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        [PermissionFilter("自定义标记管理", 2)]
        public async Task<int> DeletetAsync([FromBody] DetailQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return await this.customService.DeleteCustomAsync(model.Id, loginuser.UserId);
        }

        /// <summary>
        /// excel导入
        /// </summary>
        /// <param name="excelFile">导入excel文件</param>
        /// <param name="areaId">区域id</param>
        /// <returns></returns>
        [HttpPost("import")]
        [PermissionFilter(ModuleCodes.自定义标记管理, PermissionCodes.Import)]
        public async Task<GISImportRes> ImportAsync(IFormFile excelFile, int areaId)
        {
            LoginUser loginuser = this.LoginUser;
            int category = GetCategory(); // 所属类型 1 区域 , 2 园区
            Stream fileStream = excelFile.OpenReadStream();
            return await this.customService.ImportExcelAsync(fileStream, category, loginuser.UserId, areaId, 0);
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="keyword"></param>
        /// <param name="ids"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        [HttpGet("export")]
        [PermissionFilter(ModuleCodes.自定义标记管理, PermissionCodes.Export)]
        public async Task<FileContentResult> ExportAsync(int areaId, string ids = "", int typeId = 0, string keyword = "")
        {
            int category = GetCategory(); // 所属类型 1 区域 , 2 园区
            byte[] exportBytes = await this.customService.GetExcelData(areaId, typeId, keyword, ids, category);
            return File(exportBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        private int GetCategory()
        {
            var path = HttpContext.Request.Path.ToString();
            var category = 1; // 所属类型 1 区域 ,2 园区
            var name = path.Split("/")[4];
            switch (name)
            {
                case "park":
                    category = 2;
                    break;
                default:
                    break;
            }
            return category;
        }
    }
}
