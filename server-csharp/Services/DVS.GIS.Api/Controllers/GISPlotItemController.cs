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
    /// 地理信息
    /// </summary>
    [Route("/api/gis/console/plotitem")]
    public class GISPlotItemController : DvsControllerBase
    {
        private readonly IMapper mapper;
        private readonly IGISPlotItemService plotitemService;

        /// <summary>
        /// 地理信息
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="plotitemService"></param>
        public GISPlotItemController(IMapper mapper, IGISPlotItemService plotitemService)
        {
            this.mapper = mapper;
            this.plotitemService = plotitemService;
        }

        /// <summary>
        /// 获取打点详情列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("area/list")]
        [PermissionFilter("区域地图标绘", 128)]
        public async Task<IPagedList<GISPlotItemDto>> ListAreaPlotItemAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            var plottype = GetPlotType();
            model.PlotType = plottype;
            model.Category = 1; // 打点类型 1 区域 ,2 园区
            return await this.plotitemService.ListPlotItemAsync(model);
        }

        /// <summary>
        /// 微信公众号获取区域打点详情列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("/api/gis/wechat/plotitem/area/list")]
        public async Task<IPagedList<GISPlotItemDto>> ListAreaPlotItemWeChatAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            model.PlotType = 3;
            model.Category = 1; // 打点类型 1 区域 ,2 园区
            return await this.plotitemService.ListPlotItemAsync(model);
        }

        /// <summary>
        /// 获取打点详情列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("household/list")]
        [PermissionFilter("农户标记", 128)]
        public async Task<IPagedList<GISPlotItemDto>> ListHouseholdPlotItemAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            var plottype = GetPlotType();
            model.PlotType = plottype;
            model.Category = 1; // 打点类型 1 区域 ,2 园区
            return await this.plotitemService.ListPlotItemAsync(model);
        }

        /// <summary>
        /// 微信公众号获取户码打点详情列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("/api/gis/wechat/plotitem/household/list")]
        public async Task<IPagedList<GISPlotItemDto>> ListHouseholdPlotItemWeChatAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            model.PlotType = 4;
            model.Category = 1; // 打点类型 1 区域 ,2 园区
            return await this.plotitemService.ListPlotItemAsync(model);
        }

        /// <summary>
        /// 获取打点详情列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("farmland/list")]
        [PermissionFilter("土地标绘", 128)]
        public async Task<IPagedList<GISPlotItemDto>> ListFarmlandPlotItemAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            var plottype = GetPlotType();
            model.PlotType = plottype;
            model.Category = 1; // 打点类型 1 区域 ,2 园区
            return await this.plotitemService.ListPlotItemAsync(model);
        }

        /// <summary>
        /// 获取打点详情列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("greenhouse/list")]
        [PermissionFilter("大棚标记", 128)]
        public async Task<IPagedList<GISPlotItemDto>> ListGreenhousePlotItemAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            var plottype = GetPlotType();
            model.PlotType = plottype;
            model.Category = 1; // 打点类型 1 区域 ,2 园区
            return await this.plotitemService.ListPlotItemAsync(model);
        }

        /// <summary>
        /// 获取打点详情列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("camera/list")]
        [PermissionFilter("摄像头标记", 128)]
        public async Task<IPagedList<GISPlotItemDto>> ListCameraPlotItemAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            var plottype = GetPlotType();
            model.PlotType = plottype;
            model.Category = 1; // 打点类型 1 区域 ,2 园区
            return await this.plotitemService.ListPlotItemAsync(model);
        }

        /// <summary>
        /// 获取打点详情列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("publicfacility/list")]
        [PermissionFilter("公共设施标记", 128)]
        public async Task<IPagedList<GISPlotItemDto>> ListPublicPlotItemAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            var plottype = GetPlotType();
            model.PlotType = plottype;
            model.Category = 1; // 打点类型 1 区域 ,2 园区
            return await this.plotitemService.ListPlotItemAsync(model);
        }

        /// <summary>
        /// 获取打点详情列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("custom/list")]
        [PermissionFilter("自定义标记", 128)]
        public async Task<IPagedList<GISPlotItemDto>> ListCustomPlotItemAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            var plottype = GetPlotType();
            model.PlotType = plottype;
            model.Category = 1; // 打点类型 1 区域 ,2 园区
            return await this.plotitemService.ListPlotItemAsync(model);
        }

        /// <summary>
        /// 获取打点详情列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("planland/list")]
        [PermissionFilter("规划用地标绘", 128)]
        public async Task<IPagedList<GISPlotItemDto>> ListPlanlandPlotItemAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            var plottype = GetPlotType();
            model.PlotType = plottype;
            model.Category = 1; // 打点类型 1 区域 ,2 园区
            return await this.plotitemService.ListPlotItemAsync(model);
        }

        /// <summary>
        /// 获取打点详情列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("/api/gis/app/plotitem/farmland/list")]
        [HttpPost("/api/gis/wechat/plotitem/farmland/list")]
        public async Task<IPagedList<GISPlotItemDto>> ListPlotItemAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            var plottype = GetPlotType();
            model.PlotType = plottype;
            model.Category = 1; // 打点类型 1 区域 ,2 园区
            return await this.plotitemService.ListPlotItemAsync(model);
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <returns></returns>
        [HttpPost("area/add")]
        [PermissionFilter("区域地图标绘", 1)]
        public async Task<int> AddAreaAsync([FromBody] GISPlotItem model)
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
        /// <returns></returns>
        [HttpPost("area/edit")]
        [PermissionFilter("区域地图标绘", 4)]
        public async Task<int> UpdateAreaAsync([FromBody] GISPlotItem model)
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
        [HttpPost("household/add")]
        [PermissionFilter("农户标记", 1)]
        public async Task<int> AddHouseholdAsync([FromBody] GISPlotItem model)
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
        [HttpPost("household/edit")]
        [PermissionFilter("农户标记", 4)]
        public async Task<int> UpdateHouseholdAsync([FromBody] GISPlotItem model)
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
        [HttpPost("farmland/add")]
        [PermissionFilter("土地标绘", 1)]
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
        [PermissionFilter("土地标绘", 4)]
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
        [HttpPost("greenhouse/add")]
        [PermissionFilter("大棚标记", 1)]
        public async Task<int> AddGreenhouseAsync([FromBody] GISPlotItem model)
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
        [HttpPost("greenhouse/edit")]
        [PermissionFilter("大棚标记", 4)]
        public async Task<int> UpdateGreenhouseAsync([FromBody] GISPlotItem model)
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
        [PermissionFilter("摄像头标记", 1)]
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
        [PermissionFilter("摄像头标记", 4)]
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
        [PermissionFilter("公共设施标记", 1)]
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
        [PermissionFilter("公共设施标记", 4)]
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
        [PermissionFilter("自定义标记", 1)]
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
        [PermissionFilter("自定义标记", 4)]
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
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("planland/add")]
        [PermissionFilter("规划用地标绘", 1)]
        public async Task<int> AddPlanAsync([FromBody] GISPlotItem model)
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
        [HttpPost("planland/edit")]
        [PermissionFilter("规划用地标绘", 4)]
        public async Task<int> UpdatePlanAsync([FromBody] GISPlotItem model)
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
        /// <returns></returns>
        [HttpPost("area/detail")]
        [PermissionFilter("区域地图标绘", 8)]
        public async Task<GISPlotItemDto> GetAreaDetailtAsync([FromBody] DetailQueryModel model)
        {
            return await this.plotitemService.DetailPlotItemAsync(model.Id);
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("household/detail")]
        [PermissionFilter("农户标记", 8)]
        public async Task<GISPlotItemDto> GetHouseDetailtAsync([FromBody] DetailQueryModel model)
        {
            return await this.plotitemService.DetailPlotItemAsync(model.Id);
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("farmland/detail")]
        [PermissionFilter("土地标绘", 8)]
        public async Task<GISPlotItemDto> GetFarmDetailtAsync([FromBody] DetailQueryModel model)
        {
            return await this.plotitemService.DetailPlotItemAsync(model.Id);
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("greenhouse/detail")]
        [PermissionFilter("大棚标记", 8)]
        public async Task<GISPlotItemDto> GetGreenhouseDetailtAsync([FromBody] DetailQueryModel model)
        {
            return await this.plotitemService.DetailPlotItemAsync(model.Id);
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("camera/detail")]
        [PermissionFilter("摄像头标记", 8)]
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
        [PermissionFilter("公共设施标记", 8)]
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
        [PermissionFilter("自定义标记", 8)]
        public async Task<GISPlotItemDto> GetCustomDetailtAsync([FromBody] DetailQueryModel model)
        {
            return await this.plotitemService.DetailPlotItemAsync(model.Id);
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("planland/detail")]
        [PermissionFilter("规划用地标绘", 8)]
        public async Task<GISPlotItemDto> GetPlanlandDetailtAsync([FromBody] DetailQueryModel model)
        {
            return await this.plotitemService.DetailPlotItemAsync(model.Id);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("area/delete")]
        [PermissionFilter("区域地图标绘", 2)]
        public async Task<int> DeletetAreaAsync([FromBody] DetailQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return await this.plotitemService.DeletePlotItemAsync(model.Id, loginuser.UserId);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("household/delete")]
        [PermissionFilter("农户标记", 2)]
        public async Task<int> DeletetHousePlanlandAsync([FromBody] DetailQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return await this.plotitemService.DeletePlotItemAsync(model.Id, loginuser.UserId);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("farmland/delete")]
        [PermissionFilter("土地标绘", 2)]
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
        [HttpPost("greenhouse/delete")]
        [PermissionFilter("大棚标记", 2)]
        public async Task<int> DeletetGreenhouseAsync([FromBody] DetailQueryModel model)
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
        [PermissionFilter("摄像头标记", 2)]
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
        [PermissionFilter("公共设施标记", 2)]
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
        [PermissionFilter("自定义标记", 2)]
        public async Task<int> DeletetCustomAsync([FromBody] DetailQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return await this.plotitemService.DeletePlotItemAsync(model.Id, loginuser.UserId);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("planland/delete")]
        [PermissionFilter("规划用地标绘", 8)]
        public async Task<int> DeletetPlanlandAsync([FromBody] DetailQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            return await this.plotitemService.DeletePlotItemAsync(model.Id, loginuser.UserId);
        }

        /// <summary>
        /// APP获取区域打点详情列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("/api/gis/app/plotitem/area/list")]
        public async Task<IPagedList<GISPlotItemDto>> ListAreaPlotItemAppAsync([FromBody] GISListQueryModel model)
        {
            LoginUser loginuser = this.LoginUser;
            model.UserId = loginuser.UserId;
            model.PlotType = 3;
            model.Category = 1; // 打点类型 1 区域 ,2 园区
            model.ObjectId = loginuser.AreaId;
            return await this.plotitemService.ListPlotItemAsync(model);
        }

        private int GetPlotType()
        {
            var path = HttpContext.Request.Path.ToString();
            var plottype = 0; // 1摄像头，2传感器，3区域地图，4农户标记，5土地标记，6公共设施，7规划用地，8大棚, 9自定义
            var name = path.Split("/")[5];
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
