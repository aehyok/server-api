using AutoMapper;
using DVS.Application.Services.GIS;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Core.Domains.GIS;
using DVS.Models.Dtos.Cons.Query;
using DVS.Models.Dtos.GIS;
using DVS.Models.Dtos.GIS.Query;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.GIS.Api.Controllers.Console
{
    /// <summary>
    /// 园区地理信息
    /// </summary>
    [Route("/api/gis/console/park/plotitem")]
    public class GISParkPlotItemController : DvsControllerBase
    {
        private readonly IMapper mapper;
        private readonly IGISPlotItemService plotitemService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="plotitemService"></param>
        public GISParkPlotItemController(IMapper mapper, IGISPlotItemService plotitemService)
        {
            this.mapper = mapper;
            this.plotitemService = plotitemService;
        }

        /// <summary>
        /// 获取打点详情列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("farmland/list")]
        [PermissionFilter("企业地块标绘", 128)]
        public async Task<IPagedList<GISPlotItemDto>> ListFarmLandPlotItemAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            var plottype = GetPlotType();
            model.PlotType = plottype;
            model.Category = 2; // 打点类型 1 区域 ,2 园区
            return await this.plotitemService.ListPlotItemAsync(model);
        }

        /// <summary>
        /// 获取打点详情列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("camera/list")]
        [PermissionFilter("企业摄像头标记", 128)]
        public async Task<IPagedList<GISPlotItemDto>> ListCameraPlotItemAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            var plottype = GetPlotType();
            model.PlotType = plottype;
            model.Category = 2; // 打点类型 1 区域 ,2 园区
            return await this.plotitemService.ListPlotItemAsync(model);
        }

        /// <summary>
        /// 获取打点详情列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("publicfacility/list")]
        [PermissionFilter("企业设施标记", 128)]
        public async Task<IPagedList<GISPlotItemDto>> ListPublicPlotItemAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            var plottype = GetPlotType();
            model.PlotType = plottype;
            model.Category = 2; // 打点类型 1 区域 ,2 园区
            return await this.plotitemService.ListPlotItemAsync(model);
        }

        /// <summary>
        /// 获取打点详情列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("custom/list")]
        [PermissionFilter("企业自定义标记", 128)]
        public async Task<IPagedList<GISPlotItemDto>> ListCustomPlotItemAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            var plottype = GetPlotType();
            model.PlotType = plottype;
            model.Category = 2; // 打点类型 1 区域 ,2 园区
            return await this.plotitemService.ListPlotItemAsync(model);
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("farmland/add")]
        [PermissionFilter("企业地块标绘", 1)]
        public async Task<int> AddFarmlandAsync([FromBody] GISPlotItem model)
        {
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

            var plottype = GetPlotType();
            model.PlotType = plottype;
            var ret = await this.plotitemService.SavePlotItemAsync(model);

            return ret;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("farmland/edit")]
        [PermissionFilter("企业地块标绘", 4)]
        public async Task<int> UpdateFarmlandAsync([FromBody] GISPlotItem model)
        {
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

            var plottype = GetPlotType();
            model.PlotType = plottype;
            var ret = await this.plotitemService.SavePlotItemAsync(model);

            return ret;
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("camera/add")]
        [PermissionFilter("企业摄像头标记", 1)]
        public async Task<int> AddCameraAsync([FromBody] GISPlotItem model)
        {
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

            var plottype = GetPlotType();
            model.PlotType = plottype;
            var ret = await this.plotitemService.SavePlotItemAsync(model);

            return ret;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("camera/edit")]
        [PermissionFilter("企业摄像头标记", 4)]
        public async Task<int> UpdateCameraAsync([FromBody] GISPlotItem model)
        {
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

            var plottype = GetPlotType();
            model.PlotType = plottype;
            var ret = await this.plotitemService.SavePlotItemAsync(model);

            return ret;
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("publicfacility/add")]
        [PermissionFilter("企业设施标记", 1)]
        public async Task<int> AddPublicAsync([FromBody] GISPlotItem model)
        {
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

            var plottype = GetPlotType();
            model.PlotType = plottype;
            var ret = await this.plotitemService.SavePlotItemAsync(model);

            return ret;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("publicfacility/edit")]
        [PermissionFilter("企业设施标记", 4)]
        public async Task<int> UpdatePublicAsync([FromBody] GISPlotItem model)
        {
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

            var plottype = GetPlotType();
            model.PlotType = plottype;
            var ret = await this.plotitemService.SavePlotItemAsync(model);

            return ret;
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("custom/add")]
        [PermissionFilter("企业自定义标记", 1)]
        public async Task<int> AddCustomAsync([FromBody] GISPlotItem model)
        {
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

            var plottype = GetPlotType();
            model.PlotType = plottype;
            var ret = await this.plotitemService.SavePlotItemAsync(model);

            return ret;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("custom/edit")]
        [PermissionFilter("企业自定义标记", 4)]
        public async Task<int> UpdateCustomAsync([FromBody] GISPlotItem model)
        {
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

            var plottype = GetPlotType();
            model.PlotType = plottype;
            var ret = await this.plotitemService.SavePlotItemAsync(model);

            return ret;
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("farmland/detail")]
        [PermissionFilter("企业地块标绘", 8)]
        public async Task<GISPlotItemDto> GetFarmlandDetailtAsync([FromBody] DetailQueryModel model)
        {
            return await this.plotitemService.DetailPlotItemAsync(model.Id);
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("camera/detail")]
        [PermissionFilter("企业摄像头标记", 8)]
        public async Task<GISPlotItemDto> GetCameraDetailtAsync([FromBody] DetailQueryModel model)
        {
            return await this.plotitemService.DetailPlotItemAsync(model.Id);
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("publicfacility/detail")]
        [PermissionFilter("企业设施标记", 8)]
        public async Task<GISPlotItemDto> GetPublicDetailtAsync([FromBody] DetailQueryModel model)
        {
            return await this.plotitemService.DetailPlotItemAsync(model.Id);
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("custom/detail")]
        [PermissionFilter("企业自定义标记", 8)]
        public async Task<GISPlotItemDto> GetCustomDetailtAsync([FromBody] DetailQueryModel model)
        {
            return await this.plotitemService.DetailPlotItemAsync(model.Id);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("farmland/delete")]
        [PermissionFilter("企业地块标绘", 2)]
        public async Task<int> DeletetFarmlandAsync([FromBody] DetailQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return await this.plotitemService.DeletePlotItemAsync(model.Id, loginuser.UserId);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("camera/delete")]
        [PermissionFilter("企业摄像头标记", 2)]
        public async Task<int> DeletetCameraAsync([FromBody] DetailQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return await this.plotitemService.DeletePlotItemAsync(model.Id, loginuser.UserId);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("publicfacility/delete")]
        [PermissionFilter("企业设施标记", 2)]
        public async Task<int> DeletetPublicAsync([FromBody] DetailQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return await this.plotitemService.DeletePlotItemAsync(model.Id, loginuser.UserId);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("custom/delete")]
        [PermissionFilter("企业自定义标记", 2)]
        public async Task<int> DeletetCustomAsync([FromBody] DetailQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return await this.plotitemService.DeletePlotItemAsync(model.Id, loginuser.UserId);
        }

        /// <summary>
        /// 获取区域打点详情列表
        /// </summary>
        /// <param name="model"></param>
        /// 
        /// <returns></returns>
        [HttpPost("/api/gis/app/park/plotitem/area/list")]
        public async Task<IPagedList<GISPlotItemDto>> ListAreaPlotItemAppAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            model.PlotType = 3;
            model.Category = 2; // 打点类型 1 区域 ,2 园区
            model.ObjectId = loginuser.AreaId;
            return await this.plotitemService.ListPlotItemAsync(model);
        }

        private int GetPlotType()
        {
            var path = HttpContext.Request.Path.ToString();
            var plottype = 0; // 1摄像头，2传感器，3区域地图，4农户标记，5土地标记，6公共设施，7规划用地，8大棚, 9自定义
            var name = path.Split("/")[6];
            switch (name)
            {
                case "area":
                    plottype = 3;
                    break;
                case "household":
                    plottype = 4;
                    break;
                case "farmland":
                    plottype = 5;
                    break;
                case "greenhouse":
                    plottype = 8;
                    break;
                case "camera":
                    plottype = 1;
                    break;
                case "publicfacility":
                    plottype = 6;
                    break;
                case "custom":
                    plottype = 9;
                    break;
                case "planland":
                    plottype = 7;
                    break;
                default:
                    break;
            }
            return plottype;
        }
    }
}
