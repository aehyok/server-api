using AutoMapper;
using AutoMapper.QueryableExtensions;
using DVS.Application.Services.Common;
using DVS.Common.Infrastructures;
using DVS.Common.Services;
using DVS.Common.SO;
using DVS.Core.Domains.Village;
using DVS.Model.Dtos.Village;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Village;
using DVS.Models.Dtos.Village.Query;
using DVS.Models.Dtos.Village.Statistics;
using DVS.Models.Enum;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.Village
{
    public class EpidemicService : ServiceBase<VillageEpidemic>, IEpidemicService
    {
        private readonly IPopulationService populationService;
        private readonly ISunFileInfoService sunFileInfoService;
        private readonly IBasicDictionaryService basicDictionaryService;
        private readonly IPopulationTagService populationTagService;
        private readonly IBasicAreaService basicAreaService;
        private readonly IBasicUserService basicUserService;
        private readonly IPopulationAddressService populationAddressService;
        public EpidemicService(DbContext dbContext, IMapper mapper, IPopulationService populationService,
            ISunFileInfoService sunFileInfoService,
            IBasicDictionaryService basicDictionaryService,
            IPopulationTagService populationTagService,
            IBasicAreaService basicAreaService,
            IBasicUserService basicUserService,
            IPopulationAddressService populationAddressService
            )
            : base(dbContext, mapper)
        {
            this.populationService = populationService;
            this.sunFileInfoService = sunFileInfoService;
            this.basicDictionaryService = basicDictionaryService;
            this.populationTagService = populationTagService;
            this.basicAreaService = basicAreaService;
            this.basicUserService = basicUserService;
            this.populationAddressService = populationAddressService;
        }

        public async Task<List<EpidemicPopulationDto>> GetEpidemicInfoList(int householdId, int year, int populationId = 0)
        {
            var sql = "select distinct ve.id,ve.isFever,vp.AreaId,vp.Sex,vp.RealName,vp.HeadImageUrl,ve.Health,"+householdId+ " as householdId,ve.IsInAreas,vp.Mobile,ve.PopulationId,ve.RecorDate,vp.Relationship,ve.Remark,ve.ReversalDate,ve.SourceAddress,ve.SourceOrgCodes,vp.Tags,ve.Temperature,vp.Nation,ve.CreatedAt,ve.CreatedBy,ve.UpdatedAt,ve.UpdatedBy,ve.IsDeleted,ve.Year from VillageEpidemic ve, VillagePopulation vp where ve.populationId = vp.id and vp.isDeleted = 0 and ve.isDeleted = 0";

            if (populationId > 0)
            {
                sql += " and ve.populationId = " + populationId;
                if (year > 0)
                {
                    sql += " and ve.year = " + year;
                }
            }
            else
            {
                sql += " and ve.populationId in (select populationid from VillageHouseCodeMember where householdId = " + householdId + " and isDeleted = 0) and ve.year = " + year;
            }
            sql += " order by ve.reversalDate desc";
            var datas = this.Context.Database.SqlQuery<EpidemicPopulationDto>(sql);

            List<string> dirs = new List<string>();
            foreach (var item in datas)
            {
                item.HeadImageUrl = this.sunFileInfoService.ToAbsolutePath(item.HeadImageUrl);
                item.RealName = BasicSO.Decrypt(item.RealName);
                item.Mobile = BasicSO.Decrypt(item.Mobile);

                if (item.Relationship != "")
                {
                    dirs.Add(item.Relationship);
                }
                if (item.Nation != "")
                {
                    dirs.Add(item.Nation);
                }
            }

            List<int> userIds = datas.Select(a => a.CreatedBy).Union(datas.Select(a => a.UpdatedBy)).Distinct().ToList();
            var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));
            var dictionaries = await this.basicDictionaryService.GetBasicDictionaryCodeList(dirs);
            List<int> populationIds = datas.Select(a => a.PopulationId).Distinct().ToList();
            var tagNames = await this.populationTagService.GetTags(populationIds);
            foreach (var item in datas)
            {
                item.Relationship = await this.basicDictionaryService.GetNameByCode(item.Relationship, dictionaries);
                item.Nation = await this.basicDictionaryService.GetNameByCode(item.Nation, dictionaries);
                var user = userList.FirstOrDefault(a => a.Id == item.CreatedBy);
                item.CreatedByName = user != null ? user.NickName : "";
                user = userList.FirstOrDefault(a => a.Id == item.UpdatedBy);
                item.UpdatedByName = user != null ? user.NickName : "";
                item.SourceAddressInfo = await this.populationAddressService.GetAddressDetail(item.Id, PopulationAddressTypeEnum.返乡来源地);
                item.TagNames = tagNames.Where(a => a.Pid == item.PopulationId).ToList();
            }

            return datas;
        }

        public async Task<IPagedList<EpidemicPopulationDto>> GetEpidemicInfoList(int householdId, int year, int populationId = 0, int page = 1, int limit = 10)
        {
            var sql = "select distinct ve.id,ve.isFever,vp.AreaId,vp.Sex,vp.RealName,vp.HeadImageUrl,ve.Health,vc.HouseholdId,ve.IsInAreas,vp.Mobile,ve.PopulationId,ve.RecorDate,vp.Relationship,ve.Remark,ve.ReversalDate,ve.SourceAddress,ve.SourceOrgCodes,vp.Tags,ve.Temperature,vp.Nation,ve.CreatedAt,ve.CreatedBy,ve.UpdatedAt,ve.UpdatedBy,ve.IsDeleted,ve.Year from VillageEpidemic ve, VillagePopulation vp, VillageHouseCodeMember vc where ve.populationId = vp.id and vp.id = vc.populationId and vp.isDeleted = 0 and vc.isDeleted = 0 and ve.isDeleted = 0";
            string sqlCount = "select distinct ve.id from VillageEpidemic ve, VillagePopulation vp, VillageHouseCodeMember vc where ve.populationId = vp.id and vp.id = vc.populationId and vp.isDeleted = 0 and vc.isDeleted = 0 and ve.isDeleted = 0";

            if (populationId > 0)
            {
                sql += " and ve.populationId = " + populationId;
                sqlCount += " and ve.populationId = " + populationId;
                if (year > 0)
                {
                    sql += " and ve.year = " + year;
                    sqlCount += " and ve.year = " + year;
                }
            }
            if (householdId > 0)
            {
                sql += " and vc.householdId = " + householdId + " and ve.year = " + year;
                sqlCount += " and vc.householdId = " + householdId + " and ve.year = " + year;
            }

            var datas = this.Context.Database.SqlQueryPagedList<EpidemicPopulationDto>(page, limit, sql, sqlCount, " order by reversalDate desc");

            List<string> dirs = datas.Select(a => a.Relationship).Union(datas.Select(a => a.Nation)).Distinct().ToList();
            List<int> userIds = datas.Select(a => a.CreatedBy).Union(datas.Select(a => a.UpdatedBy)).Distinct().ToList();
            var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));
            var dictionaries = await this.basicDictionaryService.GetBasicDictionaryCodeList(dirs);
            List<int> populationIds = datas.Select(a => a.PopulationId).Distinct().ToList();
            var tagNames = await this.populationTagService.GetTags(populationIds);

            var dic6010s = await this.basicDictionaryService.GetBasicDictionaryList(DictionariyEnum.D6010);
            var dic6011s = await this.basicDictionaryService.GetBasicDictionaryList(DictionariyEnum.D6011);

            foreach (var item in datas)
            {
                item.HeadImageUrl = this.sunFileInfoService.ToAbsolutePath(item.HeadImageUrl);
                // item.RealName = BasicSO.Decrypt(item.RealName);
                item.Mobile = BasicSO.Decrypt(item.Mobile);
                item.Relationship = await this.basicDictionaryService.GetNameByCode(item.Relationship, dictionaries);
                item.Nation = await this.basicDictionaryService.GetNameByCode(item.Nation, dictionaries);
                var user = userList.FirstOrDefault(a => a.Id == item.CreatedBy);
                item.CreatedByName = user != null ? user.NickName : "";
                user = userList.FirstOrDefault(a => a.Id == item.UpdatedBy);
                item.UpdatedByName = user != null ? user.NickName : "";
                item.SourceAddressInfo = await this.populationAddressService.GetAddressDetail(item.Id, PopulationAddressTypeEnum.返乡来源地);
                item.TagNames = tagNames.Where(a => a.Pid == item.PopulationId).ToList();



                _ = int.TryParse(item.Health, out int healthCode);
                if (healthCode >= DictionariyEnum.D6010100)
                {
                    var dic = dic6010s.FirstOrDefault(a => a.Code == healthCode);
                    if (dic != null)
                    {
                        item.HealthDto = mapper.Map<BasicDictionaryDto>(dic);
                    }
                }
                else
                {
                    var dic = dic6010s.FirstOrDefault(a => a.Name == item.Health);
                    if (dic != null)
                    {
                        item.HealthDto = mapper.Map<BasicDictionaryDto>(dic);
                    }
                }

                if (item.IsInAreas >= DictionariyEnum.D6011100)
                {
                    var dic = dic6011s.FirstOrDefault(a => a.Code == item.IsInAreas);
                    if (dic != null)
                    {
                        item.IsInAreasDto = mapper.Map<BasicDictionaryDto>(dic);
                    }
                }
                else
                {
                    var dic = dic6011s.FirstOrDefault(a => a.Name == (item.IsInAreas == 1 ? "有" : "无"));
                    if (dic != null)
                    {
                        item.IsInAreasDto = mapper.Map<BasicDictionaryDto>(dic);
                    }

                }
            }

            return datas;
        }

        public async Task<IPagedList<VillageEpidemicDto>> GetEpidemicList(PagePostBody body)
        {
            string sqlWhere = "";
            if (!string.IsNullOrWhiteSpace(body.Keyword))
            {
                sqlWhere = string.Format(" and (p.realName like '%{0}%' or h.houseName  LIKE '%{0}%'  or h.houseNumber  LIKE '%{0}%' ) ", body.Keyword);
            }

            if (!string.IsNullOrWhiteSpace(body.Ids))
            {
                sqlWhere += " and h.id in (" + body.Ids + ")";
            }

            string sql = string.Format(@" SELECT t.*,
                            IFNULL(t2.peopleCount,0) as peopleCount 
                            from 
                            (
							SELECT 
							    h.id,
							    h.areaId,
							    h.houseName,
							    h.houseNumber,
                                p.realName as householdMan,
							    IFNULL(p.sex,0)  as sex,
							    p.nation,
                                p.mobile,
                                a.name as areaName 
						    from VillageHouseholdCode h
                            LEFT JOIN BasicArea a on h.areaId = a.id
                            LEFT JOIN  (SELECT v.realName,v.sex,v.mobile,v.nation, m.householdId FROM  VillagePopulation v, VillageHouseCodeMember m WHERE v.id = m.populationId and v.areaId={0} and m.isHouseholder=1 and v.isDeleted=0 and m.isDeleted=0 )p  on h.id= p.householdId
                            where h.isDeleted=0 and a.isDeleted=0 and h.areaId={0} {1} order by h.createdAt desc
                          )t
                          LEFT JOIN
                        (SELECT COUNT(*) as peopleCount, m.householdId from VillageEpidemic w, VillageHouseCodeMember m, VillagePopulation v where w.populationId = m.populationId and v.id = m.populationId and v.isDeleted = 0 and w.isDeleted=0 and m.isDeleted=0 and w.year={2} and v.areaId={0} GROUP BY m.householdId)t2
                         on t.id = t2.householdId ", body.AreaId, sqlWhere, body.Year);

            var orderby = " order by areaName asc,convert(houseName using gbk) asc, convert(houseName using gbk) asc ";
            if (body.Orders != null)
            {
                var orders = new List<string>();
                foreach (var order in body.Orders)
                {
                    orders.Add("convert(" + order.FieldName + " using gbk) " + order.Sort);
                }
                orderby = " order by " + string.Join(",", orders);
            }
            var pageData = this.Context.Database.SqlQueryPagedList<VillageEpidemicDto>(body.Page, body.Limit, sql, "", orderby);

            List<string> dirs = new List<string>();
            foreach (var item in pageData)
            {
                dirs.Add(item.Nation);
            }
            var dirNames = await this.basicDictionaryService.GetBasicDictionaryCodeList(dirs);
            foreach (var item in pageData)
            {
                item.Nation = await this.basicDictionaryService.GetNameByCode(item.Nation, dirNames);
                item.HouseholdMan = BasicSO.Decrypt(item.HouseholdMan);
                item.Mobile = BasicSO.Decrypt(item.Mobile);
            }

            return pageData;
        }

        public async Task<List<EpidemicPopulationDto>> GetEpidemicPopulationList(int householdId)
        {
            string sql = string.Format(@" SELECT 
                p.id as populationId,
                p.areaId,
                m.householdId,
                p.sex,
                p.realName,
                p.relationship,
                p.tags,p.mobile,
                p.headImageUrl,
                p.nation
                FROM VillagePopulation p , VillageHouseCodeMember m
                WHERE p.id = m.populationId and m.householdId={0} and p.isDeleted=0 and m.isDeleted = 0 ", householdId);
            var list = this.Context.Database.SqlQuery<EpidemicPopulationDto>(sql);
            List<string> dirs = new List<string>();
            List<int> populationIds = new List<int>();
            foreach (var item in list)
            {
                populationIds.Add(item.PopulationId);
                item.HeadImageUrl = this.sunFileInfoService.ToAbsolutePath(item.HeadImageUrl);
                dirs.Add(item.Relationship);
                dirs.Add(item.Nation);
            }
            var dirNames = await this.basicDictionaryService.GetBasicDictionaryCodeList(dirs);
            var tagNames = await this.populationTagService.GetTags(populationIds);

            List<VillageEpidemic> epidemiclist = this.GetQueryable().Where(a => populationIds.Contains(a.PopulationId) && a.IsDeleted == 0).OrderBy(a => a.PopulationId).OrderByDescending(a => a.ReversalDate).ToList();

            List<int?> userIds = epidemiclist.Select(a => a.CreatedBy).Distinct().Union(epidemiclist.Select(a=>a.UpdatedBy).Distinct()).Distinct().ToList();
            var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));
            foreach (var item in list)
            {
                item.TagNames = tagNames.Where(a => a.Pid == item.PopulationId).ToList();
                item.Relationship = await this.basicDictionaryService.GetNameByCode(item.Relationship, dirNames);
                item.RealName = BasicSO.Decrypt(item.RealName);
                item.Mobile = BasicSO.Decrypt(item.Mobile);
                item.Nation = await this.basicDictionaryService.GetNameByCode(item.Nation, dirNames);

                var epidemc = epidemiclist.FirstOrDefault(a => a.PopulationId == item.PopulationId);
                if (epidemc != null){
                    item.SourceOrgCodes = epidemc.SourceOrgCodes;
                    item.ReversalDate = epidemc.ReversalDate;
                    item.Temperature = epidemc.Temperature;
                    item.Health = epidemc.Health;
                    item.IsFever = epidemc.IsFever;
                    item.IsInAreas = epidemc.IsInAreas;
                    item.Remark = epidemc.Remark;
                    item.RecorDate = epidemc.RecorDate;
                    item.SourceAddress = epidemc.SourceAddress;
                    item.Id = epidemc.Id;
                    item.UpdatedBy = (int)epidemc.UpdatedBy;
                    item.CreatedBy = (int)epidemc.CreatedBy;
                    item.Year = epidemc.Year;

                    item.SourceAddressInfo = await this.populationAddressService.GetAddressDetail(item.Id, PopulationAddressTypeEnum.返乡来源地);
                    var user = userList.FirstOrDefault(a => a.Id == item.CreatedBy);
                    item.CreatedByName = user != null ? user.NickName : "";

                    user = userList.FirstOrDefault(a => a.Id == item.UpdatedBy);
                    item.UpdatedByName = user != null ? user.NickName : "";
                }

            }
            return list;
        }

        /// 1 户籍人口返乡记录，2 14天内异常人员，3 累计异常，4 按年月查询返乡记录
        public async Task<IPagedList<EpidemicPopulationDto>> GetEpidemicPopulationInfoList(EpidemicInfoListBody body)
        {
            string sql = "";
            string sqlWhere = "";
            string sqlWhere_ep = "";
            if (!string.IsNullOrWhiteSpace(body.Keyword))
            {
                sqlWhere = string.Format(" and (v.realName like '%{0}%' or v.mobileShort like '%{0}%'or v.mobile = '{1}') ", body.Keyword, BasicSO.Encrypt(body.Keyword));
            }

            string orderby = " order by createdAt desc";
            if (body.SearchType == 1)
            {
                // IsNormal 是否正常 0 所有 1 正常 2 异常
                if (body.IsNormal == 1) { sqlWhere_ep = " and (e.health = '6010100') "; }
                if (body.IsNormal == 2) { sqlWhere_ep = " and e.health <> '6010100' "; }
                if (body.IsNormal == 0)
                {
                    sql = string.Format(@" select p.*,t.sourceOrgCodes,
                                t.reversalDate,
                                t.temperature,
                                t.health,
                                t.isFever,
                                t.isInAreas,
                                t.remark,
                                t.recorDate,
                                t.sourceAddress,0 as householdId,t.id,
                                t.updatedBy,
                                t.createdBy,
                                t.year
                                from 
                                (
                                    SELECT v.id as populationId, v.areaId, v.sex, v.realName, v.relationship, v.tags, v.mobile, v.headImageUrl,v.createdAt,v.nation
                                    from VillagePopulation v
                                    where v.isDeleted=0 and v.areaId={0} {2}
                                ) p LEFT JOIN 
							    (   SELECT ep.* FROM 
								             ( select max(vc.id) as id from VillageEpidemic vc , (
                                                    SELECT max(reversalDate) as reversalDate, e.populationId FROM  VillageEpidemic e WHERE e.isDeleted = 0 and YEAR(e.reversalDate) = {1} {3}
                                                        GROUP BY e.populationId) a
                                               where vc.reversalDate = a.reversalDate and vc.populationId = a.populationId GROUP BY vc.populationId) t1
								           LEFT JOIN VillageEpidemic ep on ep.id = t1.id
						        ) t on p.populationId = t.populationId ",
                    body.AreaId, body.Year, sqlWhere, sqlWhere_ep);
                }
                else {
                    sql = string.Format(@" select 
                                v.id as populationId, v.areaId, v.sex, v.realName, v.relationship, v.tags, v.mobile, v.headImageUrl,v.createdAt,v.nation,e.sourceOrgCodes,
                                e.reversalDate,
                                e.temperature,
                                e.health,
                                e.isFever,
                                e.isInAreas,
                                e.remark,
                                e.recorDate,
                                e.sourceAddress,0 as householdId,e.id,
                                e.updatedBy,
                                e.createdBy,
                                e.year
                                from VillagePopulation v, VillageEpidemic e, (select populationId,max(reversalDate) as reversalDate,max(id) as id from VillageEpidemic e where e.isDeleted=0 and YEAR(e.reversalDate) = {1} {2} {3} group by e.populationId) f
                                where v.id = e.populationId and e.id = f.id and e.isDeleted=0 and v.isDeleted=0 and v.areaId={0} and YEAR(e.reversalDate) = {1} {2} {3}",
                    body.AreaId, body.Year, sqlWhere, sqlWhere_ep);
                    orderby = " order by reversalDate desc";
                }
            }
            else
            {
                switch (body.SearchType)
                {
                    case 2:
                        sqlWhere += string.Format(@" and e.health <> '6010100' and date(e.reversalDate)>=DATE_SUB(curdate(),INTERVAL 14 DAY) and YEAR(e.reversalDate) = {0} ", body.Year);
                        break;
                    case 3:
                        sqlWhere += string.Format(@" and e.health <> '6010100' and YEAR(e.reversalDate) = {0} ", body.Year);
                        break;
                    case 4:
                        if (body.IsNormal == 1) { sqlWhere += " and e.health = '6010100' "; }
                        if (body.IsNormal == 2) { sqlWhere += " and e.health <> '6010100' "; }
                        sqlWhere += string.Format(@" and YEAR(e.reversalDate) = {0} and MONTH(e.reversalDate) = {1} ", body.Year, body.Month);
                        break;
                    default:
                        break;
                }

                sql = string.Format(@" SELECT 
                v.id as populationId,
                v.areaId,
                0 as householdId,
                v.sex,
                v.realName,
                v.relationship,
                v.tags,
                v.mobile,
                v.headImageUrl,
                v.nation,
                e.sourceOrgCodes,
                e.reversalDate,
                e.temperature,
                e.health,
                e.isFever,
                e.isInAreas,
                e.remark,
                e.recorDate,
                e.sourceAddress,
                e.id,
                e.updatedBy,
                e.createdBy,
                e.year
                FROM VillagePopulation v, VillageEpidemic e ,(select populationId,max(reversalDate) as reversalDate,max(id) as id from VillageEpidemic e where e.isDeleted=0 {1} group by e.populationId) f
                WHERE v.id = e.populationId and e.id = f.id and e.isDeleted=0 and v.areaId= {0} and v.isDeleted=0 {1}", body.AreaId, sqlWhere);
                orderby = " order by reversalDate desc";
            }

            var list = this.Context.Database.SqlQueryPagedList<EpidemicPopulationDto>(body.Page, body.Limit, sql, "", orderby);
            if (list == null || list.Count() == 0) {
                return list;
            }
            List<string> dirs = new List<string>();
            List<int> populationIds = new List<int>();
            foreach (var item in list)
            {
                populationIds.Add(item.PopulationId);
                item.HeadImageUrl = this.sunFileInfoService.ToAbsolutePath(item.HeadImageUrl);
                dirs.Add(item.Relationship);
            }
            var dirNames = await this.basicDictionaryService.GetBasicDictionaryCodeList(dirs);
            var tagNames = await this.populationTagService.GetTags(populationIds);
            List<int> userIds = list.Select(a => a.CreatedBy).Union(list.Select(a => a.UpdatedBy)).Distinct().ToList();
            var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));
            var nationids = list.Select(a => a.Nation).Distinct().ToList();
            var dictionaries = await this.basicDictionaryService.GetBasicDictionaryCodeList(nationids);

            var ids = list.Select(a => a.PopulationId).Distinct().ToList();
            sql = string.Format(@" select distinct vc.householdId,vc.populationId,vc.isHouseholder,vh.houseName, vh.houseNumber,vp.realName, vp.areaid,vp.relationship
                    from VillageHouseCodeMember vc,VillageHouseholdCode vh ,VillagePopulation vp
                    where vc.populationId = vp.id and vc.householdId = vh.id and vp.id in ({0}) and 
                    vc.isDeleted = 0 and vp.isDeleted = 0 and vh.isDeleted = 0 order by populationId", string.Join(",", ids));
            var listhousehold = this.Context.Database.SqlQuery<HouseholderAndHouseNumberDto>(sql).ToList();

            foreach (var item in list)
            {
                item.TagNames = tagNames.Where(a => a.Pid == item.PopulationId).ToList();
                item.Relationship = await this.basicDictionaryService.GetNameByCode(item.Relationship, dirNames);
                item.RealName = BasicSO.Decrypt(item.RealName);
                item.Mobile = BasicSO.Decrypt(item.Mobile);
                item.SourceAddressInfo = await this.populationAddressService.GetAddressDetail(item.Id, PopulationAddressTypeEnum.返乡来源地);
                var user = userList.FirstOrDefault(a => a.Id == item.CreatedBy);
                item.CreatedByName = user != null ? user.NickName : "";
                user = userList.FirstOrDefault(a => a.Id == item.UpdatedBy);
                item.UpdatedByName = user != null ? user.NickName : "";
                item.Nation = await this.basicDictionaryService.GetNameByCode(item.Nation, dictionaries);

                // 判断户籍人口是否设置户码(HouseholdList=null)，属于一户(HouseholdList.count()=1，属于多户(HouseholdList.count()>1)
                item.HouseholdList = listhousehold.FindAll(a => a.PopulationId == item.PopulationId);
                if (item.HouseholdList != null && item.HouseholdList.Count() == 1)
                {
                    item.HouseholdId = item.HouseholdList[0].HouseholdId;
                }
            }
            return list;
        }

        public async Task<MessageResult<bool>> SaveEpidemicInfo(VillageEpidemic body)
        {
            var result = new MessageResult<bool>("失败", false, false);
            //if (body.HouseholdId <= 0)
            //{
            //    result.Message = "请输入户码Id";
            //    return result;
            //}

            if (body.PopulationId <= 0)
            {
                result.Message = "请输入人口Id";
                return result;
            }
            if (body.ReversalDate == null)
            {
                result.Message = "返乡日期不能为空";
                return result;
            }

            if (body.SourceAddressInfo == null)
            {
                result.Message = "返乡来源地不能为空";
                return result;
            }
            if (string.IsNullOrWhiteSpace(body.Health))
            {
                result.Message = "异常情况不能为空";
                return result;
            }
            //if (string.IsNullOrWhiteSpace(body.Temperature))
            //{
            //    result.Message = "体温不能为空";
            //    return result;
            //}
            body.RecorDate = DateTime.Now;
            body.Year = ((DateTime)body.ReversalDate).Year;

            if (body.Id > 0)
            {
                var res = await this.GetQueryable().Where(a => a.Id == body.Id)
                    .UpdateFromQueryAsync(a => new VillageEpidemic()
                    {
                        IsFever = body.IsFever,
                        Health = body.Health,
                        IsInAreas = body.IsInAreas,
                        // RecorDate= body.RecorDate,
                        Remark = body.Remark,
                        ReversalDate = body.ReversalDate,
                        SourceAddress = body.SourceAddress,
                        SourceOrgCodes = body.SourceOrgCodes,
                        Temperature = body.Temperature,
                        IsSync = 0,
                        UpdatedBy = body.UpdatedBy,
                    });
                if (res > 0)
                {
                    result.Message = "成功";
                    result.Flag = true;
                    result.Data = true;
                    await this.populationAddressService.SaveAddress(body.Id, PopulationAddressTypeEnum.返乡来源地, body.SourceAddressInfo);
                    return result;
                }
            }
            else
            {
                var pop = await this.populationService.GetAsync(a => a.Id == body.PopulationId);
                if (pop == null)
                {
                    result.Message = "户籍人口Id无效";
                    return result;
                }

                if (pop.AreaId != body.AreaId) body.AreaId = pop.AreaId;

                var res = await this.InsertAsync(body);
                if (res != null)
                {
                    result.Message = "成功";
                    result.Flag = true;
                    result.Data = true;
                    await this.populationAddressService.SaveAddress(res.Id, PopulationAddressTypeEnum.返乡来源地, body.SourceAddressInfo);
                    return result;
                }
            }
            return result;

        }

        public async Task<MessageResult<bool>> DeleteEpidemicAsync(int id, int userid)
        {
            var result = new MessageResult<bool>("失败", false, false);
            var model = await this.GetAsync(a => a.Id == id);
            if (model == null)
            {
                result.Message = "数据不存在";
                return result;
            }

            model.IsDeleted = 1;
            model.IsSync = 0;
            model.UpdatedBy = userid;

            var ret = await this.UpdateAsync(model);
            if (ret > 0)
            {
                result.Message = "成功";
                result.Flag = true;
                result.Data = true;
            }
            return result;
        }

        /// <summary>
        /// 返乡统计
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<IPagedList<StatisticsEpidemicDto>> GetEpidemicStatisticsList(PagePostBody body)
        {

            string sqlWhere_areaid, basic_areaid;
            string sqlWhere_ve = string.Format(" and YEAR(reversalDate) ={0} ", body.Year);
            if (body.AreaId == 0)
            {
                //throw new ValidException("区域id不能为空");
            }
            var area = await this.basicAreaService.GetBasicAreaTree(body.AreaId);
            if (area.Ids.Count() == 1)
            {
                sqlWhere_areaid = string.Format(" and vp.areaId = {0}", body.AreaId);
                basic_areaid = string.Format(" and BasicArea.id = {0}", body.AreaId);
            }
            else
            {
                sqlWhere_areaid = string.Format(" and vp.areaId in ({0})", string.Join(",", area.Ids));
                basic_areaid = string.Format(" and BasicArea.id in ({0})", string.Join(",", area.Ids));
            }

            string sql_ext = "";
            for (int month = 1; month <= 12; month++)
            {
                sql_ext += gensql(month, "无", sqlWhere_ve + sqlWhere_areaid);
                sql_ext += gensql(month, "异常", sqlWhere_ve + sqlWhere_areaid);
            }
            string sql = string.Format(@" select name as areaName, a.* , 
                         (select count(vh.id) from VillageHouseholdCode vh where vh.isDeleted = 0 and vh.areaid = BasicArea.id) as household,
                         (select count(vp.id) from VillagePopulation vp where vp.isDeleted = 0 and vp.areaid = BasicArea.id) as population 
                    from BasicArea left join (
                        select areaId, 
                            sum(normal1) as normal1, sum(normal2) as normal2, sum(normal3) as normal3, sum(normal4) as normal4, sum(normal5) as normal5, sum(normal6) as normal6, sum(normal7) as normal7,
                            sum(normal8) as normal8, sum(normal9) as normal9, sum(normal10) as normal10,sum(normal11) as normal11,sum(normal12) as normal12,
                            sum(unnormal1) as unnormal1, sum(unnormal2) as unnormal2, sum(unnormal3) as unnormal3, sum(unnormal4) as unnormal4, sum(unnormal5) as unnormal5, sum(unnormal6) as unnormal6, 
                            sum(unnormal7) as unnormal7, sum(unnormal8) as unnormal8, sum(unnormal9) as unnormal9, sum(unnormal10) as unnormal10,sum(unnormal11) as unnormal11,sum(unnormal12) as unnormal12,
                              (select count(ve.populationId) from VillagePopulation vp, VillageEpidemic ve where vp.id = ve.populationId and vp.areaId = b.areaId {0} {1} and ve.health <> '6010100' and vp.isDeleted = 0 and ve.isDeleted = 0) as unnormaltotal,
                              (select count(ve.populationId) from VillagePopulation vp, VillageEpidemic ve where vp.id = ve.populationId and vp.areaId = b.areaId {0} {1} and ve.health <> '6010100' and date(ve.reversalDate)>=DATE_SUB(curdate(),INTERVAL 14 DAY) and vp.isDeleted = 0 and ve.isDeleted = 0) as unnormal14

                        from (
                            select id as areaId , 0 as unnormaltotal, 0 as unnormal14  ,0 as normal1 ,0 as unnormal1 ,0 as normal2 ,0 as unnormal2 ,0 as normal3 ,0 as unnormal3 ,0 as normal4 ,0 as unnormal4 ,0 as normal5 ,0 as unnormal5 ,0 as normal6 ,0 as unnormal6 ,0 as normal7 ,0 as unnormal7 ,0 as normal8 ,0 as unnormal8 ,0 as normal9 ,0 as unnormal9 ,0 as normal10 ,0 as unnormal10 ,0 as normal11 ,0 as unnormal11 ,0 as normal12 ,0 as unnormal12 
                            from VillageEpidemic ve 
                            where ve.id = -1
                            {2}
                         ) b group by areaId
                    ) a on a.areaId = BasicArea.id where BasicArea.isDeleted = 0 and BasicArea.level = 5 {3}", sqlWhere_areaid, sqlWhere_ve, sql_ext, basic_areaid);

            var pageData = this.Context.Database.SqlQueryPagedList<StatisticsEpidemicDto>(body.Page, body.Limit, sql, "", "");
            return pageData;
        }

        private string gensql(int month, string health = "无", string sqlwhere = "")
        {
            // 6010100 数据字典为 无
            if (health == "异常")
            {
                sqlwhere += " and ve.health <> '6010100' ";
            }
            else {
                sqlwhere += " and ve.health = '6010100' ";
            }
            string fields = ", 0 as unnormaltotal, 0 as unnormal14 ";
            for (int i = 1; i <= 12; i++)
            {
                if (i == month)
                {
                    if (health == "无")
                    {
                        fields += " ,count(ve.populationId) as normal" + i + " ,0 as unnormal" + i;
                    }
                    else
                    {
                        fields += " ,0 as normal" + i + " ,count(ve.populationId) as unnormal" + i;
                    }
                }
                else
                {
                    fields += " ,0 as normal" + i + " ,0 as unnormal" + i;
                }
            }
            string sql = string.Format(@" union all 
                select vp.areaId {0} 
                 from VillagePopulation vp,VillageEpidemic ve 
                 where vp.id = ve.populationId and MONTH(ve.reversalDate) = {1} {2} and vp.isDeleted = 0 and ve.isDeleted = 0 group by vp.areaId", fields, month, sqlwhere);
            return sql;
        }

        public async Task<byte[]> GetEpidemicStatisticsExcelData(PagePostBody body)
        {
            //PagePostBody body = new PagePostBody()
            //{
            //    AreaId = areaId,
            //    Year = year
            //};
            IPagedList<StatisticsEpidemicDto> list = await this.GetEpidemicStatisticsList(body);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();

            package.Workbook.Properties.Title = "返乡人员统计";
            var workSheet = package.Workbook.Worksheets.Add("返乡人员统计");
            // 表头
            workSheet.Cells[1, 1, 2, 1].Merge = true;
            workSheet.Cells[1, 1, 2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[1, 1, 2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 1].Value = "社区/村";

            workSheet.Cells[1, 2, 2, 2].Merge = true;
            workSheet.Cells[1, 2, 2, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[1, 2, 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 2].Value = "总户数";

            workSheet.Cells[1, 3, 2, 3].Merge = true;
            workSheet.Cells[1, 3, 2, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[1, 3, 2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 3].Value = "户籍人口";

            workSheet.Cells[1, 4, 1, 15].Merge = true;
            workSheet.Cells[1, 4, 1, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 4].Value = "返乡人数";

            workSheet.Cells[2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 4].Value = "1月";

            workSheet.Cells[2, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 5].Value = "2月";

            workSheet.Cells[2, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 6].Value = "3月";

            workSheet.Cells[2, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 7].Value = "4月";

            workSheet.Cells[2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 8].Value = "5月";

            workSheet.Cells[2, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 9].Value = "6月";

            workSheet.Cells[2, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 10].Value = "7月";

            workSheet.Cells[2, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 11].Value = "8月";

            workSheet.Cells[2, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 12].Value = "9月";

            workSheet.Cells[2, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 13].Value = "10月";

            workSheet.Cells[2, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 14].Value = "11月";

            workSheet.Cells[2, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 15].Value = "12月";

            workSheet.Cells[1, 16, 2, 16].Merge = true;
            workSheet.Cells[1, 16, 2, 16].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[1, 16, 2, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 16].Value = "累计异常人数";

            workSheet.Cells[1, 17, 2, 17].Merge = true;
            workSheet.Cells[1, 17, 2, 17].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            workSheet.Cells[1, 17, 2, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 17].Value = "14天内异常人数";

            List<StatisticsEpidemicDto> epidemics = list.ToList();
            for (int i = 0; i < epidemics.Count; i++)
            {
                int rowIndex = i + 3;
                StatisticsEpidemicDto epidemic = epidemics[i];

                workSheet.Cells[rowIndex, 1].Value = epidemic.AreaName;
                workSheet.Cells[rowIndex, 2].Value = epidemic.Household;
                workSheet.Cells[rowIndex, 3].Value = epidemic.Population;
                workSheet.Cells[rowIndex, 4].Value = epidemic.Normal1; // + epidemic.Unnormal1;
                workSheet.Cells[rowIndex, 5].Value = epidemic.Normal2; // + epidemic.Unnormal2;
                workSheet.Cells[rowIndex, 6].Value = epidemic.Normal3; // + epidemic.Unnormal3;
                workSheet.Cells[rowIndex, 7].Value = epidemic.Normal4; // + epidemic.Unnormal4;
                workSheet.Cells[rowIndex, 8].Value = epidemic.Normal5; // + epidemic.Unnormal5;
                workSheet.Cells[rowIndex, 9].Value = epidemic.Normal6; // + epidemic.Unnormal6;
                workSheet.Cells[rowIndex, 10].Value = epidemic.Normal7; // + epidemic.Unnormal7;
                workSheet.Cells[rowIndex, 11].Value = epidemic.Normal8; // + epidemic.Unnormal8;
                workSheet.Cells[rowIndex, 12].Value = epidemic.Normal9; // + epidemic.Unnormal9;
                workSheet.Cells[rowIndex, 13].Value = epidemic.Normal10; // + epidemic.Unnormal10;
                workSheet.Cells[rowIndex, 14].Value = epidemic.Normal11; // + epidemic.Unnormal11;
                workSheet.Cells[rowIndex, 15].Value = epidemic.Normal12; // + epidemic.Unnormal12;
                workSheet.Cells[rowIndex, 16].Value = epidemic.Unnormaltotal;
                workSheet.Cells[rowIndex, 17].Value = epidemic.Unnormal14;
            }
            return await package.GetAsByteArrayAsync();
        }

        public async Task<byte[]> GetEpidemicExcelData(int areaId, int year, string ids)
        {
            PagePostBody body = new PagePostBody()
            {
                AreaId = areaId,
                Year = year,
                Ids = ids,
                Page = 1,
                Limit = 10000,
            };
            IPagedList<VillageEpidemicDto> list = await this.GetEpidemicList(body);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();

            package.Workbook.Properties.Title = "返乡人员登记";
            var workSheet = package.Workbook.Worksheets.Add("返乡人员登记");
            // 表头
            workSheet.Cells[1, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 1].Value = "序号";
            workSheet.Cells[1, 2].Value = "门牌名";
            workSheet.Cells[1, 3].Value = "门牌号";
            workSheet.Cells[1, 4].Value = "户主姓名";
            workSheet.Cells[1, 5].Value = "联系方式";
            workSheet.Cells[1, 6].Value = "返乡人员数量";

            List<VillageEpidemicDto> epidemics = list.ToList();
            for (int i = 0; i < epidemics.Count; i++)
            {
                int rowIndex = i + 2;
                VillageEpidemicDto epidemic = epidemics[i];

                workSheet.Cells[rowIndex, 1].Value = i + 1;
                workSheet.Cells[rowIndex, 2].Value = epidemic.HouseName;
                workSheet.Cells[rowIndex, 3].Value = epidemic.HouseNumber;
                workSheet.Cells[rowIndex, 4].Value = epidemic.HouseholdMan;
                workSheet.Cells[rowIndex, 5].Value = epidemic.Mobile;
                workSheet.Cells[rowIndex, 6].Value = epidemic.PeopleCount;
            }
            return await package.GetAsByteArrayAsync();
        }
    }
}
