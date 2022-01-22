using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Common.Services;
using DVS.Core.Domains.Cons;
using DVS.Models.Dtos.Cons;
using DVS.Models.Dtos.Cons.Query;
using DVS.Models.Dtos.Common;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;
using DVS.Application.Services.Village;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using DVS.Common.Http;
using DVS.Common.Models;
using DVS.Application.Services.GIS;
using DVS.Models.Dtos.GIS.Query;
using Microsoft.Extensions.Logging;
using DVS.Core.Domains.Village;
using DVS.Common;
using DVS.Common.RPC;
using DVS.Models.Dtos.RPC;

namespace DVS.Application.Services.Cons
{
    public class PhotoAnywhereService : ServiceBase<ConsPhotoAnywhere>, IPhotoAnywhereService
    {
        readonly IBasicUserService basicUserService;
        readonly ISunFileInfoService fileService;
        readonly IBasicDictionaryService dictionaryService;
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory factory;
        private readonly IBasicAreaService basicAreaService;
        private readonly IGISPlotItemService gisPlotItemService;
        private readonly ILogger<PhotoAnywhereService> logger;
        private readonly IServiceBase<VillagePopulation> populationService;
        private readonly IServiceBase<VillageHouseCodeMember> memberService;

        public PhotoAnywhereService(DbContext dbContext, IConfiguration configuration, IHttpClientFactory factory, IMapper mapper, ISunFileInfoService sunfileservice, IBasicUserService basicUserService, IBasicDictionaryService dictionaryService, IServiceBase<VillageHouseCodeMember> memberService, IBasicAreaService basicAreaService, IGISPlotItemService gisPlotItemService, IServiceBase<VillagePopulation> populationService, ILogger<PhotoAnywhereService> logger)
            : base(dbContext, mapper)
        {
            this.basicUserService = basicUserService;
            this.fileService = sunfileservice;
            this.dictionaryService = dictionaryService;
            this.configuration = configuration;
            this.basicAreaService = basicAreaService;
            this.factory = factory;
            this.gisPlotItemService = gisPlotItemService;
            this.logger = logger;
            this.populationService = populationService;
            this.memberService = memberService;
        }

