using AutoMapper;
using Castle.Core.Logging;
using DVS.Application.Services.Common;
using DVS.Application.Services.Village;
using DVS.Common.Http;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.GIS;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.GIS.Query;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.GIS
{
    public class GISPlotItemSyncService : ServiceBase<GISPlotItem>, IGISPlotItemSyncService
    {
        private readonly IBasicAreaService basicAreaService;
        private readonly IHttpClientFactory factory;
        private readonly IConfiguration configuration;
        private readonly IGISCameraService cameraService;
        private readonly IGISCollectivePropertyService collectivePropertyService;
        private readonly IGISGreenHouseService greenHouseService;
        private readonly IGISCustomService customService;
        private readonly IGISFarmLandService farmLandService;
        private readonly IGISHouseHoldService houseHoldService;
        private readonly IPopulationService populationService;
        private readonly ILogger<GISPlotItemSyncService> Logger;
        private readonly IServiceBase<VillageHouseCodeMember> memberService;

        public GISPlotItemSyncService(DbContext dbContext, IMapper mapper, ILogger<GISPlotItemSyncService> logger, 
            IBasicAreaService basicAreaService, 
            IHttpClientFactory factory, 
            IConfiguration configuration, 
            IGISCameraService cameraService, 
            IGISCollectivePropertyService collectivePropertyService,
            IGISGreenHouseService greenHouseService, 
            IGISCustomService customService, 
            IGISFarmLandService farmLandService, 
            IGISHouseHoldService houseHoldService,
            IServiceBase<VillageHouseCodeMember> memberService,
            IPopulationService populationService)
            : base(dbContext, mapper)
        {
            this.basicAreaService = basicAreaService;
            this.factory = factory;
            this.configuration = configuration;
            this.cameraService = cameraService;
            this.collectivePropertyService = collectivePropertyService;
            this.greenHouseService = greenHouseService;
            this.customService = customService;
            this.farmLandService = farmLandService;
            this.houseHoldService = houseHoldService;
            this.populationService = populationService;
            this.Logger = logger;
            this.memberService = memberService;
        }

        public async Task<IPagedList<GISPlotItem>> ListPlotItemAsync(GISListQueryModel model)
        {
            Expression<Func<GISPlotItem, bool>> expression = a => a.IsSync == 0;

            if (model.PlotType > 0)
            {
                expression = expression.And(a => a.PlotType == model.PlotType);
            }

            var pageInfo = await this.GetQueryable().Where(expression).OrderBy(a => a.CreatedAt).ToPagedListAsync(model.Page, model.Limit);

            return pageInfo;
        }

        private async Task<int> UpdatePlotItemStatus(GISPlotItem model)
        {
            return await this.UpdateAsync(model);
        }

        private async Task<int> UpdatePopulationStatus(VillagePopulation model)
        {
            return await this.populationService.UpdateAsync(model);
        }

        private async Task<int> PostAsync(GISPlotItem model, string url, Object data)
        {
            string token = await this.GetTokenAsync();
            try
            {
                HttpResponseMessage response = await this.factory.PostAsync(url, data, (header) =>
                {
                    header.Add("Authorization", token);
                });
                var result = await response.Content.ReadAsStringAsync();
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var SyncId = jo.GetValue("data").ToString(); // 数据大屏返回的familyid
                    model.SyncId = SyncId;
                    model.IsSync = 1;
                    model.SyncRes = jo.GetValue("description").ToString();
                    model.SyncDate = DateTime.Now;
                    await UpdatePlotItemStatus(model);
                }
                else
                {
                    model.IsSync = 1;
                    model.SyncDate = DateTime.Now;
                    model.SyncRes = jo != null ? jo.GetValue("description").ToString() : "";
                    await UpdatePlotItemStatus(model);
                    this.Logger.LogInformation(model.SyncRes);
                    return -1;
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogInformation(ex.Message);
                return -1;
            }
            return 0;
        }

        private async Task<int> PostAsync(VillagePopulation model, string url, Object data)
        {
            string token = await this.GetTokenAsync();
            try
            {
                HttpResponseMessage response = await this.factory.PostAsync(url, data, (header) =>
                {
                    header.Add("Authorization", token);
                });
                var result = await response.Content.ReadAsStringAsync();
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var SyncId = jo.GetValue("data").ToString();
                    model.SyncId = SyncId;
                    model.IsSync = 1;
                    model.SyncRes = jo.GetValue("description").ToString();
                    model.SyncDate = DateTime.Now;
                    await UpdatePopulationStatus(model);
                }
                else
                {
                    model.IsSync = 1;
                    model.SyncDate = DateTime.Now;
                    model.SyncRes = jo != null ? jo.GetValue("description").ToString() : "";
                    await UpdatePopulationStatus(model);
                    this.Logger.LogInformation(model.SyncRes);
                    return -1;
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogInformation(ex.Message);
                return -1;
            }
            return 0;
        }

        private async Task<int> PutAsync(GISPlotItem model, string url, Object data)
        {
            string token = await this.GetTokenAsync();
            try
            {
                HttpResponseMessage response = await this.factory.PutAsync(url, data, (header) =>
                {
                    header.Add("Authorization", token);
                });
                var result = await response.Content.ReadAsStringAsync();
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    model.IsSync = 1;
                    model.SyncDate = DateTime.Now;
                    model.SyncRes = jo.GetValue("description").ToString();
                    await UpdatePlotItemStatus(model);
                }
                else
                {
                    model.IsSync = 1;
                    model.SyncDate = DateTime.Now;
                    model.SyncRes = jo != null ? jo.GetValue("description").ToString() : "";
                    await UpdatePlotItemStatus(model);
                    this.Logger.LogInformation(model.SyncRes);
                    return -1;
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogInformation(ex.Message);
                return -1;
            }
            return 0;
        }

        private async Task<int> DeleteAsync(GISPlotItem model, string url)
        {
            string token = await this.GetTokenAsync();
            try
            {
                HttpResponseMessage response = await this.factory.DeleteAsync(url, (header) =>
                {
                    header.Add("Authorization", token);
                });
                var result = await response.Content.ReadAsStringAsync();
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    model.IsSync = 1;
                    model.SyncDate = DateTime.Now;
                    model.SyncRes = jo.GetValue("description").ToString();
                    await UpdatePlotItemStatus(model);
                }
                else
                {
                    model.IsSync = 1;
                    model.SyncDate = DateTime.Now;
                    model.SyncRes = jo != null ? jo.GetValue("description").ToString() : "";
                    await UpdatePlotItemStatus(model);
                    this.Logger.LogInformation(model.SyncRes);
                    return -1;
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogInformation(ex.Message);
                return -1;
            }
            return 0;
        }

        public async Task<int> SyncCameraToDVM(GISPlotItem model)
        {
            var camera = await this.cameraService.GetAsync(a => a.Id == model.ObjectId);
            if (camera == null) {
                this.Logger.LogInformation("摄像头数据不存在");

                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                model.SyncRes = "摄像头数据不存在";
                return await UpdatePlotItemStatus(model);
            }

            var area = await this.basicAreaService.GetAsync(a => a.Id == camera.AreaId);
            if (area == null)
            {
                this.Logger.LogInformation("摄像头区域id数据不存在");
                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                model.SyncRes = "摄像头区域id数据不存在";
                return await UpdatePlotItemStatus(model);
            }

            Point point = GetPoint(model.Point);
            var data = new
            {
                streamAddress = camera.StreamUrl,
                url = camera.Url,
                organizationCode = area.AreaCode,
                address = camera.Address,
                name = camera.Name,
                intro = camera.Remark,
                sourceId = area.AreaCode + "-" + camera.Id,
                longitude = (point != null) ? point.longitude : 0,
                latitude = (point != null) ? point.latitude : 0,
                altitude = (point != null) ? point.altitude : 0,
            };
            var url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Camera";
            if (model.IsDeleted == 1 || camera.IsDeleted == 1)
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Camera/" + area.AreaCode + "-" + model.ObjectId;
                return await DeleteAsync(model, url);
            }
            else if (model.SyncId != "")
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Camera/" + area.AreaCode + "-" + model.ObjectId;
                return await PutAsync(model, url, data);
            }
            return await PostAsync(model, url, data);
        }

        private async Task<int> SyncCollectivePropertyToDVM(GISPlotItem model)
        {
            var data = await this.collectivePropertyService.DetailCollectivePropertyAsync(model.ObjectId);
            if (data == null)
            {
                this.Logger.LogInformation("公共设施数据不存在");

                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                model.SyncRes = "公共设施数据不存在";
                return await UpdatePlotItemStatus(model);
            }

            var area = await this.basicAreaService.GetAsync(a => a.Id == data.AreaId);
            if (area == null)
            {
                this.Logger.LogInformation("公共设施区域id数据不存在");

                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                model.SyncRes = "公共设施区域id数据不存在";
                return await UpdatePlotItemStatus(model);
            }

            Point point = GetPoint(model.Point);

            List<string> pictures = new List<string>();
            if (data.MediaFiles.Count > 0)
            {
                pictures.Add(data.MediaFiles[0].Url);
            }
            var content = new
            {
                type = data.TypeId,
                typeName = data.TypeName,
                organizationCode = area.AreaCode,
                address = data.Address,
                name = data.Name,
                intro = data.Remark,
                sourceId = area.AreaCode + "-" + data.Id,
                longitude = (point != null) ? point.longitude : 0,
                latitude = (point != null) ? point.latitude : 0,
                altitude = (point != null) ? point.altitude : 0,
                pictures,
            };

            var url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Infrastructure";
            if (model.IsDeleted == 1 || data.IsDeleted == 1)
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Infrastructure/" + area.AreaCode + "-" + model.ObjectId;
                return await DeleteAsync(model, url);
            }
            else if (model.SyncId != "")
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Infrastructure/" + area.AreaCode + "-" + model.ObjectId;
                return await PutAsync(model, url, content);
            }
            return await PostAsync(model, url, content);
        }

        private async Task<int> SyncGreenHouseToDVM(GISPlotItem model)
        {
            var data = await this.greenHouseService.DetailGreenHouseAsync(model.ObjectId);
            if (data == null)
            {
                this.Logger.LogInformation("大棚数据不存在");

                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                model.SyncRes = "大棚数据不存在";
                return await UpdatePlotItemStatus(model);
            }

            var area = await this.basicAreaService.GetAsync(a => a.Id == data.AreaId);
            if (area == null)
            {
                this.Logger.LogInformation("大棚区域id数据不存在");

                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                model.SyncRes = "大棚区域id数据不存在";
                return await UpdatePlotItemStatus(model);
            }

            Point point = GetPoint(model.Point);
            var content = new
            {
                organizationCode = area.AreaCode,
                address = data.Address,
                phone = data.Phone,
                name = data.Name,
                intro = data.Remark,
                sourceId = area.AreaCode + "-" + data.Id,
                area = data.Area,
                manager = data.Owner,
                crops = "",
                longitude = (point != null) ? point.longitude : 0,
                latitude = (point != null) ? point.latitude : 0,
                altitude = (point != null) ? point.altitude : 0,
            };

            var url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Greenhouse";
            if (model.IsDeleted == 1 || data.IsDeleted == 1)
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Greenhouse/" + area.AreaCode + "-" + model.ObjectId;
                return await DeleteAsync(model, url);
            }
            else if (model.SyncId != "")
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Greenhouse/" + area.AreaCode + "-" + model.ObjectId;
                return await PutAsync(model, url, content);
            }
            return await PostAsync(model, url, content);
        }

        private async Task<int> SyncCustomToDVM(GISPlotItem model)
        {
            var data = await this.customService.DetailCustomAsync(model.ObjectId);
            if (data == null)
            {
                this.Logger.LogInformation("自定义数据不存在");

                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                model.SyncRes = "自定义数据不存在";
                return await UpdatePlotItemStatus(model);
            }

            var area = await this.basicAreaService.GetAsync(a => a.Id == data.AreaId);
            if (area == null)
            {
                this.Logger.LogInformation("自定义区域id数据不存在");

                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                model.SyncRes = "自定义区域id数据不存在";
                return await UpdatePlotItemStatus(model);
            }

            Point point = GetPoint(model.Point);
            string icon = "", mediaurl = data.Url;
            if (data.IconFiles.Count > 0) {
                icon = data.IconFiles[0].Url;
            }
            if (data.MediaFiles.Count > 0) {
                mediaurl = data.MediaFiles[0].Url;
            }
            var content = new
            {
                organizationCode = area.AreaCode,
                name = data.Name,
                intro = data.Remark,
                sourceId = area.AreaCode + "-" + data.Id,
                longitude = (point != null) ? point.longitude : 0,
                latitude = (point != null) ? point.latitude : 0,
                altitude = (point != null) ? point.altitude : 0,
                type = data.MediaType - 1,
                url = mediaurl,
                icon,
                popupWidth = data.Width,
                popupHeight = data.Height,
            };

            var url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/CustomizeMarker";
            if (model.IsDeleted == 1 || data.IsDeleted == 1)
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/CustomizeMarker/" + area.AreaCode + "-" + model.ObjectId;
                return await DeleteAsync(model, url);
            }
            else if (model.SyncId != "")
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/CustomizeMarker/" + area.AreaCode + "-" + model.ObjectId;
                return await PutAsync(model, url, content);
            }
            return await PostAsync(model, url, content);
        }

        private async Task<int> SyncPlanlandToDVM(GISPlotItem model)
        {
            var data = await this.farmLandService.DetailFarmLandAsync(model.ObjectId);
            if (data == null)
            {
                this.Logger.LogInformation("规划用地数据不存在");

                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                model.SyncRes = "规划用地数据不存在";
                return await UpdatePlotItemStatus(model);
            }

            var area = await this.basicAreaService.GetAsync(a => a.Id == data.AreaId);
            if (area == null)
            {
                this.Logger.LogInformation("规划用地区域id数据不存在");

                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                model.SyncRes = "规划用地区域id数据不存在";
                return await UpdatePlotItemStatus(model);
            }

            List<Point> points = GetPoints(model.PointItems);
            var content = new
            {
                organizationCode = area.AreaCode,
                name = data.Name,
                address = data.Address,
                intro = data.Remark,
                sourceId = area.AreaCode + "-" + data.Id,
                fences = points,
                type = data.TypeId,
                typeName = data.TypeName,
            };

            var url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/LandPlanning";
            if (model.IsDeleted == 1)
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/LandPlanning/" + area.AreaCode + "-" + model.ObjectId;
                return await DeleteAsync(model, url);
            }
            else if (model.SyncId != "")
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/LandPlanning/" + area.AreaCode + "-" + model.ObjectId;
                return await PutAsync(model, url, content);
            }
            return await PostAsync(model, url, content);
        }

        private async Task<int> SyncFarmlandToDVM(GISPlotItem model)
        {
            var data = await this.farmLandService.DetailFarmLandAsync(model.ObjectId);
            if (data == null)
            {
                this.Logger.LogInformation("地块数据不存在");

                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                model.SyncRes = "地块数据不存在";
                return await UpdatePlotItemStatus(model);
            }

            var area = await this.basicAreaService.GetAsync(a => a.Id == data.AreaId);
            if (area == null)
            {
                this.Logger.LogInformation("地块数据区域id数据不存在");
                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                model.SyncRes = "地块数据区域id数据不存在";
                return await UpdatePlotItemStatus(model);
            }

            var plotitem = await this.GetAsync(a => a.PlotType == 4 && a.ObjectId == data.HouseholdId && a.IsDeleted == 0);
            List<Point> points = GetPoints(model.PointItems);
            var content = new
            {
                familyId = plotitem != null ? plotitem.SyncId : "0",
                area = data.Area,
                crops = "",
                matureDate = "",
                sourceId = area.AreaCode + "-" + data.Id,
                fences = points,
                output = 0,
            };

            var url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/ArableLand";
            if (model.IsDeleted == 1 || data.IsDeleted == 1)
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/ArableLand/" + area.AreaCode + "-" + model.ObjectId;
                return await DeleteAsync(model, url);
            }
            else if (model.SyncId != "")
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/ArableLand/" + area.AreaCode + "-" + model.ObjectId;
                return await PutAsync(model, url, content);
            }
            return await PostAsync(model, url, content);
        }

        private async Task<int> SyncHouseHoldToDVM(GISPlotItem model)
        {
            var data = await this.houseHoldService.DetailHouseholdCodeAsync(model.ObjectId);
            if (data == null)
            {
                this.Logger.LogInformation("户码数据不存在");

                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                model.SyncRes = "户码数据不存在";
                return await UpdatePlotItemStatus(model);
            }

            var area = await this.basicAreaService.GetAsync(a => a.Id == data.AreaId);
            if (area == null)
            {
                this.Logger.LogInformation("户码区域id数据不存在");

                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                model.SyncRes = "户码区域id数据不存在";
                return await UpdatePlotItemStatus(model);
            }

            Point point = GetPoint(model.Point);
            var content = new
            {
                organizationCode = area.AreaCode,
                name = data.RealName,
                mobile = data.Mobile,
                address = data.Address+data.HouseName+data.HouseNumber,
                houseName = data.HouseName,
                houseNumber = data.HouseNumber,
                sourceId = area.AreaCode + "-" + data.HouseholdId,
                gender = data.Sex - 1,
                idNumber = data.IdCard,
                imageUrls = data.ImageUrls,
                tags = string.Join(",",data.HouseholdTagNames.ToArray().Select(a=>a.Name)),
                longitude = (point != null) ? point.longitude : 0,
                latitude = (point != null) ? point.latitude : 0,
                altitude = (point != null) ? point.altitude : 0,
            };

            var url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Family";
            if (model.IsDeleted == 1 || data.IsDeleted == 1)
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Family/" + area.AreaCode + "-" + model.ObjectId;
                return await DeleteAsync(model, url);
            }
            else if (model.SyncId != "")
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Family/" + area.AreaCode + "-" + model.ObjectId;
                return await PutAsync(model, url, content);
            }
            var ret = await PostAsync(model, url, content);
            if (ret == 0) {
                // 户码打点数据同步后，将户码表的同步标志和同步返回id更新(familyId)
                _ = await this.houseHoldService.GetQueryable().Where(a => a.Id == model.ObjectId).UpdateFromQueryAsync(a => new VillageHouseholdCode()
                {
                    SyncId = model.SyncId,
                    IsSync = 1
                });
                _ = await SyncPersonToDVM(model);
            }
            return ret;
        }

        /// <summary>
        /// 户码对应的户籍人口同步到大屏
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<int> SyncPersonToDVM(GISPlotItem model)
        {
            var qeury = from m in this.memberService.GetQueryable()
                       join p in this.populationService.GetQueryable() on m.PopulationId equals p.Id
                       where m.HouseholdId == model.ObjectId && m.IsDeleted == 0
                       select p;
            // var datas = await this.populationService.GetListAsync(a => a.HouseholdId == model.ObjectId && a.IsDeleted == 0);
            var datas = await qeury.ToListAsync();
            if (datas == null)
            {
                this.Logger.LogInformation("户籍人口数据不存在");
                return -1;
            }

            var area = await this.basicAreaService.GetAsync(a => a.Id == datas[0].AreaId);
            if (area == null)
            {
                this.Logger.LogInformation("户籍人口区域id数据不存在");
                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                model.SyncRes = "户籍人口区域id数据不存在";
                return await UpdatePlotItemStatus(model);
            }

            foreach (var data in datas)
            {
                var population = await this.populationService.GetPopulationDetail(data.Id, "", 1, 1);
                if (population == null) continue;
                var content = new
                {
                    familyId = model.SyncId,
                    name = population.RealName,
                    mobile = population.Mobile,
                    birthday = population.Birthday,
                    relationship = population.Relationship,
                    education = population.Education,
                    gender = population.Sex - 1,
                    idNumber = population.IdCard,
                    sourceId = area.AreaCode + "-" + population.Id,
                    tags = string.Join(",", population.TagNames.ToArray().Select(a => a.Name)),
                    political = population.Political,
                    marital = population.Marital,
                    avatar = population.HeadImageUrl,
                };

                var url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Person";
                if (population.IsDeleted == 1)
                {
                    url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Person/" + area.AreaCode + "-" + population.Id;
                    await DeleteAsync(model, url);
                }
                else if (population.SyncId != "")
                {
                    url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Person/" + area.AreaCode + "-" + population.Id;
                    await PutAsync(model,url, content);
                }
                else
                {
                    await PostAsync(data, url, content);
                }
            }
            return 0;
        }

        private async Task<int> SyncAreaToDVM(GISPlotItem model)
        {
            var data = await this.basicAreaService.GetAsync(model.ObjectId);
            if (data == null)
            {
                this.Logger.LogInformation("区域数据不存在");

                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                model.SyncRes = "区域数据不存在";
                return await UpdatePlotItemStatus(model);
            }
            int level = data.Level - 1; // 1:省份 2:地市 3:区县 4:乡镇 5:社区/村 , 数据大屏 0:省份 1:地市 2:区县 3:乡镇 4:社区/村

            string parentcode = "";
            var parent = await this.basicAreaService.GetAsync(a => a.Id == data.Pid); // 查找上级区域
            if (parent != null)
            {
                parentcode = parent.AreaCode.ToString();
                this.Logger.LogInformation("上级区域代码: "+ parentcode);
            }
            if (string.IsNullOrWhiteSpace(parentcode))
            {
                try
                {
                    switch (data.Level)
                    {
                        case 1:
                            parentcode = "0";
                            break;
                        case 2:
                            parentcode = data.AreaCode.ToString().Substring(0, 2);
                            break;
                        case 3:
                            parentcode = data.AreaCode.ToString().Substring(0, 4);
                            break;
                        case 4:
                            parentcode = data.AreaCode.ToString().Substring(0, 6);
                            break;
                        case 5:
                            parentcode = data.AreaCode.ToString().Substring(0, 9);
                            break;
                        default:
                            parentcode = data.AreaCode.ToString();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.LogInformation(ex.Message);
                    parentcode = data.AreaCode.ToString();
                }
            }
            List<Point> points = GetPoints(model.PointItems);
            Point point = GetPoint(model.Point);
            var content = new
            {
                code = data.AreaCode,
                parentCode = int.Parse(parentcode),
                name = data.Name,
                level,
                address = "",
                sourceId = data.AreaCode + "-" + data.Id,
                longitude = (point != null) ? point.longitude : 0,
                latitude = (point != null) ? point.latitude : 0,
                altitude = (point != null) ? point.altitude : 0,
                fences = points
            };

            var url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Organization";
            if (model.IsDeleted == 1 || data.IsDeleted == 1)
            {
                //url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "​/dvm/Organization/" + data.AreaCode;
                //return await DeleteAsync(model, url);
                // 区域删除不同步
                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                model.SyncRes = "";
                await UpdatePlotItemStatus(model);
            }
            else if (model.SyncId != "")
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Organization/" + data.AreaCode;
                return await PutAsync(model, url, content);
            }
            return await PostAsync(model, url, content);
        }

        public async Task<int> SyncToDVM(GISPlotItem model)
        {
            /// 分类 1摄像头，2传感器，3区域地图，4农户标记，5土地标记，6公共设施，7规划用地，8大棚, 9自定义
            switch (model.PlotType) {

                case 1:
                    _ = await SyncCameraToDVM(model);
                    break;
                case 2:
                    break;
                case 3:
                    _ = await SyncAreaToDVM(model);
                    break;
                case 4:
                    _ = await SyncHouseHoldToDVM(model);
                    break;
                case 5:
                    _ = await SyncFarmlandToDVM(model);
                    break;
                case 6:
                    _ = await SyncCollectivePropertyToDVM(model);
                    break;
                case 7:
                    _ = await SyncPlanlandToDVM(model);
                    break;
                case 8:
                    _ = await SyncGreenHouseToDVM(model);
                    break;
                case 9:
                    _ = await SyncCustomToDVM(model);
                    break;
                default:
                    break;

            }
            return 0;
        }

        private async Task<string> GetTokenAsync()
        {
            var url = "https://api.sea.utuapp.cn/connect/token";
            if (configuration.GetValue<string>("BIGDATA:TOKEN_URL") != null)
            {
                url = configuration.GetValue<string>("BIGDATA:TOKEN_URL").ToString();
            }
            string clientId = configuration.GetValue<string>("BIGDATA:CLIENT_ID").ToString();
            string secret = configuration.GetValue<string>("BIGDATA:CLIENT_SECRET").ToString();

            FormUrlEncodedContent fromContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string,string>("client_id", clientId),
                new KeyValuePair<string,string>("client_secret", secret),
                new KeyValuePair<string,string>("grant_type", "client_credentials")
            });
            fromContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            HttpClient client = factory.CreateClient();
            HttpResponseMessage message = await client.PostAsync(url, fromContent);

            client.Dispose();

            var result = await message.Content.ReadAsStringAsync();
            if (message.StatusCode != System.Net.HttpStatusCode.OK) {
                this.Logger.LogInformation(result);
                return "";
            }
            JObject jo = (JObject)JsonConvert.DeserializeObject(result);
            var token = jo.GetValue("token_type").ToString() + " " + jo.GetValue("access_token").ToString();
            return token;
        }

        private Point GetPoint(string pointitem)
        {
            Point point = null;
            try
            {
                point = JsonConvert.DeserializeObject<Point>(pointitem);
            }
            catch (Exception ex)
            {
                this.Logger.LogInformation(ex.Message);
            }
            return point;
        }

        private List<Point> GetPoints(string pointitems)
        {
            List<Point> points = null;
            try
            {
                points = JsonConvert.DeserializeObject<List<Point>>(pointitems);
            }
            catch (Exception ex)
            {
                this.Logger.LogInformation(ex.Message);
            }
            return points;
        }
    }  
}
