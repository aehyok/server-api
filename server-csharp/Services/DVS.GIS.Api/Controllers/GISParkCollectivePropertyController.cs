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
    /// 园区公共设施信息
    /// </summary>
    [Route("/api/gis/console/park/collectiveproperty")]
    public class GISParkCollectivePropertyController : DvsControllerBase
    {
        private readonly IMapper mapper;
        private readonly IGISCollectivePropertyService propertyService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        public GISParkCollectivePropertyController(IMapper mapper, IGISCollectivePropertyService service)
        {
            this.mapper = mapper;
            this.propertyService = service;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("list")]
        [PermissionFilter("企业设施标记管理", 128)]
        public async Task<IPagedList<GISCollectivePropertyDto>> GetListAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            model.Category = GetCategory(); // 所属类型 1 区域 ,2 园区
            return await this.propertyService.ListCollectivePropertyAsync(model);
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <returns></returns>
        [HttpPost("add")]
        [PermissionFilter("企业设施标记管理", 1)]
        public async Task<int> AddAsync([FromBody] GISCollectiveProperty model)
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
            var ret = await this.propertyService.SaveCollectivePropertyAsync(model);

            return ret;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("edit")]
        [PermissionFilter("企业设施标记管理", 4)]
        public async Task<int> UpdateAsync([FromBody] GISCollectiveProperty model)
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
            var ret = await this.propertyService.SaveCollectivePropertyAsync(model);

            return ret;
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("batchupdate")]
        [PermissionFilter("企业设施标记管理", 4)]
        public async Task<int> UpdateBacthAsync([FromBody] GISBatchDto model)
        {
            LoginUser loginuser = this.LoginUser;
            int userid = loginuser != null ? loginuser.UserId : 0;
            if (model.Ids.Count == 0)
            {
                throw new ValidException("参数无效");
            }
            else
            {
                model.UpdatedBy = model.UpdatedBy != 0 ? model.UpdatedBy : userid;
            }
            var ret = await this.propertyService.BatchUpdateCollectivePropertyAsync(model);

            return ret;
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <returns></returns>
        [HttpPost("detail")]
        [PermissionFilter("企业设施标记管理", 8)]
        public async Task<GISCollectivePropertyDto> GetDetailtAsync([FromBody] GISDetailQueryModel model)
        {
            return await this.propertyService.DetailCollectivePropertyAsync(model.Id);

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        [PermissionFilter("企业设施标记管理", 2)]
        public async Task<int> DeletetAsync([FromBody] DetailQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return await this.propertyService.DeleteCollectivePropertyAsync(model.Id, loginuser.UserId);
        }

        /// <summary>
        /// excel导入
        /// </summary>
        /// <param name="excelFile">导入文件名</param>
        /// <param name="areaId">区域id</param>
        /// <param name="parkId">园区id</param>
        /// <returns></returns>
        [HttpPost("import")]
        [PermissionFilter(ModuleCodes.企业设施标记管理, PermissionCodes.Import)]
        public async Task<GISImportRes> ImportAsync(IFormFile excelFile, int areaId, int parkId)
        {
            LoginUser loginuser = this.LoginUser;
            int category = GetCategory(); // 所属类型 1 区域 , 2 园区
            Stream fileStream = excelFile.OpenReadStream();
            return await this.propertyService.ImportExcelAsync(fileStream, category, loginuser.UserId, areaId, parkId);
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="keyword"></param>
        /// <param name="ids"></param>
        /// <param name="areaId"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        [HttpGet("export")]
        [PermissionFilter(ModuleCodes.企业设施标记管理, PermissionCodes.Export)]
        public async Task<FileContentResult> ExportAsync(int areaId, string ids = "", int typeId = 0, string keyword = "", int parkId = 0)
        {
            int category = 2; // 所属类型 1 区域 , 2 园区
            if (parkId == 0)
            {
                throw new ValidException("parkId必须大于0");
            }
            byte[] exportBytes = await this.propertyService.GetExcelData(areaId, typeId, keyword, ids, category, parkId);
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