        public async Task<IPagedList<ListPhotoAnywhereModel>> GetDataList(PhotoAnywhereListQueryModel model)
        {
            Expression<Func<ConsPhotoAnywhere, bool>> filter = a => a.IsDeleted == 0;

            Expression<Func<ConsPhotoAnywhere, object>> orderby = a => a.CreatedAt;

            if (!model.Keyword.IsNullOrEmpty())
            {
                filter = filter.And(a => a.Descript.ToLower().Contains(model.Keyword));
            }

            if (model.IsReply == 1 || model.IsReply == 2)
            {
                filter = filter.And(a => a.IsReply == model.IsReply);
            }

            if (model.UserId != 0)
            {
                filter = filter.And(a => a.UserId == model.UserId);
            }

            if (model.PopulationId != 0)
            {
                // 查询村民发的随手拍
                var basicuser = await this.basicUserService.GetAsync(a => a.PopulationId == model.PopulationId && a.IsDeleted == 0);
                if (basicuser != null)
                {
                    filter = filter.And(a => a.UserId == basicuser.Id);
                }
                else
                {
                    filter = filter.And(a => a.UserId == -1);
                }
            }

            if (model.HouseHoldId != 0)
            {
                //filter = filter.And(a => a.HouseholdId == model.HouseHoldId); // 根据户码id来查

                // 根据populationid查，从户码中移除后，发布的随手拍不再在原来的户码中展示 2.4再改
                var popidlist = await this.memberService.GetListAsync(a => a.HouseholdId == model.HouseHoldId && a.IsDeleted == 0);
                var ids = popidlist.Select(a => a.PopulationId).Distinct().ToList();
                if (ids.Count() > 0)
                {
                    var users = await this.basicUserService.GetListAsync(a => ids.Contains(a.PopulationId) && a.IsDeleted == 0);
                    if (users.Count() > 0)
                    {
                        var userids = users.Select(a => a.Id).Distinct().ToList();
                        filter = filter.And(a => userids.Contains(a.UserId));
                    }
                    else
                    {
                        filter = filter.And(a => a.HouseholdId == -1);
                    }
                }
                else
                {
                    filter = filter.And(a => a.HouseholdId == -1);
                }
            }

            if (model.AreaId != 0 && model.PopulationId == 0 && model.HouseHoldId == 0)
            {
                //var areaIds = await this.basicAreaService.FindChildrenAreaIds(model.AreaId, true, false); // 本级及下级区域
                //filter = filter.And(a => areaIds.Contains(a.AreaId));
                filter = filter.And(a => a.AreaId == model.AreaId);
            }
            if (!string.IsNullOrEmpty(model.Type))
            {
                filter = filter.And(a => a.Type == model.Type);
            }
            var data = await this.GetPagedListAsync(filter, orderBy: orderby, model.Page, model.Limit, asc: false);

            List<int> userIds = new List<int>();
            foreach (var item in data)
            {
                int userid = (int)item.CreatedBy;
                if (!(userIds.IndexOf(userid) >= 0))
                {
                    userIds.Add(userid);
                }

                userid = (int)item.UpdatedBy;
                if (!(userIds.IndexOf(userid) >= 0))
                {
                    userIds.Add(userid);
                }

                userid = item.UserId;
                if (!(userIds.IndexOf(userid) >= 0))
                {
                    userIds.Add(userid);
                }
            }
            var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));
            var popids = userList.Select(a => a.PopulationId).Distinct().ToList();
            var poplist = await this.populationService.GetListAsync(a => popids.Contains(a.Id) && a.IsDeleted == 0);
            var dictionarys = await this.dictionaryService.GetBasicDictionaryList(1010);
            var imageIdList = data.Where(a => a.ImageIds != "").Select(a => a.ImageIds).Union(data.Where(a => a.ReplyImageIds != "").Select(a => a.ReplyImageIds)).Distinct().ToList();
            var imageIds = string.Join(",", imageIdList);
            var files = await this.fileService.GetSunFileInfoList(imageIds);
            var dictionarys8010 = await this.dictionaryService.GetBasicDictionaryList(8010);
            var list = new List<ListPhotoAnywhereModel>();
            foreach (var m in data)
            {
                var r = this.mapper.Map<ListPhotoAnywhereModel>(m);

                var user = userList.FirstOrDefault(a => a.Id == m.CreatedBy);
                r.CreatedByName = user != null ? Utils.Decrypt(user.NickName) : "";

                user = userList.FirstOrDefault(a => a.Id == m.UpdatedBy);
                r.UpdatedByName = user != null ? Utils.Decrypt(user.NickName) : "";

                user = userList.FirstOrDefault(a => a.Id == m.UserId);
                r.RealName = user != null ? Utils.Decrypt(user.NickName) : "";
                r.Mobile = user != null ? Utils.Decrypt(user.Mobile) : "";

                r.ImageFiles = string.IsNullOrWhiteSpace(m.ImageIds) ? new List<SunFileInfoDto>() : files.FindAll(a => m.ImageIds.Contains(a.Id.ToString()));
                r.ReplyImageFiles = string.IsNullOrWhiteSpace(m.ReplyImageIds) ? new List<SunFileInfoDto>() : files.FindAll(a => m.ReplyImageIds.Contains(a.Id.ToString()));
                r.AreaName = this.basicAreaService.GetParentAreaString(r.AreaId);

                user = userList.FirstOrDefault(a => a.Id == m.UserId);
                var pop = poplist.FirstOrDefault(a => a.Id == user?.PopulationId);
                r.Relationship = pop != null ? pop.Relationship : "";
                _ = int.TryParse(r.Relationship, out int relationshipId);
                var dic = dictionarys.FirstOrDefault(a => a.Code == relationshipId);
                r.Relationship = dic != null ? dic.Name : "";
                r.AvatarUrl = pop != null ? this.fileService.ToAbsolutePath(pop.HeadImageUrl) : "";
                if (r.AvatarUrl == "")
                {
                    // 获取用户的头像
                    var portraitFileId = user != null ? user.PortraitFileId : 0;
                    var userimages = await this.fileService.GetSunFileInfoList(portraitFileId.ToString());
                    if (userimages != null && userimages.Count() > 0)
                    {
                        r.AvatarUrl = userimages[0].Url;
                    }
                }
                if (string.IsNullOrWhiteSpace(r.RealName))
                {
                    r.RealName = Utils.Decrypt(pop?.RealName);
                }
                if (string.IsNullOrWhiteSpace(r.Mobile))
                {
                    r.Mobile = Utils.Decrypt(pop?.Mobile);
                }
                _ = int.TryParse(r.Type, out int typeId);
                var dic8010 = dictionarys8010.FirstOrDefault(a => a.Code == typeId);
                if (dic8010 != null)
                {
                    r.TypeDto = this.mapper.Map<BasicDictionaryDto>(dic8010);
                }
                list.Add(r);
            }

