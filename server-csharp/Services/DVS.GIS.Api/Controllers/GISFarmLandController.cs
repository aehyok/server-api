using AutoMapper;
using DVS.Application.Services.GIS;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.Cons.Query;
using DVS.Models.Dtos.GIS;
using DVS.Models.Dtos.GIS.Query;
using DVS.Models.Dtos.Village.Farmland;
using DVS.Models.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.GIS.Api.Controllers.Console
{
    /// <summary>
    /// 土地信息/规划用地
    /// </summary>
    [Route("/api/gis")]
    public class GISFarmLandController : DvsControllerBase
    {
        private readonly IMapper mapper;
        private readonly IGISFarmLandService farmlandService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        public GISFarmLandController(IMapper mapper, IGISFarmLandService service)
        {
            this.mapper = mapper;
            this.farmlandService = service;
        }

        /// <summary>
        /// 获取土地信息列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("console/farmland/list")]
        [PermissionFilter("土地管理", 128)]
        public async Task<IPagedList<VillageFarmlandDto>> GetFarmListAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            model.Category = 1; // 所属类型 1 区域 ,2 园区
            model.UseFor = GetUseFor();
            return await this.farmlandService.ListFarmLandAsync(model);
        }

        /// <summary>
        /// 获取规划用地列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("console/planland/list")]
        [PermissionFilter("规划用地管理", 128)]
        public async Task<IPagedList<VillageFarmlandDto>> GetPlanListAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            model.Category = 1; // 所属类型 1 区域 ,2 园区
            model.UseFor = GetUseFor();
            return await this.farmlandService.ListFarmLandAsync(model);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("app/farmland/list")]
        [HttpPost("wechat/farmland/list")]
        public async Task<IPagedList<VillageFarmlandDto>> GetListAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            model.Category = 1; // 所属类型 1 区域 ,2 园区
            model.UseFor = GetUseFor();
            return await this.farmlandService.ListFarmLandAsync(model);
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <returns></returns>
        [HttpPost("console/farmland/add")]
        [PermissionFilter("土地管理", 1)]
        public async Task<int> AddFarmAsync([FromBody] VillageFarmland model)
        {
            LoginUser loginuser = this.LoginUser;
            if (model.Id > 0)
            {
                throw new ValidException("参数无效");
            }
            model.UseFor = GetUseFor();
            model.Category = 1; // 所属类型 1 区域 ,2 园区
            var ret = await this.farmlandService.SaveFarmLandAsync(model, loginuser.UserId);

            return ret;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("console/farmland/edit")]
        [PermissionFilter("土地管理", 4)]
        public async Task<int> UpdateFarmAsync([FromBody] VillageFarmland model)
        {
            LoginUser loginuser = this.LoginUser;
            if (model.Id == 0)
            {
                throw new ValidException("参数无效");
            }
            model.UseFor = GetUseFor();
            model.Category = 1; // 所属类型 1 区域 ,2 园区
            var ret = await this.farmlandService.SaveFarmLandAsync(model, loginuser.UserId);

            return ret;
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("console/farmland/batchupdate")]
        [PermissionFilter("土地管理", 4)]
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
            var ret = await this.farmlandService.BatchUpdateFarmLandAsync(model);

            return ret;
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <returns></returns>
        [HttpPost("console/planland/add")]
        [PermissionFilter("规划用地管理", 1)]
        public async Task<int> AddPlanAsync([FromBody] VillageFarmland model)
        {
            LoginUser loginuser = this.LoginUser;
            if (model.Id > 0)
            {
                throw new ValidException("参数无效");
            }
            model.UseFor = GetUseFor();
            model.Category = 1; // 所属类型 1 区域 ,2 园区
            var ret = await this.farmlandService.SaveFarmLandAsync(model, loginuser.UserId);

            return ret;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("console/planland/edit")]
        [PermissionFilter("规划用地管理", 4)]
        public async Task<int> UpdatePlanAsync([FromBody] VillageFarmland model)
        {
            LoginUser loginuser = this.LoginUser;
            if (model.Id == 0)
            {
                throw new ValidException("参数无效");
            }
            model.UseFor = GetUseFor();
            model.Category = 1; // 所属类型 1 区域 ,2 园区
            var ret = await this.farmlandService.SaveFarmLandAsync(model, loginuser.UserId);

            return ret;
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("console/planland/batchupdate")]
        [PermissionFilter("规划用地管理", 4)]
        public async Task<int> UpdateBacthPlanAsync([FromBody] GISBatchDto model)
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
            var ret = await this.farmlandService.BatchUpdateFarmLandAsync(model);

            return ret;
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <returns></returns>
        [HttpPost("console/farmland/detail")]
        [PermissionFilter("土地管理", 8)]
        public async Task<VillageFarmlandDto> GetFarmDetailtAsync([FromBody] GISDetailQueryModel model)
        {
            return await this.farmlandService.DetailFarmLandAsync(model.Id);

        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("console/planland/detail")]
        [PermissionFilter("规划用地管理", 8)]
        public async Task<VillageFarmlandDto> GetPlanDetailtAsync([FromBody] GISDetailQueryModel model)
        {
            return await this.farmlandService.DetailFarmLandAsync(model.Id);

        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("app/farmland/detail")]
        [HttpPost("wechat/farmland/detail")]
        public async Task<VillageFarmlandDto> GetDetailtAsync([FromBody] GISDetailQueryModel model)
        {
            return await this.farmlandService.DetailFarmLandAsync(model.Id);

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("console/farmland/delete")]
        [PermissionFilter("土地管理", 2)]
        public async Task<int> DeletetFarmAsync([FromBody] DetailQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return await this.farmlandService.DeleteFarmLandAsync(model.Id, loginuser.UserId);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("console/planland/delete")]
        [PermissionFilter("规划用地管理", 2)]
        public async Task<int> DeletetPlanAsync([FromBody] DetailQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return await this.farmlandService.DeleteFarmLandAsync(model.Id, loginuser.UserId);
        }

        /// <summary>
        /// excel导入
        /// </summary>
        /// <param name="excelFile">导入excel文件</param>
        /// <param name="areaId">区域id</param>
        /// <returns></returns>
        [HttpPost("console/farmland/import")]
        [PermissionFilter(ModuleCodes.土地管理, PermissionCodes.Import)]
        public async Task<GISImportRes> ImportAsync(IFormFile excelFile, int areaId)
        {
            LoginUser loginuser = this.LoginUser;
            int category = 1; // 所属类型 1 区域 , 2 园区
            int usefor = GetUseFor();
            Stream fileStream = excelFile.OpenReadStream();
            return await this.farmlandService.ImportExcelAsync(fileStream, category, usefor, loginuser.UserId, areaId);
        }

        /// <summary>
        /// excel导入
        /// </summary>
        /// <param name="excelFile">导入excel文件</param>
        /// <param name="areaId">区域id</param>
        /// <returns></returns>
        [HttpPost("console/planland/import")]
        [PermissionFilter(ModuleCodes.规划用地管理, PermissionCodes.Import)]
        public async Task<GISImportRes> ImportPlanAsync(IFormFile excelFile, int areaId)
        {
            LoginUser loginuser = this.LoginUser;
            int category = 1; // 所属类型 1 区域 , 2 园区
            int usefor = GetUseFor();
            Stream fileStream = excelFile.OpenReadStream();
            return await this.farmlandService.ImportExcelAsync(fileStream, category, usefor, loginuser.UserId, areaId);
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="keyword"></param>
        /// <param name="ids"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        [HttpGet("console/farmland/export")]
        [PermissionFilter(ModuleCodes.土地管理, PermissionCodes.Export)]
        public async Task<FileContentResult> ExportAsync(int areaId, string ids = "", int typeId = 0, string keyword = "")
        {
            int category = 1; // 所属类型 1 区域 , 2 园区
            int usefor = GetUseFor();
            byte[] exportBytes = await this.farmlandService.GetLandExcelData(areaId, typeId, keyword, ids, category, usefor);
            return File(exportBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="keyword"></param>
        /// <param name="ids"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        [HttpGet("console/planland/export")]
        [PermissionFilter(ModuleCodes.规划用地管理, PermissionCodes.Export)]
        public async Task<FileContentResult> ExportPlanAsync(int areaId, string ids = "", int typeId = 0, string keyword = "")
        {
            int category = 1; // 所属类型 1 区域 , 2 园区
            int usefor = GetUseFor();
            byte[] exportBytes = await this.farmlandService.GetLandExcelData(areaId, typeId, keyword, ids, category, usefor);
            return File(exportBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
        private int GetUseFor()
        {
            var path = HttpContext.Request.Path.ToString();
            var usefor = 0; // 地块用途 1 普通用地 ,2 规划用地
            var name = path.Split("/")[4];
            switch (name)
            {
                case "farmland":
                    usefor = 1;
                    break;
                case "planland":
                    usefor = 2;
                    break;
                default:
                    break;
            }
            return usefor;
        }
    }
}
