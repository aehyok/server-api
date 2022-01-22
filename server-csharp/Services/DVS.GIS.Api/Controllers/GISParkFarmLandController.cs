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
    /// 园区土地信息
    /// </summary>
    [Route("/api/gis/console/park/farmland")]
    public class GISParkFarmLandController : DvsControllerBase
    {
        private readonly IMapper mapper;
        private readonly IGISFarmLandService farmlandService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="service"></param>
        public GISParkFarmLandController(IMapper mapper, IGISFarmLandService service)
        {
            this.mapper = mapper;
            this.farmlandService = service;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("list")]
        [PermissionFilter("企业地块标绘管理", 128)]
        public async Task<IPagedList<VillageFarmlandDto>> GetListAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            model.Category = GetCategory(); // 所属类型 1 区域 ,2 园区
            model.UseFor = GetUseFor();
            return await this.farmlandService.ListFarmLandAsync(model);
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <returns></returns>
        [HttpPost("add")]
        [PermissionFilter("企业地块标绘管理", 1)]
        public async Task<int> AddAsync([FromBody] VillageFarmland model)
        {
            LoginUser loginuser = this.LoginUser;
            if (model.Id > 0)
            {
                throw new ValidException("参数无效");
            }
            model.UseFor = GetUseFor();
            model.Category = GetCategory(); // 所属类型 1 区域 ,2 园区
            var ret = await this.farmlandService.SaveFarmLandAsync(model, loginuser.UserId);

            return ret;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("edit")]
        [PermissionFilter("企业地块标绘管理", 4)]
        public async Task<int> UpdateAsync([FromBody] VillageFarmland model)
        {
            LoginUser loginuser = this.LoginUser;
            if (model.Id == 0)
            {
                throw new ValidException("参数无效");
            }
            model.UseFor = GetUseFor();
            model.Category = GetCategory(); // 所属类型 1 区域 ,2 园区
            var ret = await this.farmlandService.SaveFarmLandAsync(model, loginuser.UserId);

            return ret;
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <returns></returns>
        [HttpPost("detail")]
        [PermissionFilter("企业地块标绘管理", 8)]
        public async Task<VillageFarmlandDto> GetDetailtAsync([FromBody] GISDetailQueryModel model)
        {
            return await this.farmlandService.DetailFarmLandAsync(model.Id);

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        [PermissionFilter("企业地块标绘管理", 2)]
        public async Task<int> DeletetAsync([FromBody] DetailQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return await this.farmlandService.DeleteFarmLandAsync(model.Id, loginuser.UserId);
        }

        /// <summary>
        /// excel导入
        /// </summary>
        /// <param name="excelFile">导入excel文件</param>
        /// <param name="areaId">区域id</param>
        /// <param name="parkId">园区id</param>
        /// <returns></returns>
        [HttpPost("import")]
        [PermissionFilter(ModuleCodes.企业地块标绘管理, PermissionCodes.Import)]
        public async Task<GISImportRes> ImportAsync(IFormFile excelFile, int areaId, int parkId)
        {
            LoginUser loginuser = this.LoginUser;
            int category = 2; // 所属类型 1 区域 , 2 园区
            int usefor = GetUseFor();
            Stream fileStream = excelFile.OpenReadStream();
            return await this.farmlandService.ImportExcelAsync(fileStream, category, usefor, loginuser.UserId, areaId, parkId);
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
        [PermissionFilter(ModuleCodes.企业地块标绘管理, PermissionCodes.Export)]
        public async Task<FileContentResult> ExportAsync(int areaId, string ids = "", int typeId = 0, string keyword = "", int parkId = 0)
        {
            int category = 2; // 所属类型 1 区域 , 2 园区
            int usefor = GetUseFor();
            if (parkId == 0) {
                throw new ValidException("parkId必须大于0");
            }
            byte[] exportBytes = await this.farmlandService.GetLandExcelData(areaId, typeId, keyword, ids, category, usefor, parkId);
            return File(exportBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        /// <summary>
        /// 返回地块用途 1 普通用地 ,2 规划用地
        /// </summary>
        /// <returns></returns>
        private int GetUseFor()
        {
            var path = HttpContext.Request.Path.ToString();
            var usefor = 0; // 地块用途 1 普通用地 ,2 规划用地
            var name = path.Split("/")[5];
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

        

        /// <summary>
        /// 返回所属类型 1 区域 ,2 园区
        /// </summary>
        /// <returns></returns>
        private int GetCategory()
        {
            var path = HttpContext.Request.Path.ToString();
            var category = 2; // 所属类型 1 区域 ,2 园区
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