            return new StaticPagedList<ListPhotoAnywhereModel>(list, model.Page, model.Limit, data.TotalItemCount);
        }


        public async Task<DetailPhotoAnywhereModel> GetDetail(int id)
        {
            var data = await this.GetAsync(a => a.Id == id);
            var ret = mapper.Map<DetailPhotoAnywhereModel>(data);

            var files = await this.fileService.GetSunFileInfoList(data.ImageIds);
            ret.ImageFiles = files;

            files = await this.fileService.GetSunFileInfoList(data.ReplyImageIds);
            ret.ReplyImageFiles = files;

            var cUser = await this.basicUserService.GetAsync(a => a.Id == data.CreatedBy);
            if (cUser != null)
            {
                ret.RealName = Utils.Decrypt(cUser.NickName);
                ret.Mobile = Utils.Decrypt(cUser.Mobile);
                ret.CreatedByName = Utils.Decrypt(cUser.NickName);
            }
            var uUser = await this.basicUserService.GetAsync(a => a.Id == data.UpdatedBy);
            if (uUser != null)
            {
                ret.UpdatedByName = Utils.Decrypt(uUser.NickName);
            }

            var user = await this.basicUserService.GetPopulationByUserId(ret.UserId);
            if (user != null)
            {
                ret.RealName = user.RealName;
                ret.Mobile = user.Mobile;
                ret.IsHouseholder = user.IsHouseholder;
                ret.AvatarUrl = this.fileService.ToAbsolutePath(user.HeadImageUrl);
                if (ret.AvatarUrl == "")
                {
                    // 获取用户的头像
                    var portraitFileId = user != null ? user.PortraitFileId : 0;
                    var userimages = await this.fileService.GetSunFileInfoList(portraitFileId.ToString());
                    if (userimages != null && userimages.Count() > 0)
                    {
                        ret.AvatarUrl = userimages[0].Url;
                    }
                }
                ret.Relationship = await this.dictionaryService.GetNameByCode(user.Relationship);
            }
            else
            {
                var portraitFileId = cUser != null ? cUser.PortraitFileId : 0;
                var userimages = await this.fileService.GetSunFileInfoList(portraitFileId.ToString());
                if (userimages != null && userimages.Count() > 0)
                {
                    ret.AvatarUrl = userimages[0].Url;
                }
            }
            ret.AreaName = this.basicAreaService.GetParentAreaString(ret.AreaId);
            return ret;
        }


        public async Task<int> Save(CreatePhotoAnywhereModel model)
        {
            var data = mapper.Map<ConsPhotoAnywhere>(model);
            if (model.Id == 0)
            {
                if (data.CreatedByDeptId == 0)
                {
                    var user = await this.basicUserService.GetAsync(a => a.Id == data.CreatedBy);
                    if (user != null && !string.IsNullOrEmpty(user.DepartmentIds))
                    {
                        data.CreatedByDeptId = int.Parse(user.DepartmentIds);
                    }
                }
                data = await this.InsertAsync(data);
                try
                {
                    BasicRPC.AllotScore(new IntegralReq()
                    {
                        IntegralAction = IntegralAction.PhotoAnyWhere,
                        Description = "发布随手拍",
                        HouseholdId = data.HouseholdId,
                        UserId = data.CreatedBy == null ? 0 : data.CreatedBy.Value
                    });
                    _ = await SyncToDVM(data.Id);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex.Message);
                }
                return data.Id;
            }
            else
            {
                var info = await this.GetAsync(a => a.Id == model.Id);
                if (info == null)
                {
                    throw new Exception("数据不存在");
                }
                info.Descript = data.Descript;
                info.ImageIds = data.ImageIds;
                info.VideoIds = data.VideoIds;
                info.Type = data.Type;
                info.Longitude = data.Longitude;
                info.Latitude = data.Latitude;
                info.IsSync = 0;
                int res = await this.UpdateAsync(info);
                if (res > 0)
                {
                    try
                    {
                        _ = await SyncToDVM(data.Id);
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError(ex.Message);
                    }
                    return data.Id;
                }
                return res;
            }
        }

        public async Task<List<PhotoAnywhereTypeDto>> StatisticPhotoAnywhereByType(int AreaId)
        {
            var ids = AreaId.ToString();
            var tree = await this.basicAreaService.GetBasicAreaTree(AreaId);
            if (tree != null)
            {
                var list = tree.Ids;
                ids = string.Join(",", list);
            }
            var range = " and ConsPhotoAnywhere.createdAt >= date_format(date_sub(now(), INTERVAL 5 MONTH),'%Y-%m-01')"; // 近半年（6个月）

            string sql = @" SELECT ConsPhotoAnywhere.Type, count(*) as Cnt FROM ConsPhotoAnywhere INNER JOIN BasicUser ON BasicUser.id = ConsPhotoAnywhere.userId WHERE ConsPhotoAnywhere.areaId in ({0}) and ConsPhotoAnywhere.isDeleted = 0 {1} GROUP BY ConsPhotoAnywhere.type";

            sql = string.Format(sql, ids, range);

            var pageData = this.Context.Database.SqlQuery<PhotoAnywhereTypeDto>(sql);
            var dictionaries = await this.dictionaryService.GetBasicDictionaryList(8010);
            foreach (var item in pageData)
            {
                int type = 0;
                int.TryParse(item.Type, out type);
                var dictionary = dictionaries.FirstOrDefault(a => a.Code == type);
                if (dictionary != null)
                {
                    item.Type = dictionary.Name;
                }
            }
            return pageData;
        }

        public async Task<List<PhotoAnywhereTypeMonthDto>> StatisticPhotoAnywhereByTypeMonth(int AreaId)
        {
            var ids = AreaId.ToString();
            var tree = await this.basicAreaService.GetBasicAreaTree(AreaId);
            if (tree != null)
            {
                var list = tree.Ids;
                ids = string.Join(",", list);
            }
            var range = " and ConsPhotoAnywhere.createdAt >= date_format(date_sub(now(), INTERVAL 5 MONTH),'%Y-%m-01')"; // 近半年（6个月）

            string sql = @" select Month,sum(cnt1) as Cnt1,sum(cnt2) as Cnt2 from (select count(ConsPhotoAnywhere.id) as cnt1, 0 as cnt2,substr(ConsPhotoAnywhere.createdAt,1,7) as month from ConsPhotoAnywhere  INNER JOIN BasicUser ON BasicUser.id = ConsPhotoAnywhere.userId WHERE ConsPhotoAnywhere.areaId in ({0}) and ConsPhotoAnywhere.isDeleted = 0 {2} and isReply in (1,2) group by month
            union all 
            select 0 as cnt1, count(ConsPhotoAnywhere.id) as cnt2,substr(ConsPhotoAnywhere.createdAt,1,7) as month from ConsPhotoAnywhere INNER JOIN BasicUser ON BasicUser.id = ConsPhotoAnywhere.userId WHERE ConsPhotoAnywhere.areaId in ({1}) and ConsPhotoAnywhere.isDeleted = 0 {3} and isReply = 2 group by month) a 
            group by month order by month ";

            sql = string.Format(sql, ids, ids, range, range);

            var pageData = this.Context.Database.SqlQuery<PhotoAnywhereTypeMonthDto>(sql);
            return pageData;
        }

        public List<PhotoAnywhereTypeDto> StatisticPhotoAnywhereByStatus(int AreaId)
        {
            string sql = @" SELECT CAST(isReply as char) as Type, count(*) as Cnt FROM ConsPhotoAnywhere INNER JOIN BasicUser ON BasicUser.id = ConsPhotoAnywhere.userId
            WHERE ConsPhotoAnywhere.areaId = {0} and ConsPhotoAnywhere.isDeleted = 0 GROUP BY ConsPhotoAnywhere.isReply";

            sql = string.Format(sql, AreaId);

            var pageData = this.Context.Database.SqlQuery<PhotoAnywhereTypeDto>(sql);
            return pageData;
        }

        public async Task<List<BasicDictionaryDto>> GetPhotoAnywhereType()
        {
            var list = await this.dictionaryService.GetBasicDictionaryList(8010);
            return mapper.Map<List<BasicDictionaryDto>>(list);
        }

        /// <summary>
        /// 我发的随手拍数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
		public async Task<int> MyPhotoAnywhereCount(int userId)
        {
            return await this.GetQueryable().Where(a => a.UserId == userId && a.IsDeleted == 0).CountAsync();
        }

        /// <summary>
        /// 我处理的随手拍数量
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
		public async Task<int> MyReplyPhotoAnywhereCount(int userId)
        {
            return await this.GetQueryable().Where(a => a.UpdatedBy == userId && a.IsDeleted == 0 && a.IsReply == 2).CountAsync();
        }

        public async Task<IPagedList<ListPhotoAnywhereModel>> MyReplyDataList(PhotoAnywhereListQueryModel model)
        {
            Expression<Func<ConsPhotoAnywhere, bool>> filter = a => a.IsDeleted == 0;

            Expression<Func<ConsPhotoAnywhere, object>> orderby = a => a.CreatedAt;

            if (!model.Keyword.IsNullOrEmpty())
            {
                filter = filter.And(a => a.Descript.ToLower().Contains(model.Keyword));
            }

            filter = filter.And(a => a.IsReply == 2);
            filter = filter.And(a => a.UpdatedBy == model.UserId);

            if (!string.IsNullOrEmpty(model.Type))
            {
                filter = filter.And(a => a.Type == model.Type);
            }
            var data = await this.GetPagedListAsync(filter, orderBy: orderby, model.Page, model.Limit, asc: false);

            List<int> userIds = new List<int>();
            foreach (var item in data)
            {
                int userid = (int)item.CreatedBy;
                if (!(userIds.IndexOf(userid) >= 0))
                {
                    userIds.Add(userid);
                }

                userid = (int)item.UpdatedBy;
                if (!(userIds.IndexOf(userid) >= 0))
                {
                    userIds.Add(userid);
                }

                userid = item.UserId;
                if (!(userIds.IndexOf(userid) >= 0))
                {
                    userIds.Add(userid);
                }
            }
            var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));
            var popids = userList.Select(a => a.PopulationId).Distinct().ToList();
            var poplist = await this.populationService.GetListAsync(a => popids.Contains(a.Id) && a.IsDeleted == 0);
            var imageIdList = data.Where(a => a.ImageIds != "").Select(a => a.ImageIds).Union(data.Where(a => a.ReplyImageIds != "").Select(a => a.ReplyImageIds)).Distinct().ToList();
            var imageIds = string.Join(",", imageIdList);
            var files = await this.fileService.GetSunFileInfoList(imageIds);
            var list = new List<ListPhotoAnywhereModel>();
            foreach (var m in data)
            {
                var r = this.mapper.Map<ListPhotoAnywhereModel>(m);

                var user = userList.FirstOrDefault(a => a.Id == m.CreatedBy);
                r.CreatedByName = user != null ? Utils.Decrypt(user.NickName) : "";

                user = userList.FirstOrDefault(a => a.Id == m.UpdatedBy);
                r.UpdatedByName = user != null ? Utils.Decrypt(user.NickName) : "";

                user = userList.FirstOrDefault(a => a.Id == m.UserId);
                r.RealName = user != null ? Utils.Decrypt(user.NickName) : "";

                //var files = await this.fileService.GetSunFileInfoList(m.ImageIds);
                //r.ImageFiles = files;

                //files = await this.fileService.GetSunFileInfoList(m.ReplyImageIds);
                //r.ReplyImageFiles = files;

                r.ImageFiles = string.IsNullOrWhiteSpace(m.ImageIds) ? new List<SunFileInfoDto>() : files.FindAll(a => m.ImageIds.Contains(a.Id.ToString()));
                r.ReplyImageFiles = string.IsNullOrWhiteSpace(m.ReplyImageIds) ? new List<SunFileInfoDto>() : files.FindAll(a => m.ReplyImageIds.Contains(a.Id.ToString()));

                var pop = poplist.FirstOrDefault(a => a.Id == user?.PopulationId);
                r.AvatarUrl = pop != null ? this.fileService.ToAbsolutePath(pop.HeadImageUrl) : "";
                if (r.AvatarUrl == "")
                {
                    // 获取用户的头像
                    var portraitFileId = user != null ? user.PortraitFileId : 0;
                    var userimages = await this.fileService.GetSunFileInfoList(portraitFileId.ToString());
                    if (userimages != null && userimages.Count() > 0)
                    {
                        r.AvatarUrl = userimages[0].Url;
                    }
                }

                list.Add(r);
            }

            return new StaticPagedList<ListPhotoAnywhereModel>(list, model.Page, model.Limit, data.TotalItemCount);
        }

        public List<PhotoAnywhereTypeDto> StatisticPhotoAnywhereStatusByUser(int userId)
        {
            string sql = @" SELECT CAST(isReply as char) as Type, count(*) as Cnt FROM ConsPhotoAnywhere 
            WHERE userId = {0} and isDeleted = 0 GROUP BY ConsPhotoAnywhere.isReply";

            sql = string.Format(sql, userId);

            var pageData = this.Context.Database.SqlQuery<PhotoAnywhereTypeDto>(sql);
            return pageData;
        }

        public async Task<int> SyncToDVM(int id)
        {
            if (configuration.GetValue<string>("BIGDATA:HOST") == null)
            {
                this.logger.LogError("未配置数据同步地址");
                return 0;
            }
            var model = await this.GetDetail(id);

            var cUser = await this.basicUserService.GetAsync(model.UserId);
            var areaid = 0;
            if (cUser != null)
            {
                areaid = cUser.AreaId;
            }
            else
            {
                this.logger.LogError("非微信发布的随手拍不需要同步");
                return 0;
            }

            var area = await this.basicAreaService.GetAsync(a => a.Id == areaid);
            if (area == null)
            {
                this.logger.LogError("随手拍区域id数据不存在");
                return 0;
            }

            var dictionarys8010 = await this.dictionaryService.GetBasicDictionaryList(8010);
            _ = int.TryParse(model.Type, out int typeId);
            var dic = dictionarys8010.FirstOrDefault(a => a.Code == typeId);
            var typeName = dic != null ? dic.Name : "";
            // 查询打点记录获取familyid
            var plotitems = await this.gisPlotItemService.ListPlotItemAsync(new GISListQueryModel()
            {
                PlotType = 4,
                ObjectId = model.HouseholdId // 随手拍增加了户码id
            });
            var list = plotitems.Where(a => a.IsDeleted == 0).Select(a => a.SyncId);
            var familyId = "0";
            if (list != null && list.Count() > 0)
            {
                familyId = list.First();
            }
            //if (list == null || list.Count() == 0)
            //{
            //    Console.WriteLine("户码数据没有打点");
            //    return 0;
            //}
            var t = model.ReplyImageFiles.Select(a => a.Url).ToArray();
            Object data = new
            {
                familyId = familyId,
                sourceId = area.AreaCode + "-" + model.Id,
                organizationCode = area.AreaCode,
                title = model.RealName + typeName + "事件",
                type = typeName,
                reportTime = model.CreatedAt,
                address = model.Address,
                state = model.IsReply == 2 ? "已办理" : "已上报",
                eventLevel = "一般事件",
                userName = model.RealName,
                mobile = model.Mobile,
                descript = model.Descript,
                pictures = model.ImageFiles.Select(a => a.Url).ToList(),
                replyDesc = model.ReplyDesc,
                replyImages = model.ReplyImageFiles.Select(a => a.Url).ToList(),
                replyDateTime = model.ReplyDateTime,
                replyer = model.Replyer,
                latitude = model.Latitude,
                longitude = model.Longitude
            };
            var url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Snapshot/Add";
            if (model.IsDeleted == 1)
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Snapshot/" + area.AreaCode + "-" + model.Id;
                return await DeleteAsync(model, url);
            }
            else if (model.SyncId != "")
            {
                url = configuration.GetValue<string>("BIGDATA:HOST").ToString() + "/dvm/Snapshot/Update/" + area.AreaCode + "-" + model.Id;
                return await PutAsync(model, url, data);
            }
            else
            {
                return await PostAsync(model, url, data);
            }
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
            if (message.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(result);
                this.logger.LogError(result);
                return "";
            }
            JObject jo = (JObject)JsonConvert.DeserializeObject(result);
            var token = jo.GetValue("token_type").ToString() + " " + jo.GetValue("access_token").ToString();
            return token;
        }

        private async Task<int> PostAsync(DetailPhotoAnywhereModel model, string url, Object data)
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
                    await this.GetQueryable().Where(a => a.Id == model.Id).UpdateFromQueryAsync(a => new ConsPhotoAnywhere()
                    {
                        SyncId = SyncId,
                        IsSync = 1,
                        SyncRes = jo.GetValue("description").ToString(),
                        SyncDate = DateTime.Now,
                    });
                }
                else
                {
                    await this.GetQueryable().Where(a => a.Id == model.Id).UpdateFromQueryAsync(a => new ConsPhotoAnywhere()
                    {
                        IsSync = 1,
                        SyncRes = jo.GetValue("description").ToString(),
                        SyncDate = DateTime.Now,
                    });
                    Console.WriteLine(jo.GetValue("description").ToString());
                    return -1;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                Console.WriteLine(ex.Message);
                return -1;
            }
            return 0;
        }

        private async Task<int> PutAsync(DetailPhotoAnywhereModel model, string url, Object data)
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
                    await this.GetQueryable().Where(a => a.Id == model.Id).UpdateFromQueryAsync(a => new ConsPhotoAnywhere()
                    {
                        IsSync = 1,
                        SyncRes = jo.GetValue("description").ToString(),
                        SyncDate = DateTime.Now,
                    });
                }
                else
                {
                    await this.GetQueryable().Where(a => a.Id == model.Id).UpdateFromQueryAsync(a => new ConsPhotoAnywhere()
                    {
                        IsSync = 1,
                        SyncRes = jo.GetValue("description").ToString(),
                        SyncDate = DateTime.Now,
                    });
                    Console.WriteLine(jo.GetValue("description").ToString());
                    return -1;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                Console.WriteLine(ex.Message);
                return -1;
            }
            return 0;
        }

        private async Task<int> DeleteAsync(DetailPhotoAnywhereModel model, string url)
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
                    await this.GetQueryable().Where(a => a.Id == model.Id).UpdateFromQueryAsync(a => new ConsPhotoAnywhere()
                    {
                        IsSync = 1,
                        SyncRes = jo.GetValue("description").ToString(),
                        SyncDate = DateTime.Now,
                    });
                }
                else
                {
                    await this.GetQueryable().Where(a => a.Id == model.Id).UpdateFromQueryAsync(a => new ConsPhotoAnywhere()
                    {
                        IsSync = 1,
                        SyncRes = jo.GetValue("description").ToString(),
                        SyncDate = DateTime.Now,
                    });
                    Console.WriteLine(model.SyncRes);
                    return -1;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
                Console.WriteLine(ex.Message);
                return -1;
            }
            return 0;
        }

        public async Task<int> Reply(ReplyPhotoAnywhereModel model, LoginUser loginuser)
        {
            var info = await this.GetAsync(a => a.Id == model.Id);
            if (info == null)
            {
                throw new Exception("数据不存在");
            }
            else
            {
                info.ReplyDesc = model.ReplyDesc;
                info.ReplyImageIds = model.ReplyImageIds;
                info.ReplyDateTime = DateTime.Now;
                info.IsReply = 2;
                info.Replyer = loginuser.NickName;
                info.IsSync = 0;
                if (loginuser != null)
                {
                    info.UpdatedBy = loginuser.UserId;
                }
                var ret = await this.UpdateAsync(info);
                _ = await SyncToDVM(model.Id);
                return ret;
            }
        }

        public async Task<int> DeletePhotoAnywhereAsync(int id, int userid)
        {
            var info = await this.GetAsync(a => a.Id == id);
            if (info == null)
            {
                throw new ValidException("数据不存在");
            }
            else
            {
                info.UpdatedBy = userid;
                info.IsDeleted = 1;
                info.IsSync = 0;
                var ret = await this.UpdateAsync(info);
                _ = await SyncToDVM(id);
                return ret;
            }
        }
    }
}
