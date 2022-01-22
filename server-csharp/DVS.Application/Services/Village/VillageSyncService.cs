using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Application.Services.GIS;
using DVS.Common.Http;
using DVS.Common.ModelDtos;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Core.Domains.GIS;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.Village.Query;
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

namespace DVS.Application.Services.Village
{
    public class VillageSyncService : ServiceBase<VillageHouseholdCode>, IVillageSyncService
    {
        private readonly IBasicAreaService basicAreaService;
        private readonly IHttpClientFactory factory;
        private readonly IConfiguration configuration;
        private readonly IPopulationService populationService;
        private readonly IGISPlotItemService gisPlotItemService;
        private readonly IHouseholdCodeService householdservice;
        private readonly IWorkService workService;
        private readonly IIncomeService incomeService;
        private readonly IEpidemicService epidemicService;
        private readonly ILogger<VillageSyncService> Logger;
        private readonly IServiceBase<VillageHouseCodeMember> memberService;

        public VillageSyncService(DbContext dbContext, IMapper mapper, ILogger<VillageSyncService> logger, IBasicAreaService basicAreaService, IHttpClientFactory factory, IConfiguration configuration, IPopulationService populationService, IGISPlotItemService gisPlotItemService,IHouseholdCodeService householdservice, IEpidemicService epidemicService, IIncomeService incomeService, IWorkService workService, IServiceBase<VillageHouseCodeMember> memberService)
            : base(dbContext, mapper)
        {
            this.basicAreaService = basicAreaService;
            this.factory = factory;
            this.configuration = configuration;
            this.populationService = populationService;
            this.gisPlotItemService = gisPlotItemService;
            this.householdservice = householdservice;
            this.workService = workService;
            this.incomeService = incomeService;
            this.epidemicService = epidemicService;
            this.Logger = logger;
            this.memberService = memberService;
        }

        /// <summary>
        /// 查询未同步户码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IPagedList<VillageHouseholdCode>> ListHouseHoldAsync(PostBody model)
        {
            Expression<Func<VillageHouseholdCode, bool>> expression = a => a.IsSync == 0;

            var pageInfo = await this.GetQueryable().Where(expression).OrderBy(a => a.CreatedAt).ToPagedListAsync(model.Page, model.Limit);

            return pageInfo;
        }

        /// <summary>
        /// 查询未同步户籍
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IPagedList<VillagePopulation>> ListPopulationAsync(PostBody model)
        {
            Expression<Func<VillagePopulation, bool>> expression = a => a.IsSync == 0;

            var pageInfo = await this.populationService.GetQueryable().Where(expression).OrderBy(a => a.CreatedAt).ToPagedListAsync(model.Page, model.Limit);

            return pageInfo;
        }

        private async Task<int> UpdatePopulationStatus(VillagePopulation model)
        {
            return await this.populationService.GetQueryable().Where(a => a.Id == model.Id)
                    .UpdateFromQueryAsync(a => new VillagePopulation()
                    {
                        IsSync = 1,
                        SyncDate = DateTime.Now
                    });
        }

        private async Task<int> UpdateEpidemicStatus(VillageEpidemic model)
        {
            return await this.epidemicService.GetQueryable().Where(a => a.Id == model.Id)
                    .UpdateFromQueryAsync(a => new VillageEpidemic()
                    {
                        IsSync = 1,
                        SyncDate = DateTime.Now,
                        SyncRes = model.SyncRes
                    });
        }

        private async Task<int> UpdateIncomeStatus(VillageIncome model)
        {
            return await this.incomeService.GetQueryable().Where(a => a.Id == model.Id)
                    .UpdateFromQueryAsync(a => new VillageIncome()
                    {
                        IsSync = 1,
                        SyncDate = DateTime.Now,
                        SyncRes = model.SyncRes
                    });
        }

        private async Task<int> UpdateWorkStatus(VillageWork model)
        {
            return await this.workService.GetQueryable().Where(a => a.Id == model.Id)
                    .UpdateFromQueryAsync(a => new VillageWork()
                    {
                        IsSync = 1,
                        SyncDate = DateTime.Now,
                        SyncRes = model.SyncRes
                    });
        }

        private async Task<int> UpdateHouseHouldStatus(VillageHouseholdCode model)
        {
            return await this.householdservice.GetQueryable().Where(a => a.Id == model.Id)
                    .UpdateFromQueryAsync(a => new VillageHouseholdCode()
                    {
                        IsSync = 1,
                        SyncDate = DateTime.Now,
                        SyncRes = model.SyncRes
                    });
        }

        private async Task<int> UpdatePlotItemStatus(GISPlotItem model)
        {
            return await this.gisPlotItemService.GetQueryable().Where(a => a.Id == model.Id)
                    .UpdateFromQueryAsync(a => new GISPlotItem()
                    {
                        IsSync = 1,
                        SyncDate = DateTime.Now,
                        SyncId = model.SyncId,
                    });
        }
        
        private async Task<ResultModel<VillageHouseholdCode>> PostAsync(string url, Object data)
        {
            string token = await this.GetTokenAsync();
            ResultModel<VillageHouseholdCode> ret = new ResultModel<VillageHouseholdCode>
            {
                Code = 0
            };
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
                    ret.Message = jo.GetValue("data").ToString();
                    return ret;
                }
                else
                {
                    ret.Code = -1;
                    ret.Message = jo != null ? jo.GetValue("description").ToString() : "";
                    this.Logger.LogInformation(jo != null ? jo.GetValue("description").ToString() : "");
                    return ret;
                }
            }
            catch (Exception ex)
            {
                ret.Code = -1;
                ret.Message = ex.Message;
                this.Logger.LogInformation(ex.Message);
                return ret;
            }
        }

        private async Task<ResultModel<VillageHouseholdCode>> PutAsync(string url, Object data)
        {
            string token = await this.GetTokenAsync();
            ResultModel<VillageHouseholdCode> ret = new ResultModel<VillageHouseholdCode>
            {
                Code = 0
            };
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
                    ret.Message = jo.GetValue("data").ToString();
                    return ret;
                }
                else
                {
                    ret.Code = -1;
                    ret.Message = jo != null ? jo.GetValue("description").ToString() : "";
                    this.Logger.LogInformation(jo != null ? jo.GetValue("description").ToString() : "");
                    return ret;
                }
            }
            catch (Exception ex)
            {
                ret.Code = -1;
                ret.Message = ex.Message;
                this.Logger.LogInformation(ex.Message);
                return ret;
            }
        }

        private async Task<ResultModel<VillageHouseholdCode>> DeleteAsync(string url)
        {
            string token = await this.GetTokenAsync();
            ResultModel<VillageHouseholdCode> ret = new ResultModel<VillageHouseholdCode>
            {
                Code = 0
            };
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
                    ret.Message = jo.GetValue("data").ToString();
                    return ret;
                }
                else
                {
                    ret.Code = -1;
                    ret.Message = jo != null ? jo.GetValue("description").ToString() : "";
                    this.Logger.LogInformation(jo != null ? jo.GetValue("description").ToString() : "");
                    return ret;
                }
            }
            catch (Exception ex)
            {
                ret.Code = -1;
                ret.Message = ex.Message;
                this.Logger.LogInformation(ex.Message);
                return ret;
            }
        }

        public async Task<int> SyncHouseHoldToDVM(VillageHouseholdCode model)
        {
            var result = new ResultModel<VillageHouseholdCode>();
            var area = await this.basicAreaService.GetAsync(a => a.Id == model.AreaId);
            if (area == null)
            {
                this.Logger.LogInformation("户码区域id数据不存在");
                model.IsSync = 1;
                model.SyncRes = "户码区域id数据不存在";
                model.SyncDate = DateTime.Now;
                await UpdateHouseHouldStatus(model);
                return -1;
            }

            var url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Family";
            if (model.IsDeleted == 1 || model.Status == 0)
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Family/" + area.AreaCode + "-" + model.Id;
                result = await DeleteAsync(url);
                if (result.Code == 0)
                {
                    model.IsSync = 1;
                    model.SyncDate = DateTime.Now;
                    await UpdateHouseHouldStatus(model);
                }
                return 0;
            }

            var data = await this.householdservice.GethouseholderAndHouseNumber(model.Id);
            if (data == null)
            {
                this.Logger.LogInformation("户码数据不存在");
                model.IsSync = 1;
                model.SyncRes = "户码数据不存在";
                model.SyncDate = DateTime.Now;
                await UpdateHouseHouldStatus(model);
                return -1;
            }

            bool IsSync = true;
            Expression<Func<GISPlotItem, bool>> expression = a => a.PlotType == 4 && a.ObjectId == model.Id;
            var pageInfo = await this.gisPlotItemService.GetQueryable().Where(expression).OrderBy(a => a.CreatedAt).ToListAsync();
            foreach (var item in pageInfo)
            {

                Point point = GetPoint(item.Point);
                var content = new
                {
                    organizationCode = area.AreaCode,
                    name = data.RealName,
                    mobile = data.Mobile,
                    address = data.Address + data.HouseName + data.HouseNumber,
                    houseName = data.HouseName,
                    houseNumber = data.HouseNumber,
                    sourceId = area.AreaCode + "-" + model.Id,
                    gender = data.Sex - 1,
                    idNumber = data.IdCard,
                    imageUrls = data.ImageUrls,
                    tags = string.Join(",", data.HouseholdTagNames.ToArray().Select(a => a.Name)),
                    longitude = (point != null) ? point.longitude : 0,
                    latitude = (point != null) ? point.latitude : 0,
                    altitude = (point != null) ? point.altitude : 0,
                };

                if (model.IsDeleted == 1)
                {
                    url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Family/" + area.AreaCode + "-" + model.Id;
                    result = await DeleteAsync(url);
                    if (result.Code != 0)
                    {
                        IsSync = false;
                    }
                }
                else if (model.SyncId != "")
                {
                    url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Family/" + area.AreaCode + "-" + model.Id;
                    result = await PutAsync(url, content);
                    if (result.Code != 0)
                    {
                        IsSync = false;
                    }
                }
                result = await PostAsync(url, content);
                if (result.Code == 0)
                {
                    item.SyncId = result.Message;
                    item.IsSync = 1;
                    item.SyncDate = DateTime.Now;
                    await UpdatePlotItemStatus(item);
                }
                else
                {
                    IsSync = false;
                }
            }
            //if (IsSync)
            //{
                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                await UpdateHouseHouldStatus(model);
            //}
            return 0;
        }

        /// <summary>
        /// 户码对应的户籍人口同步到大屏
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> SyncPersonToDVM(VillagePopulation model)
        {
            var result = new ResultModel<VillageHouseholdCode>();
            var population = await this.populationService.GetPopulationDetail(model.Id, "", 1, 0);
            var households = await this.memberService.GetListAsync(a => a.PopulationId == model.Id && a.IsDeleted == 0);
            var ids = households.Select(a => a.HouseholdId).Distinct().ToList();
            var plotitem = await this.gisPlotItemService.GetAsync(a => a.PlotType == 4 && ids.Contains(a.ObjectId) && a.IsDeleted == 0 && a.SyncId != "");

            var area = await this.basicAreaService.GetAsync(a => a.Id == model.AreaId);
            if (area == null)
            {
                this.Logger.LogInformation("户码区域id数据不存在");
                model.IsSync = 1;
                model.SyncRes = "户码区域id数据不存在";
                model.SyncDate = DateTime.Now;
                await UpdatePopulationStatus(model);
                return -1;
            }
            var content = new
            {
                familyId = plotitem != null ? plotitem.SyncId : "0",
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
            if (population.IsDeleted == 1 || population.SyncId == "")
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Person/" + area.AreaCode + "-" + population.Id;
                result = await DeleteAsync(url);
                //if (result.Code == 0)
                //{
                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                await UpdatePopulationStatus(model);
                //}
                return 0;
            }
            else if (population.SyncId != "")
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Person/" + area.AreaCode + "-" + population.Id;
                result = await PutAsync(url, content);
                //if (result.Code == 0)
                //{
                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                await UpdatePopulationStatus(model);
                //}
                return 0;
            }
            result = await PostAsync(url, content);
            if (result.Code == 0)
            {
                model.SyncId = result.Message;
            }
            else
            {
                model.SyncRes = result.Message;
            }
            model.IsSync = 1;
            model.SyncDate = DateTime.Now;
            await UpdatePopulationStatus(model);
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

        public async Task<IPagedList<VillageEpidemic>> ListEpidemicAsync(PostBody model)
        {
            Expression<Func<VillageEpidemic, bool>> expression = a => a.IsSync == 0;

            var pageInfo = await this.epidemicService.GetQueryable().Where(expression).OrderBy(a => a.CreatedAt).ToPagedListAsync(model.Page, model.Limit);

            return pageInfo;
        }

        public async Task<IPagedList<VillageIncome>> ListIncomeAsync(PostBody model)
        {
            Expression<Func<VillageIncome, bool>> expression = a => a.IsSync == 0;

            var pageInfo = await this.incomeService.GetQueryable().Where(expression).OrderBy(a => a.CreatedAt).ToPagedListAsync(model.Page, model.Limit);

            return pageInfo;
        }

        public async Task<IPagedList<VillageWork>> ListWorkAsync(PostBody model)
        {
            Expression<Func<VillageWork, bool>> expression = a => a.IsSync == 0;

            var pageInfo = await this.workService.GetQueryable().Where(expression).OrderBy(a => a.CreatedAt).ToPagedListAsync(model.Page, model.Limit);

            return pageInfo;
        }

        public async Task<int> SyncEpidemicToDVM(VillageEpidemic model)
        {
            var result = new ResultModel<VillageHouseholdCode>();
            var pop = await this.populationService.GetAsync(a => a.Id == model.PopulationId);
            if (pop == null)
            {
                this.Logger.LogInformation("返乡登记人员数据不存在");
                model.IsSync = 1;
                model.SyncRes = "返乡登记人员数据不存在";
                model.SyncDate = DateTime.Now;
                await UpdateEpidemicStatus(model);
                return -1;
            }
            var area = await this.basicAreaService.GetAsync(a => a.Id == pop.AreaId);
            if (area == null)
            {
                this.Logger.LogInformation("防疫情况区域id数据不存在");
                model.IsSync = 1;
                model.SyncRes = "防疫情况区域id数据不存在";
                model.SyncDate = DateTime.Now;
                await UpdateEpidemicStatus(model);
                return -1;
            }

            var url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Family/SaveFamilyEpidemic";
            if (model.IsDeleted == 1)
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "​/dvm/Family/DeleteFamilyEpidemic/" + area.AreaCode + "-" + model.Id;
                result = await DeleteAsync(url);
                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                if (result.Code != 0)
                {
                    model.SyncRes = result.Message;
                    this.Logger.LogInformation(result.Message);
                }
                await UpdateEpidemicStatus(model);
                return 0;
            }

            var population = await this.populationService.GetPopulationDetail(model.PopulationId, "", 1, 1);
            var plotitem = await this.gisPlotItemService.GetAsync(a => a.PlotType == 4 && a.ObjectId == model.HouseholdId && a.IsDeleted == 0);
            var content = new
            {
                organizationCode = area.AreaCode,
                familyId = plotitem != null ? plotitem.SyncId : "0",
                name = plotitem != null ? population.RealName : "",
                recordDate = model.CreatedAt.ToString(),
                temperature = model.Temperature,
                health = model.Health,
                isFever = model.IsFever,
                isInAreas = model.IsInAreas,
                year = model.Year,
                sourceId = area.AreaCode + "-" + model.Id,
            };
            if (model.SyncId != "")
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Family/EditFamilyEpidemic/" + area.AreaCode + "-" + model.Id;
                result = await PutAsync(url, content);
                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                if (result.Code != 0)
                {
                    model.SyncRes = result.Message;
                    this.Logger.LogInformation(result.Message);
                }
                await UpdateEpidemicStatus(model);
                return 0;
            }
            result = await PostAsync(url, content);
            if (result.Code == 0)
            {
                model.SyncId = result.Message;
            }
            else {
                model.SyncRes = result.Message;
            }
            model.IsSync = 1;
            model.SyncDate = DateTime.Now;
            await UpdateEpidemicStatus(model);
            return 0;
        }

        public async Task<int> SyncIncomeToDVM(VillageIncome model)
        {
            var result = new ResultModel<VillageHouseholdCode>();
            var area = await this.basicAreaService.GetAsync(a => a.Id == model.AreaId);
            if (area == null)
            {
                this.Logger.LogInformation("户码收入区域id数据不存在");
                model.IsSync = 1;
                model.SyncRes = "户码收入区域id数据不存在";
                model.SyncDate = DateTime.Now;
                await UpdateIncomeStatus(model);
                return -1;
            }

            var url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Family/SaveFamilyIncome";
            if (model.IsDeleted == 1)
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "​/dvm/Family/DeleteFamilyIncome/" + area.AreaCode + "-" + model.Id;
                result = await DeleteAsync(url);
                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                if (result.Code != 0)
                {
                    model.SyncRes = result.Message;
                    this.Logger.LogInformation(result.Message);
                }
                await UpdateIncomeStatus(model);
                return 0;
            }

            var plotitem = await this.gisPlotItemService.GetAsync(a => a.PlotType == 4 && a.ObjectId == model.HouseholdId && a.IsDeleted == 0);
            var content = new
            {
                organizationCode = area.AreaCode,
                familyId = plotitem != null ? plotitem.SyncId : "0",
                year = model.Year,
                product = model.Product,
                houseRental = model.HouseRental,
                collectiveDividend = model.CollectiveDividend,
                distribution = model.Distribution,
                landCirculation = model.LandCirculation,
                workIncome = model.WorkIncome,
                govSubsidy = model.GovSubsidy,
                other = model.Other,
                totalIncome = model.TotalIncome,
                sourceId = area.AreaCode + "-" + model.Id,
            };

            if (model.SyncId != "")
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Family/EditFamilyIncome/" + area.AreaCode + "-" + model.Id;
                result = await PutAsync(url, content);
                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                if (result.Code != 0)
                {
                    model.SyncRes = result.Message;
                    this.Logger.LogInformation(result.Message);
                }
                await UpdateIncomeStatus(model);
                return 0;
            }
            result = await PostAsync(url, content);
            if (result.Code == 0)
            {
                model.SyncId = result.Message;
            }
            else
            {
                model.SyncRes = result.Message;
            }
            model.IsSync = 1;
            model.SyncDate = DateTime.Now;
            await UpdateIncomeStatus(model);
            return 0;
        }

        public async Task<int> SyncWorkToDVM(VillageWork model)
        {
            var result = new ResultModel<VillageHouseholdCode>();
            var area = await this.basicAreaService.GetAsync(a => a.Id == model.AreaId);
            if (area == null)
            {
                this.Logger.LogInformation("外出务工区域id数据不存在");
                model.IsSync = 1;
                model.SyncRes = "外出务工区域id数据不存在";
                model.SyncDate = DateTime.Now;
                await UpdateWorkStatus(model);
                return -1;
            }
            var url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Family/SaveFamilyWork";
            if (model.IsDeleted == 1)
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Family/DeleteFamilyWork/"+ area.AreaCode + "-" + model.Id;
                result = await DeleteAsync(url);
                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                if (result.Code != 0)
                {
                    model.SyncRes = result.Message;
                    this.Logger.LogInformation(result.Message);
                }
                await UpdateWorkStatus(model);
                return 0;
            }

            var population = await this.populationService.GetPopulationDetail(model.PopulationId, "", 1, 1);
            var plotitem = await this.gisPlotItemService.GetAsync(a => a.PlotType == 4 && a.ObjectId == model.HouseholdId && a.IsDeleted == 0);
            var sex = population.Sex.ToString();
            var content = new
            {
                organizationCode = area.AreaCode,
                familyId = plotitem != null ? plotitem.SyncId : "0",
                year = model.Year,
                name = population != null ? population.RealName : "",
                birthday = population != null ? population.Birthday.ToString() : "",
                gender = population != null ? (population.Sex.ToString() == "男" ? 0 : 1) : 0,
                occupation = model.Occupation,
                salary = model.Salary,
                province = model.WorkOrgCodes == "" ? "" : model.WorkOrgCodes.Split(",")[0],
                sourceId = area.AreaCode + "-" + model.Id,
            };
            if (model.SyncId != "")
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Family/EditFamilyWork/" + area.AreaCode + "-" + model.Id;
                result = await PutAsync(url, content);
                model.IsSync = 1;
                model.SyncDate = DateTime.Now;
                if (result.Code != 0)
                {
                    model.SyncRes = result.Message;
                    this.Logger.LogInformation(result.Message);
                }
                await UpdateWorkStatus(model);
                return 0;
            }
            result = await PostAsync(url, content);
            if (result.Code == 0)
            {
                model.SyncId = result.Message;
            }
            else
            {
                model.SyncRes = result.Message;
            }
            model.IsSync = 1;
            model.SyncDate = DateTime.Now;
            await UpdateWorkStatus(model);
            return 0;
        }        
    }
}
