using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Common.Infrastructures;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Common.SO;
using DVS.Core.Domains.Village;
using DVS.Model.Dtos.Village;
using DVS.Models.Dtos.Village;
using DVS.Models.Dtos.Village.Query;
using DVS.Models.Dtos.Village.Vaccination;
using DVS.Models.Enum;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.Village
{
    public class VaccinationService : ServiceBase<VillageVaccination>, IVaccinationService
    {

        private readonly IPopulationService populationService;
        private readonly IBasicDictionaryService basicDictionaryService;
        private readonly IHouseholdCodeService householdCodeService;
        private readonly IBasicAreaService basicAreaService;
        private readonly ISunFileInfoService sunFileService;
        private readonly IPopulationTagService populationTagService;
        private readonly IServiceBase<VillageHouseCodeMember> memberService;
        private readonly IBasicUserService basicUserService;
        private readonly IPopulationAddressService populationAddressService;
        public VaccinationService(DbContext dbContext, IMapper mapper, IPopulationService populationService, IBasicDictionaryService basicDictionaryService, IHouseholdCodeService householdCodeService, IBasicAreaService basicAreaService, ISunFileInfoService sunFileService, IPopulationTagService populationTagService, IServiceBase<VillageHouseCodeMember> memberService, IBasicUserService basicUserService, IPopulationAddressService populationAddressService
            )
            : base(dbContext, mapper)
        {
            this.populationService = populationService;
            this.basicDictionaryService = basicDictionaryService;
            this.householdCodeService = householdCodeService;
            this.basicAreaService = basicAreaService;
            this.sunFileService = sunFileService;
            this.populationTagService = populationTagService;
            this.memberService = memberService;
            this.basicUserService = basicUserService;
            this.populationAddressService = populationAddressService;
        }

        /// <summary>
        /// 疫苗接种查询（按户码统计）
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<IPagedList<VaccinationHouseholdDto>> GetVaccinationList(PagePostBody body)
        {
            string sqlWhere = "";
            if (!string.IsNullOrWhiteSpace(body.Keyword))
            {
                sqlWhere = string.Format(" and (p.realName like '%{0}%' or h.houseName  LIKE '%{0}%'  or h.houseNumber  LIKE '%{0}%' ) ",body.Keyword);
            }

            if (!string.IsNullOrWhiteSpace(body.Ids))
            {
                sqlWhere += " and h.id in (" + body.Ids + ")";
            }

            if (body.Year == 0)
            {
                body.Year = DateTime.Now.Year;
            }
            string sql = string.Format(@" 
                SELECT t.*,
                    (select count(vp.id) from VillagePopulation vp,VillageHouseCodeMember vc where vc.populationId = vp.id and vc.householdId = t.id and vc.isDeleted=0 and vp.isDeleted=0) as peopleCount ,
                    (select count(vv.id) from VillageVaccination vv, VillageHouseCodeMember vc where vv.populationId = vc.populationId and vc.householdId = t.id and numberStitch=1 and vv.isDeleted=0 and vc.isDeleted=0 and year = {2}) as firstCount ,
                    (select count(vv.id) from VillageVaccination vv, VillageHouseCodeMember vc where vv.populationId = vc.populationId and vc.householdId = t.id  and numberStitch=2 and vv.isDeleted=0 and vc.isDeleted=0 and year = {2}) as secondCount 
                    from 
                    (
					    SELECT 
						    h.id,
						    h.areaId,
						    h.houseName,
						    h.houseNumber,
                            p.realName as householdMan,
						    IFNULL(p.sex,0)  as sex,
                            p.mobile,
                            p.nation,
                            p.populationId
					    from VillageHouseholdCode h
                        LEFT JOIN  (SELECT v.realName,v.sex,v.mobile,v.nation, m.householdId,m.populationId FROM VillagePopulation v, VillageHouseCodeMember m WHERE v.id = m.populationId and  
                          v.areaId={0} and m.isHouseholder=1 and v.isDeleted=0 and m.isDeleted=0 )p  on h.id= p.householdId
                        where h.isDeleted=0 and h.isDeleted=0 and h.areaId={0} {1})t ",
                body.AreaId, sqlWhere, body.Year);


            var orderby = " order by convert(houseName using gbk) asc, convert(houseName using gbk) asc ";
            if (body.Orders != null)
            {
                var orders = new List<string>();
                foreach (var order in body.Orders)
                {
                    orders.Add("convert(" + order.FieldName + " using gbk) " + order.Sort);
                }
                orderby = " order by " + string.Join(",", orders);
            }
            var pageData = this.Context.Database.SqlQueryPagedList<VaccinationHouseholdDto>(body.Page, body.Limit, sql, "", orderby);

            List<string> nations = pageData.Select(a => a.Nation).Distinct().ToList();
            List<int> idList = new List<int>();
            foreach (var code in nations)
            {
                int _code;
                if (code != null && int.TryParse(code, out _code))
                {
                    idList.Add(_code);
                }
            }
            var dirNames = await this.basicDictionaryService.GetBasicDictionaryList(idList);
            foreach (var item in pageData)
            {
                int nation = 0;
                _ = int.TryParse(item.Nation, out nation);

                var dictionary = dirNames.FirstOrDefault(a => a.Code == nation);
                item.Nation = dictionary != null ? dictionary.Name : item.Nation;
                item.HouseholdMan = BasicSO.Decrypt(item.HouseholdMan);
                item.Mobile = BasicSO.Decrypt(item.Mobile);
                item.Year = body.Year;
            }

            return pageData;
        }

        /// <summary>
        /// 疫苗接种记录(按户码查询)
        /// </summary>
        /// <param name="householdId"></param>
        /// <param name="year"></param>
        /// <param name="populationId"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public async Task<IPagedList<VaccinationDto>> GetVaccinationInfoList(int householdId, int year, int populationId = 0, int page = 1, int limit = 10)
        {
            var sql = string.Format("select vv.*, vp.sex,vp.areaId,vp.realName,vc.householdId from VillageVaccination vv, VillagePopulation vp, VillageHouseCodeMember vc                            where vv.populationId = vp.id and vp.id = vc.populationId and vc.householdId = {0} and vp.isDeleted = 0 and vc.isDeleted = 0 and vv.isDeleted = 0 and vv.year = {1}", householdId, year);
            string sqlCount = string.Format("select vv.id from VillageVaccination vv, VillagePopulation vp, VillageHouseCodeMember vc where vv.populationId = vp.id and vp.id = vc.populationId and vc.householdId = {0} and vp.isDeleted = 0 and vc.isDeleted = 0 and vv.isDeleted = 0 and vv.year = {1}", householdId, year);
            if (populationId > 0) {
                sql += " and vv.populationId = " + populationId;
                sqlCount += " and vv.populationId = " + populationId;
            }

            var datas = this.Context.Database.SqlQueryPagedList<VaccinationDto>(page, limit, sql, sqlCount, " order by id desc");
            List<int> userIds = datas.Select(a => a.CreatedBy).Union(datas.Select(a => a.UpdatedBy)).Distinct().ToList();
            var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));
            foreach (var item in datas)
            {
                item.RealName = BasicSO.Decrypt(item.RealName);
                var user = userList.FirstOrDefault(a => a.Id == item.CreatedBy);
                item.CreatedByName = user != null ? user.NickName : "";
                user = userList.FirstOrDefault(a => a.Id == item.UpdatedBy);
                item.UpdatedByName = user != null ? user.NickName : "";
                item.AddressInfo = await this.populationAddressService.GetAddressDetail(item.Id, PopulationAddressTypeEnum.接种地);
            }
            return datas;
        }

        public async Task<MessageResult<bool>> SaveVaccinationInfo(VillageVaccination body)
        {
            var result = new MessageResult<bool>("失败", false, false);
            if (body.PopulationId <= 0)
            {
                result.Message = "请输入人口Id";
                return result;
            }

            if (body.IsVaccination == 0 && string.IsNullOrWhiteSpace(body.NotReason))
            {
                result.Message = "未接种原因不能为空";
                return result;
            }

            if (body.IsVaccination == 1 && body.VaccinationDatetime == null)
            {
                result.Message = "接种日期不能为空";
                return result;
            }

            if (body.IsVaccination == 0)
            {
                body.Year = DateTime.Now.Year;
                body.VaccinationDatetime = DateTime.Now;
            }
            else {
                body.Year = body.VaccinationDatetime.Year;
            }

            Expression<Func<VillageVaccination, bool>> expression = a => a.PopulationId == body.PopulationId && a.IsDeleted == 0;
            if (body.Id > 0)
            {
                expression = expression.And(a => a.Id != body.Id);
            }

            if (body.IsVaccination == 0)
            {
                // 未接种记录不需要重复登记
                expression = expression.And(a => a.IsVaccination == 0);
                var model = await this.GetAsync(expression);
                if (model != null)
                {
                    result.Message = "不能重复登记";
                    return result;
                }
            }
            else
            {
                expression = expression.And(a => a.NumberStitch == body.NumberStitch && a.IsVaccination == 1);
                var model = await this.GetAsync(expression);
                if (model != null)
                {
                    result.Message = "接种针剂次数重复";
                    return result;
                }
            }

            if (body.Id > 0)
            {
                var res = await this.GetQueryable().Where(a => a.Id == body.Id)
                    .UpdateFromQueryAsync(a => new VillageVaccination()
                    {
                        PopulationId = body.PopulationId,
                        VaccinationDatetime = body.VaccinationDatetime,
                        IsVaccination = body.IsVaccination,
                        NumberStitch = body.NumberStitch,
                        AddressType = body.AddressType,
                        Address = body.Address,
                        Remark = body.Remark,
                        NotReason = body.NotReason,
                        UpdatedBy = body.UpdatedBy,
                    });
                if (res > 0)
                {
                    await this.populationAddressService.SaveAddress(body.Id, PopulationAddressTypeEnum.接种地, body.AddressInfo);
                    result.Message = "成功";
                    result.Flag = true;
                    result.Data = true;
                    return result;
                }
            }
            else
            {
                var res = await this.InsertAsync(body);
                if (res != null)
                {
                    await this.populationAddressService.SaveAddress(res.Id, PopulationAddressTypeEnum.接种地, body.AddressInfo);
                    result.Message = "成功";
                    result.Flag = true;
                    result.Data = true;
                    return result;
                }
            }

            return result;

        }

        public async Task<MessageResult<bool>> DeleteVaccinationInfo(int id, int userid)
        {
            var result = new MessageResult<bool>("失败", false, false);
            var model = await this.GetAsync(a => a.Id == id);
            if (model == null)
            {
                result.Message = "数据不存在";
                return result;
            }

            model.IsDeleted = 1;
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

        public async Task<VaccinationDto> DetailVaccinationInfo(int id)
        {
            var model = await this.GetAsync(a => a.Id == id);
            if (model == null)
            {
                throw new ValidException("数据不存在");
            }
            var detail = mapper.Map<VaccinationDto>(model);
            var members = await this.memberService.GetListAsync(a => a.PopulationId == detail.PopulationId && a.IsDeleted == 0);
            if (members != null && members.Count() > 0)
            {
                detail.IsHouseholder = members.FirstOrDefault().IsHouseholder;
                detail.HouseholdId = members.FirstOrDefault().HouseholdId;
                var household = await this.householdCodeService.GetAsync(a => a.Id == detail.HouseholdId);
                if (household != null)
                {
                    detail.HouseName = household.HouseName;
                    detail.HouseNumber = household.HouseNumber;
                }
            }
            var pop = await this.populationService.GetAsync(a => a.Id == detail.PopulationId);
            if (pop != null)
            {
                detail.RealName = BasicSO.Decrypt(pop.RealName);
                detail.HeadImageUrl = this.sunFileService.ToAbsolutePath(pop.HeadImageUrl);
                _ = int.TryParse(pop.Relationship, out int relationshipId);
                var dic = await this.basicDictionaryService.GetAsync(a => a.Code == relationshipId && a.IsDeleted == 0);
                detail.Relationship = dic != null ? dic.Name : "";
                detail.Mobile = BasicSO.Decrypt(pop.Mobile);
                detail.Sex = (int)pop.Sex;
                detail.TagNames = await this.populationTagService.GetTags(pop.Id);

                List<int> userIds = new List<int>();
                userIds.Add(detail.CreatedBy);
                userIds.Add(detail.UpdatedBy);
                var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));
                var user = userList.FirstOrDefault(a => a.Id == pop.CreatedBy);
                detail.CreatedByName = user != null ? user.NickName : "";
                user = userList.FirstOrDefault(a => a.Id == pop.UpdatedBy);
                detail.UpdatedByName = user != null ? user.NickName : "";
                detail.AddressInfo = await this.populationAddressService.GetAddressDetail(detail.Id, PopulationAddressTypeEnum.接种地);
            }
            return detail;
        }

        public async Task<List<VaccinationDto>> VaccinationInfoList(int populationId, int householdId)
        {
            Expression<Func<VillageVaccination, bool>> expression = a => a.PopulationId == populationId && a.IsDeleted == 0 && a.IsVaccination == 1;
            var list = this.GetQueryable().Where(expression).OrderByDescending(a => a.NumberStitch);
            if (list == null || list.Count() == 0)
            {
                return new List<VaccinationDto>();
            }
            var details = mapper.Map<List<VaccinationDto>>(list);
            var household = await this.householdCodeService.GetAsync(a => a.Id == householdId);
            var member = await this.memberService.GetAsync(a => a.HouseholdId == householdId && a.PopulationId == populationId && a.IsDeleted == 0);
            List<int> userIds = details.Select(a => a.CreatedBy).Union(details.Select(a => a.UpdatedBy)).Distinct().ToList();
            var userList = await this.basicUserService.GetListAsync(a => userIds.Contains(a.Id));
            var pop = await this.populationService.GetAsync(a => a.Id == populationId);
            var tagname = await this.populationTagService.GetTags(pop.Id);
            var imageurl = this.sunFileService.ToAbsolutePath(pop.HeadImageUrl);
            var realname = BasicSO.Decrypt(pop.RealName);
            var mobile = BasicSO.Decrypt(pop.Mobile);
            _ = int.TryParse(pop.Relationship, out int relationshipId);
            var dic = await this.basicDictionaryService.GetAsync(a => a.Code == relationshipId && a.IsDeleted == 0);
            foreach (var detail in details)
            {
                detail.IsHouseholder = member == null ? 0 : member.IsHouseholder;
                detail.HouseName = household != null ? household.HouseName : "";
                detail.HouseNumber = household != null ? household.HouseNumber : "";
                detail.RealName = realname;
                detail.HeadImageUrl = imageurl;
                detail.Relationship = dic != null ? dic.Name : "";
                detail.Mobile = mobile;
                detail.Sex = (int)pop.Sex;
                detail.TagNames = tagname;

                var user = userList.FirstOrDefault(a => a.Id == detail.CreatedBy);
                detail.CreatedByName = user != null ? user.NickName : "";
                user = userList.FirstOrDefault(a => a.Id == detail.UpdatedBy);
                detail.UpdatedByName = user != null ? user.NickName : "";
                detail.AddressInfo = await this.populationAddressService.GetAddressDetail(detail.Id, PopulationAddressTypeEnum.接种地);
            }
            return details;
        }

        /// <summary>
        /// 疫苗接种台账查询,app
        /// 1 需求接种户籍，2 已接种，3 接种第一针，4 接种第一针本地 ，5 接种第一针异地，6 接种第二针，7 接种第二针本地，8 接种第二针异地，9 未登记，10 未接种
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<IPagedList<VaccinationDto>> GetVaccinationInfoList(VaccinationPagePostBody body)
        {
            string sql = "";
            string searchtype = body.SearchType;
            string sqlWhere_vp = "";
            string sqlWhere_vv = "";
            string orderby = " order by createdAt desc";
            string sqlWhere_vv_not = "";
            if (!string.IsNullOrWhiteSpace(body.Keyword))
            {
                sqlWhere_vp = string.Format(" and (vp.realName like '%{0}%' or vp.mobile = '{1}' or vp.mobileShort like '%{0}%') ", body.Keyword, BasicSO.Encrypt(body.Keyword));
            }

            string beginDate = body.BeginDate == null ? null : body.BeginDate.Value.ToString("yyyy-MM-dd");
            string endDate = body.EndDate == null ? null : body.EndDate.Value.ToString("yyyy-MM-dd");
            if (body.BeginDate != null)
            {
                sqlWhere_vv += string.Format(" and date(vv.vaccinationDatetime)>= date('{0}')", beginDate);
                sqlWhere_vv_not += string.Format(" and date(vv.createdAt)>= date('{0}')", beginDate);
            }
            if (body.EndDate != null)
            {
                sqlWhere_vv += string.Format(" and date(vv.vaccinationDatetime)<= date('{0}')", endDate);
                sqlWhere_vv_not += string.Format(" and date(vv.createdAt)<= date('{0}')", endDate);
            }

            if (!string.IsNullOrWhiteSpace(body.NotReason))
            {
                if (body.NotReason == "低龄")
                {
                    sqlWhere_vv_not += string.Format(" and (notReason = '低龄' or notReason = '未成年')");
                }
                else if (body.NotReason == "哺乳")
                {
                    sqlWhere_vv_not += string.Format(" and (notReason = '哺乳' or notReason = '哺乳期')");
                }
                else
                {
                    sqlWhere_vv_not += string.Format(" and notReason = '{0}'", body.NotReason);
                }
            }

            // 1 需求接种户籍，2 已接种，3 接种第一针，4 接种第一针本地 ，5 接种第一针异地，6 接种第二针，7 接种第二针本地，8 接种第二针异地，9 未登记，10 未接种
            if (searchtype == "9")
            {
                sql = string.Format(@" SELECT vp.id as populationId, vp.areaId, vp.realName, vp.sex, vp.mobile, vp.headImageUrl,vp.relationship,vp.createdAt
                              from VillagePopulation vp
                             where vp.isDeleted=0 and vp.areaId={0} {1} and 
                                   vp.id not in (select populationId from VillageVaccination vv where vv.isDeleted=0 {2}) ",
                body.AreaId, sqlWhere_vp, sqlWhere_vv);
            }
            else
            {
                if (searchtype == "1")
                {
                    sql = string.Format(@" 
                            SELECT vp.areaId, vp.realName, vp.sex, vp.mobile, vp.id as populationId ,vp.headImageUrl,vp.relationship,vp.createdAt
                              from VillagePopulation vp
                             where vp.isDeleted=0 and vp.areaId={0} {1} ",
                    body.AreaId, sqlWhere_vp);
                }
                else if (searchtype == "2")
                {
                    sql = string.Format(@" 
                        SELECT vv.id, vp.areaId, vp.realName, vp.sex, vp.mobile, vp.headImageUrl,vp.relationship,vv.isVaccination, vv.vaccinationDatetime, vv.numberStitch, vv.addressType, vv.notReason, vv.populationId,vv.createdAt
                        from VillagePopulation vp,(select id, populationId, max(numberStitch) as numberStitch, isVaccination, vaccinationDatetime, addressType, notReason, createdAt from VillageVaccination vv where isVaccination = 1 and isDeleted = 0 {2} group by populationId) vv 
                        where vp.id = vv.populationId and vp.isDeleted = 0 and vp.areaId = {0} {1}",
                    body.AreaId, sqlWhere_vp, sqlWhere_vv);
                    orderby = " order by vaccinationDatetime asc";
                }
                else
                {
                    switch (searchtype)
                    {
                        case "2":
                            sqlWhere_vv += " and vv.isVaccination = 1";
                            break;
                        case "3":
                            sqlWhere_vv += " and vv.isVaccination = 1 and vv.numberStitch = 1 ";
                            break;
                        case "4":
                            sqlWhere_vv += " and vv.isVaccination = 1 and vv.numberStitch = 1 and addressType = 1 ";
                            break;
                        case "5":
                            sqlWhere_vv += " and vv.isVaccination = 1 and vv.numberStitch = 1 and addressType = 2 ";
                            break;
                        case "6":
                            sqlWhere_vv += " and vv.isVaccination = 1 and vv.numberStitch = 2 ";
                            break;
                        case "7":
                            sqlWhere_vv += " and vv.isVaccination = 1 and vv.numberStitch = 2 and addressType = 1 ";
                            break;
                        case "8":
                            sqlWhere_vv += " and vv.isVaccination = 1 and vv.numberStitch = 2 and addressType = 2 ";
                            break;
                        case "10":
                            sqlWhere_vv_not += " and vv.isVaccination = 0 ";
                            sqlWhere_vv = sqlWhere_vv_not;
                            break;
                        default:
                            break;
                    }
                    sql = string.Format(@" 
                        SELECT vv.id, vp.areaId, vp.realName, vp.sex, vp.mobile, vp.headImageUrl,vp.relationship,vv.isVaccination, vv.vaccinationDatetime, vv.numberStitch, vv.addressType, vv.notReason, vv.populationId,vv.createdAt
                         from VillagePopulation vp ,VillageVaccination vv
                        where vp.id = vv.populationId and vp.isDeleted = 0 and vv.isDeleted = 0 and vp.areaId = {0} {1} {2}",
                    body.AreaId, sqlWhere_vp, sqlWhere_vv);
                    if (searchtype == "10")
                    {
                        orderby = " order by createdAt desc";
                    }
                    else {
                        orderby = " order by vaccinationDatetime desc";
                    }
                }
            }

            var pageData = this.Context.Database.SqlQueryPagedList<VaccinationDto>(body.Page, body.Limit, sql, "", orderby);
            if (pageData == null || pageData.Count() == 0)
            {
                return pageData;
            }
            List<VillageVaccination> vaccinations = null;
            var ids = pageData.Select(a => a.PopulationId).ToList();
            if (searchtype == "1")
            {
                // 查询该页户籍人口的接种记录
                Expression<Func<VillageVaccination, bool>> expression = a => a.IsDeleted == 0 && ids.Contains(a.PopulationId);
                if (body.BeginDate != null)
                {
                    expression = expression.And(a => a.VaccinationDatetime.Date >= body.BeginDate);
                }
                if (body.EndDate != null)
                {
                    expression = expression.And(a => a.VaccinationDatetime.Date <= body.EndDate);
                }
                vaccinations = this.GetQueryable().Where(expression).OrderBy(a => a.PopulationId).OrderByDescending(a => a.NumberStitch).ToList();
            }
            sql = string.Format(@" select distinct vc.householdId,vc.populationId,vc.isHouseholder,vh.houseName, vh.houseNumber,vp.realName 
                    from VillageHouseCodeMember vc,VillageHouseholdCode vh ,VillagePopulation vp, 
                        (select distinct householdId from VillageHouseCodeMember where populationId in ({0})) vm 
                    where vc.populationId = vp.id and vc.householdId = vh.id and vc.householdId = vm.householdId and 
                    vc.isDeleted = 0 and vp.isDeleted = 0 and vh.isDeleted = 0 order by populationId", string.Join(",", ids));
            var list = this.Context.Database.SqlQuery<HouseholderAndHouseNumberDto>(sql).ToList();

            var relationids = pageData.Where(a => a.Relationship != "").Select(a => a.Relationship).Distinct().ToList();
            var dics = await this.basicDictionaryService.GetBasicDictionaryCodeList(relationids);
            foreach (var item in pageData)
            {
                item.RealName = BasicSO.Decrypt(item.RealName);
                item.Mobile = BasicSO.Decrypt(item.Mobile);
                item.HeadImageUrl = this.sunFileService.ToAbsolutePath(item.HeadImageUrl);
                var tagname = await this.populationTagService.GetTags(item.PopulationId);
                item.TagNames = tagname;
                if (item.Relationship != "")
                {
                    int _code;
                    int.TryParse(item.Relationship, out _code);
                    var dic = dics.FirstOrDefault(a => a.Code == _code);
                    item.Relationship = dic != null ? dic.Name : "";
                }
                // 判断户籍人口是否设置户码(HouseholdList=null)，属于一户(HouseholdList.count()=1，属于多户(HouseholdList.count()>1)
                item.HouseholdList = list.FindAll(a => a.PopulationId == item.PopulationId);
                if (searchtype == "1")
                {
                    var vaccination = vaccinations.FirstOrDefault(a => a.PopulationId == item.PopulationId);
                    item.IsVaccination = vaccination != null ? vaccination.IsVaccination : 0;
                    item.NumberStitch = vaccination != null ? vaccination.NumberStitch : 0;
                    item.VaccinationDatetime = vaccination != null ? vaccination.VaccinationDatetime : DateTime.Parse("1970-01-01 00:00:01");
                    item.NotReason = vaccination != null ? vaccination.NotReason : "";
                    item.Id = vaccination != null ? vaccination.Id : 0;
                }

                if (item.HouseholdList != null && item.HouseholdList.Count() == 1)
                {
                    item.HouseName = item.HouseholdList[0].HouseName;
                    item.HouseNumber = item.HouseholdList[0].HouseNumber;
                    item.HouseholdId = item.HouseholdList[0].HouseholdId;
                }

            }
            return pageData;
        }

        /// <summary>
        /// 疫苗接种记录(按户查询),app
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<IPagedList<VaccinationDto>> GetVaccinationHouseholdList(VaccinationPagePostBody body)
        {
            string sql = "";
            sql = string.Format(@" SELECT vp.areaId, vp.realName, vp.sex, vp.mobile, vp.id as populationId ,vp.relationship,vp.headImageUrl, 
                                vc.isHouseholder,vc.householdId,vh.houseName,vh.houseNumber
                              from VillageHouseCodeMember vc,VillageHouseholdCode vh ,VillagePopulation vp
                             where vc.householdId = vh.id and vp.id = vc.populationId and vh.id={0} and vc.isDeleted=0 and vp.isDeleted=0", body.HouseholdId);

            var pageData = this.Context.Database.SqlQueryPagedList<VaccinationDto>(body.Page, body.Limit, sql, "", " order by isHouseholder desc");
            if (pageData == null || pageData.Count() == 0)
            {
                return pageData;
            }
            var ids = pageData.Select(a => a.PopulationId).ToList();
            var relationships = pageData.Where(a=>a.Relationship !="").Select(a => a.Relationship).Distinct().ToList();
            Expression<Func<VillageVaccination, bool>> expression = a => a.IsDeleted == 0 && ids.Contains(a.PopulationId);
            List<VillageVaccination> vaccinations = this.GetQueryable().Where(expression).OrderBy(a => a.PopulationId).OrderByDescending(a => a.NumberStitch).ToList();
            var tags = await this.populationTagService.GetTags(ids);
            var dictionaries = await this.basicDictionaryService.GetBasicDictionaryCodeList(relationships);
            foreach (var item in pageData)
            {
                item.RealName = BasicSO.Decrypt(item.RealName);
                item.Mobile = BasicSO.Decrypt(item.Mobile);
                if (!string.IsNullOrWhiteSpace(item.Relationship))
                {
                    int _code;
                    int.TryParse(item.Relationship, out _code);
                    var dictionary = dictionaries.ToList().FirstOrDefault(a => a.Code == _code);
                    item.Relationship = dictionary != null ? dictionary.Name : "";
                }
                var vaccination = vaccinations.FirstOrDefault(a => a.PopulationId == item.PopulationId);
                item.IsVaccination = vaccination != null ? vaccination.IsVaccination : 0;
                item.NumberStitch = vaccination != null ? vaccination.NumberStitch : 0;
                item.VaccinationDatetime = vaccination != null ? vaccination.VaccinationDatetime : DateTime.Parse("1970-01-01 00:00:01");
                item.NotReason = vaccination != null ? vaccination.NotReason : "";
                item.Address = vaccination != null ? vaccination.Address : "";
                item.AddressType = vaccination != null ? vaccination.AddressType : 0;
                item.Remark = vaccination != null ? vaccination.Remark : "";
                item.HeadImageUrl = this.sunFileService.ToAbsolutePath(item.HeadImageUrl);
                item.Id = vaccination != null ? vaccination.Id : 0;
                var tagname = tags.ToList().FindAll(a => a.Pid == item.PopulationId);
                //var tagname = await this.populationTagService.GetTags(item.PopulationId);
                item.TagNames = tagname;
            }
            return pageData;
        }

        /// <summary>
        /// 疫苗接种统计
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<IPagedList<StatisticsVaccinationDto>> GetVaccinationStatisticsList(VaccinationPagePostBody body)
        {

            string sqlWhere_areaid, basic_areaid;
            string sqlWhere_end;
            string sqlWhere_vv = "";
            string sqlWhere_vv_not = "";
            string beginDate = body.BeginDate == null ? null : body.BeginDate.Value.ToString("yyyy-MM-dd");
            string endDate = body.EndDate == null ? null : body.EndDate.Value.ToString("yyyy-MM-dd");
            if (beginDate != null)
            {
                sqlWhere_vv += string.Format(" and date(vv.vaccinationDatetime)>= date('{0}')", beginDate);
                sqlWhere_vv_not += string.Format(" and date(vv.createdAt)>= date('{0}')", beginDate);
            }
            if (body.EndDate != null)
            {
                sqlWhere_vv += string.Format(" and date(vv.vaccinationDatetime)<= date('{0}')", endDate);
                sqlWhere_vv_not += string.Format(" and date(vv.createdAt)<= date('{0}')", endDate);
                sqlWhere_end = string.Format(" and date(vv.vaccinationDatetime)<= date('{0}')", endDate);
            }
            else
            {
                sqlWhere_vv += string.Format(" and date(vv.vaccinationDatetime)<= DATE_SUB(curdate(),INTERVAL 1 DAY)");
                sqlWhere_vv_not += string.Format(" and date(vv.createdAt)<= DATE_SUB(curdate(),INTERVAL 1 DAY)");
                sqlWhere_end = string.Format(" and date(vv.vaccinationDatetime)<= DATE_SUB(curdate(),INTERVAL 1 DAY)");
            }
            if (body.AreaId == 0)
            {
                throw new ValidException("区域id不能为空");
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
            string sql = string.Format(@" select name as areaName, a.*, 
                         (select count(vh.id) from VillageHouseholdCode vh where vh.isDeleted = 0 and vh.areaid = BasicArea.id) as household,
                         (select count(vp.id) from VillagePopulation vp where vp.isDeleted = 0 and vp.areaid = BasicArea.id) as population 
                    from BasicArea left join (
                    select areaId,sum(vaccinated) as vaccinated, sum(vaccinated_first) as vaccinated_first, sum(vaccinated_first_local) as vaccinated_first_local, 
                        sum(vaccinated_first_nonlocal) as vaccinated_first_nonlocal, 
                        sum(vaccinated_second) as vaccinated_second, sum(vaccinated_second_local) as vaccinated_second_local,sum(vaccinated_second_nonlocal) as vaccinated_second_nonlocal,
                        sum(notvaccinated) as notvaccinated, 
                        sum(old) as old, sum(children) as children, sum(pregnant) as pregnant, sum(lactation) as lactation, sum(sick) as sick, sum(outwork) as outwork, sum(missing) as missing, 
                        sum(other) as other, sum(notRegister) as notRegister , sum(vaccinated_third) as vaccinated_third, sum(vaccinated_third_local) as vaccinated_third_local,
                        sum(vaccinated_third_nonlocal) as vaccinated_third_nonlocal
                    from (
                        select vp.areaId,count(distinct vv.populationId) as vaccinated, 0 as vaccinated_first, 0 as vaccinated_first_local, 0 as vaccinated_first_nonlocal, 
                        0 as vaccinated_second, 0 as vaccinated_second_local,0 as vaccinated_second_nonlocal, 0 as notvaccinated, 
                        0 as old, 0 as children, 0 as pregnant, 0 as lactation, 0 as sick, 0 as outwork, 0 as missing, 0 as other, 0 as notRegister, 
                        0 as vaccinated_third, 0 as vaccinated_third_local,0 as vaccinated_third_nonlocal
                        from VillagePopulation vp,VillageVaccination vv
                        where vp.id = vv.populationId and vv.isVaccination = 1 and vp.isDeleted = 0 and vv.isDeleted = 0 {0} {1}
                        group by vp.areaId
                        union all
                        select vp.areaId,0 as vaccinated, count(vv.id) as vaccinated_first, 0 as vaccinated_first_local, 0 as vaccinated_first_nonlocal, 
                        0 as vaccinated_second, 0 as vaccinated_second_local,0 as vaccinated_second_nonlocal, 0 as notvaccinated, 
                        0 as old, 0 as children, 0 as pregnant, 0 as lactation, 0 as sick, 0 as outwork, 0 as missing, 0 as other, 0 as notRegister, 
                        0 as vaccinated_third, 0 as vaccinated_third_local,0 as vaccinated_third_nonlocal
                        from VillagePopulation vp,VillageVaccination vv
                        where vp.id = vv.populationId and vv.isVaccination = 1 and vv.numberStitch = 1 and vp.isDeleted = 0 and vv.isDeleted = 0 {0} {1}
                        group by vp.areaId
                        union all
                        select vp.areaId,0 as vaccinated, 0 as vaccinated_first, count(vv.id) as vaccinated_first_local, 0 as vaccinated_first_nonlocal, 
                        0 as vaccinated_second, 0 as vaccinated_second_local,0 as vaccinated_second_nonlocal, 0 as notvaccinated, 
                        0 as old, 0 as children, 0 as pregnant, 0 as lactation, 0 as sick, 0 as outwork, 0 as missing, 0 as other, 0 as notRegister, 
                        0 as vaccinated_third, 0 as vaccinated_third_local,0 as vaccinated_third_nonlocal
                        from VillagePopulation vp,VillageVaccination vv
                        where vp.id = vv.populationId and vv.isVaccination = 1 and vv.numberStitch = 1 and addressType = 1 and vp.isDeleted = 0 and vv.isDeleted = 0 {0} {1}
                        group by vp.areaId
                        union all
                        select vp.areaId,0 as vaccinated, 0 as vaccinated_first, 0 as vaccinated_first_local, count(vv.id) as vaccinated_first_nonlocal, 
                        0 as vaccinated_second, 0 as vaccinated_second_local,0 as vaccinated_second_nonlocal, 0 as notvaccinated, 
                        0 as old, 0 as children, 0 as pregnant, 0 as lactation, 0 as sick, 0 as outwork, 0 as missing, 0 as other, 0 as notRegister, 
                        0 as vaccinated_third, 0 as vaccinated_third_local,0 as vaccinated_third_nonlocal
                        from VillagePopulation vp,VillageVaccination vv
                        where vp.id = vv.populationId and vv.isVaccination = 1  and vv.numberStitch = 1 and addressType = 2 and vp.isDeleted = 0 and vv.isDeleted = 0 {0} {1}
                        group by vp.areaId
                        union all
                        select vp.areaId,0 as vaccinated, 0 as vaccinated_first, 0 as vaccinated_first_local, 0 as vaccinated_first_nonlocal, 
                        count(vv.id) as vaccinated_second, 0 as vaccinated_second_local,0 as vaccinated_second_nonlocal, 0 as notvaccinated, 
                        0 as old, 0 as children, 0 as pregnant, 0 as lactation, 0 as sick, 0 as outwork, 0 as missing, 0 as other, 0 as notRegister, 
                        0 as vaccinated_third, 0 as vaccinated_third_local,0 as vaccinated_third_nonlocal
                        from VillagePopulation vp,VillageVaccination vv
                        where vp.id = vv.populationId and vv.isVaccination = 1 and vv.numberStitch = 2 and vp.isDeleted = 0 and vv.isDeleted = 0 {0} {1}
                        group by vp.areaId
                        union all
                        select vp.areaId,0 as vaccinated, 0 as vaccinated_first, 0 as vaccinated_first_local, 0 as vaccinated_first_nonlocal, 
                        0 as vaccinated_second, count(vv.id) as vaccinated_second_local,0 as vaccinated_second_nonlocal, 0 as notvaccinated, 
                        0 as old, 0 as children, 0 as pregnant, 0 as lactation, 0 as sick, 0 as outwork, 0 as missing, 0 as other, 0 as notRegister, 
                        0 as vaccinated_third, 0 as vaccinated_third_local,0 as vaccinated_third_nonlocal
                        from VillagePopulation vp,VillageVaccination vv
                        where vp.id = vv.populationId and vv.isVaccination = 1 and vv.numberStitch = 2 and addressType = 1 and vp.isDeleted = 0 and vv.isDeleted = 0 {0} {1}
                        group by vp.areaId
                        union all
                        select vp.areaId,0 as vaccinated, 0 as vaccinated_first, 0 as vaccinated_first_local, 0 as vaccinated_first_nonlocal, 
                        0 as vaccinated_second, 0 as vaccinated_second_local,count(vv.id) as vaccinated_second_nonlocal, 0 as notvaccinated, 
                        0 as old, 0 as children, 0 as pregnant, 0 as lactation, 0 as sick, 0 as outwork, 0 as missing, 0 as other, 0 as notRegister, 
                        0 as vaccinated_third, 0 as vaccinated_third_local,0 as vaccinated_third_nonlocal
                        from VillagePopulation vp,VillageVaccination vv
                        where vp.id = vv.populationId and vv.isVaccination = 1  and vv.numberStitch = 2 and addressType = 2 and vp.isDeleted = 0 and vv.isDeleted = 0 {0} {1}
                        group by vp.areaId
                        union all
                        select vp.areaId,0 as vaccinated, 0 as vaccinated_first, 0 as vaccinated_first_local, 0 as vaccinated_first_nonlocal, 
                        0 as vaccinated_second, 0 as vaccinated_second_local,0 as vaccinated_second_nonlocal, 0 as notvaccinated, 
                        0 as old, 0 as children, 0 as pregnant, 0 as lactation, 0 as sick, 0 as outwork, 0 as missing, 0 as other, 0 as notRegister, 
                        count(vv.id) as vaccinated_third, 0 as vaccinated_third_local,0 as vaccinated_third_nonlocal
                        from VillagePopulation vp,VillageVaccination vv
                        where vp.id = vv.populationId and vv.isVaccination = 1 and vv.numberStitch = 3 and vp.isDeleted = 0 and vv.isDeleted = 0 {0} {1}
                        group by vp.areaId
                        union all
                        select vp.areaId,0 as vaccinated, 0 as vaccinated_first, 0 as vaccinated_first_local, 0 as vaccinated_first_nonlocal, 
                        0 as vaccinated_second, 0 as vaccinated_second_local,0 as vaccinated_second_nonlocal, 0 as notvaccinated, 
                        0 as old, 0 as children, 0 as pregnant, 0 as lactation, 0 as sick, 0 as outwork, 0 as missing, 0 as other, 0 as notRegister, 
                        0 as vaccinated_third, count(vv.id) as vaccinated_third_local,0 as vaccinated_third_nonlocal
                        from VillagePopulation vp,VillageVaccination vv
                        where vp.id = vv.populationId and vv.isVaccination = 1 and vv.numberStitch = 3 and addressType = 1 and vp.isDeleted = 0 and vv.isDeleted = 0 {0} {1}
                        group by vp.areaId
                        union all
                        select vp.areaId,0 as vaccinated, 0 as vaccinated_first, 0 as vaccinated_first_local, 0 as vaccinated_first_nonlocal, 
                        0 as vaccinated_second, 0 as vaccinated_second_local,0 as vaccinated_second_nonlocal, 0 as notvaccinated, 
                        0 as old, 0 as children, 0 as pregnant, 0 as lactation, 0 as sick, 0 as outwork, 0 as missing, 0 as other, 0 as notRegister, 
                        0 as vaccinated_third, 0 as vaccinated_third_local,count(vv.id) as vaccinated_third_nonlocal
                        from VillagePopulation vp,VillageVaccination vv
                        where vp.id = vv.populationId and vv.isVaccination = 1  and vv.numberStitch = 3 and addressType = 2 and vp.isDeleted = 0 and vv.isDeleted = 0 {0} {1}
                        group by vp.areaId                        
                        union all
                        select vp.areaId,0 as vaccinated, 0 as vaccinated_first, 0 as vaccinated_first_local, 0 as vaccinated_first_nonlocal, 
                        0 as vaccinated_second, 0 as vaccinated_second_local,0 as vaccinated_second_nonlocal, count(vv.id) as notvaccinated, 
                        0 as old, 0 as children, 0 as pregnant, 0 as lactation, 0 as sick, 0 as outwork, 0 as missing, 0 as other, 0 as notRegister, 
                        0 as vaccinated_third, 0 as vaccinated_third_local,0 as vaccinated_third_nonlocal
                        from VillagePopulation vp,VillageVaccination vv
                        where vp.id = vv.populationId and vv.isVaccination = 0 and vp.isDeleted = 0 and vv.isDeleted = 0 {0} {4}
                        group by vp.areaId
                        union all
                        select vp.areaId,0 as vaccinated, 0 as vaccinated_first, 0 as vaccinated_first_local, 0 as vaccinated_first_nonlocal, 
                        0 as vaccinated_second, 0 as vaccinated_second_local,0 as vaccinated_second_nonlocal, 0 as notvaccinated, 
                        count(vv.id) as old, 0 as children, 0 as pregnant, 0 as lactation, 0 as sick, 0 as outwork, 0 as missing, 0 as other, 0 as notRegister, 
                        0 as vaccinated_third, 0 as vaccinated_third_local,0 as vaccinated_third_nonlocal
                        from VillagePopulation vp,VillageVaccination vv
                        where vp.id = vv.populationId and vv.isVaccination = 0 and notReason = '高龄' and vp.isDeleted = 0 and vv.isDeleted = 0 {0} {4}
                        group by vp.areaId
                        union all
                        select vp.areaId,0 as vaccinated, 0 as vaccinated_first, 0 as vaccinated_first_local, 0 as vaccinated_first_nonlocal, 
                        0 as vaccinated_second, 0 as vaccinated_second_local,0 as vaccinated_second_nonlocal, 0 as notvaccinated, 
                        0 as old, count(vv.id) as children, 0 as pregnant, 0 as lactation, 0 as sick, 0 as outwork, 0 as missing, 0 as other, 0 as notRegister, 
                        0 as vaccinated_third, 0 as vaccinated_third_local,0 as vaccinated_third_nonlocal
                        from VillagePopulation vp,VillageVaccination vv
                        where vp.id = vv.populationId and vv.isVaccination = 0 and (notReason = '低龄' or notReason = '未成年') and vp.isDeleted = 0 and vv.isDeleted = 0 {0} {4}
                        group by vp.areaId
                        union all
                        select vp.areaId,0 as vaccinated, 0 as vaccinated_first, 0 as vaccinated_first_local, 0 as vaccinated_first_nonlocal, 
                        0 as vaccinated_second, 0 as vaccinated_second_local,0 as vaccinated_second_nonlocal, 0 as notvaccinated, 
                        0 as old, 0 as children, count(vv.id) as pregnant, 0 as lactation, 0 as sick, 0 as outwork, 0 as missing, 0 as other, 0 as notRegister, 
                        0 as vaccinated_third, 0 as vaccinated_third_local,0 as vaccinated_third_nonlocal
                        from VillagePopulation vp,VillageVaccination vv
                        where vp.id = vv.populationId and vv.isVaccination = 0 and notReason = '怀孕' and vp.isDeleted = 0 and vv.isDeleted = 0 {0} {4}
                        group by vp.areaId
                        union all
                        select vp.areaId,0 as vaccinated, 0 as vaccinated_first, 0 as vaccinated_first_local, 0 as vaccinated_first_nonlocal, 
                        0 as vaccinated_second, 0 as vaccinated_second_local,0 as vaccinated_second_nonlocal, 0 as notvaccinated, 
                        0 as old, 0 as children, 0 as pregnant, count(vv.id) as lactation, 0 as sick, 0 as outwork, 0 as missing, 0 as other, 0 as notRegister, 
                        0 as vaccinated_third, 0 as vaccinated_third_local,0 as vaccinated_third_nonlocal
                        from VillagePopulation vp,VillageVaccination vv
                        where vp.id = vv.populationId and vv.isVaccination = 0 and (notReason = '哺乳期' or notReason = '哺乳') and vp.isDeleted = 0 and vv.isDeleted = 0 {0} {4}
                        group by vp.areaId
                        union all
                        select vp.areaId,0 as vaccinated, 0 as vaccinated_first, 0 as vaccinated_first_local, 0 as vaccinated_first_nonlocal, 
                        0 as vaccinated_second, 0 as vaccinated_second_local,0 as vaccinated_second_nonlocal, 0 as notvaccinated, 
                        0 as old, 0 as children, 0 as pregnant, 0 as lactation, count(vv.id) as sick, 0 as outwork, 0 as missing, 0 as other, 0 as notRegister, 
                        0 as vaccinated_third, 0 as vaccinated_third_local,0 as vaccinated_third_nonlocal
                        from VillagePopulation vp,VillageVaccination vv
                        where vp.id = vv.populationId and vv.isVaccination = 0 and notReason = '因病' and vp.isDeleted = 0 and vv.isDeleted = 0 {0} {4}
                        group by vp.areaId
                        union all
                        select vp.areaId,0 as vaccinated, 0 as vaccinated_first, 0 as vaccinated_first_local, 0 as vaccinated_first_nonlocal, 
                        0 as vaccinated_second, 0 as vaccinated_second_local,0 as vaccinated_second_nonlocal, 0 as notvaccinated, 
                        0 as old, 0 as children, 0 as pregnant, 0 as lactation, 0 as sick, count(vv.id) as outwork, 0 as missing, 0 as other, 0 as notRegister, 
                        0 as vaccinated_third, 0 as vaccinated_third_local,0 as vaccinated_third_nonlocal
                        from VillagePopulation vp,VillageVaccination vv
                        where vp.id = vv.populationId and vv.isVaccination = 0 and notReason = '外出' and vp.isDeleted = 0 and vv.isDeleted = 0 {0} {4}
                        group by vp.areaId
                        union all
                        select vp.areaId,0 as vaccinated, 0 as vaccinated_first, 0 as vaccinated_first_local, 0 as vaccinated_first_nonlocal, 
                        0 as vaccinated_second, 0 as vaccinated_second_local,0 as vaccinated_second_nonlocal, 0 as notvaccinated, 
                        0 as old, 0 as children, 0 as pregnant, 0 as lactation, 0 as sick, 0 as outwork, count(vv.id) as missing, 0 as other, 0 as notRegister, 
                        0 as vaccinated_third, 0 as vaccinated_third_local,0 as vaccinated_third_nonlocal
                        from VillagePopulation vp,VillageVaccination vv
                        where vp.id = vv.populationId and vv.isVaccination = 0 and notReason = '失联' and vp.isDeleted = 0 and vv.isDeleted = 0 {0} {4}
                        group by vp.areaId
                        union all
                        select vp.areaId,0 as vaccinated, 0 as vaccinated_first, 0 as vaccinated_first_local, 0 as vaccinated_first_nonlocal, 
                        0 as vaccinated_second, 0 as vaccinated_second_local,0 as vaccinated_second_nonlocal, 0 as notvaccinated, 
                        0 as old, 0 as children, 0 as pregnant, 0 as lactation, 0 as sick, 0 as outwork, 0 as missing, count(vv.id) as other, 0 as notRegister, 
                        0 as vaccinated_third, 0 as vaccinated_third_local,0 as vaccinated_third_nonlocal
                        from VillagePopulation vp,VillageVaccination vv
                        where vp.id = vv.populationId and vv.isVaccination = 0 and notReason = '其他' and vp.isDeleted = 0 and vv.isDeleted = 0 {0} {4}
                        group by vp.areaId
                        union all
                        SELECT vp.areaId,0 as vaccinated, 0 as vaccinated_first, 0 as vaccinated_first_local, 0 as vaccinated_first_nonlocal, 
                        0 as vaccinated_second, 0 as vaccinated_second_local,0 as vaccinated_second_nonlocal, 0 as notvaccinated, 
                        0 as old, 0 as children, 0 as pregnant, 0 as lactation, 0 as sick, 0 as outwork, 0 as missing, 0 as other, count(vp.id) as notRegister, 
                        0 as vaccinated_third, 0 as vaccinated_third_local,0 as vaccinated_third_nonlocal
                        from VillagePopulation vp
                        where vp.isDeleted=0 and vp.isDeleted=0 {0} and vp.id not in (select vv.populationId from VillageVaccination vv where vv.isDeleted=0 {2})
                     ) b group by areaId
                   ) a on a.areaId = BasicArea.id where BasicArea.isDeleted = 0 and BasicArea.level = 5 {3}", sqlWhere_areaid, sqlWhere_vv, sqlWhere_vv, basic_areaid, sqlWhere_vv_not);

            var pageData = this.Context.Database.SqlQueryPagedList<StatisticsVaccinationDto>(body.Page, body.Limit, sql, "", "");
            return pageData;
        }


        public async Task<byte[]> GetVaccinationStatisticsExcelData(VaccinationPagePostBody body)
        {
            //VaccinationPagePostBody body = new VaccinationPagePostBody()
            //{
            //    AreaId = areaId,
            //    BeginDate = beginDate,
            //    EndDate = endDate
            //};
            IPagedList<StatisticsVaccinationDto> list = await this.GetVaccinationStatisticsList(body);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();

            package.Workbook.Properties.Title = "新冠疫苗接种统计";
            var workSheet = package.Workbook.Worksheets.Add("新冠疫苗接种统计");
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

            workSheet.Cells[1, 4, 1, 6].Merge = true;
            workSheet.Cells[1, 4, 1, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 4].Value = "第一针接种人数";

            workSheet.Cells[2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 4].Value = "接种总数";

            workSheet.Cells[2, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 5].Value = "本地接种";

            workSheet.Cells[2, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 6].Value = "异地接种";

            workSheet.Cells[1, 7, 1, 9].Merge = true;
            workSheet.Cells[1, 7, 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 7].Value = "第二针接种人数";

            workSheet.Cells[2, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 7].Value = "接种总数";

            workSheet.Cells[2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 8].Value = "本地接种";

            workSheet.Cells[2, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 9].Value = "异地接种";

            workSheet.Cells[1, 10, 1, 17].Merge = true;
            workSheet.Cells[1, 10, 1, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 10].Value = "未接种人数";

            workSheet.Cells[2, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 10].Value = "未成年";

            workSheet.Cells[2, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 11].Value = "因病";

            workSheet.Cells[2, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 12].Value = "外出";

            workSheet.Cells[2, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 13].Value = "怀孕";

            workSheet.Cells[2, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 14].Value = "哺乳期";

            workSheet.Cells[2, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 15].Value = "高龄";

            workSheet.Cells[2, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 16].Value = "失联";

            workSheet.Cells[2, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[2, 17].Value = "其他";

            List<StatisticsVaccinationDto> vaccinations = list.ToList();
            for (int i = 0; i < vaccinations.Count; i++)
            {
                int rowIndex = i + 3;
                StatisticsVaccinationDto vaccination = vaccinations[i];

                workSheet.Cells[rowIndex, 1].Value = vaccination.AreaName;
                workSheet.Cells[rowIndex, 2].Value = vaccination.Household;
                workSheet.Cells[rowIndex, 3].Value = vaccination.Population;
                workSheet.Cells[rowIndex, 4].Value = vaccination.Vaccinated_first;
                workSheet.Cells[rowIndex, 5].Value = vaccination.Vaccinated_first_local;
                workSheet.Cells[rowIndex, 6].Value = vaccination.Vaccinated_first_nonlocal;
                workSheet.Cells[rowIndex, 7].Value = vaccination.Vaccinated_second;
                workSheet.Cells[rowIndex, 8].Value = vaccination.Vaccinated_second_local;
                workSheet.Cells[rowIndex, 9].Value = vaccination.Vaccinated_second_nonlocal;
                workSheet.Cells[rowIndex, 10].Value = vaccination.Children;
                workSheet.Cells[rowIndex, 11].Value = vaccination.Sick;
                workSheet.Cells[rowIndex, 12].Value = vaccination.Outwork;
                workSheet.Cells[rowIndex, 13].Value = vaccination.Pregnant;
                workSheet.Cells[rowIndex, 14].Value = vaccination.Lactation;
                workSheet.Cells[rowIndex, 15].Value = vaccination.Old;
                workSheet.Cells[rowIndex, 16].Value = vaccination.Missing;
                workSheet.Cells[rowIndex, 17].Value = vaccination.Other;
            }
            return await package.GetAsByteArrayAsync();
        }

        public async Task<byte[]> GetVaccinationExcelData(int areaId, int year, string ids)
        {
            PagePostBody body = new PagePostBody()
            {
                AreaId = areaId,
                Year = year,
                Ids = ids,
                Page = 1,
                Limit = 10000
            };
            IPagedList<VaccinationHouseholdDto> list = await this.GetVaccinationList(body);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();

            package.Workbook.Properties.Title = "新冠疫苗接种登记";
            var workSheet = package.Workbook.Worksheets.Add("新冠疫苗接种登记");
            // 表头
            workSheet.Cells[1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 1].Value = "序号";
            workSheet.Cells[1, 2].Value = "门牌名";
            workSheet.Cells[1, 3].Value = "门牌号";
            workSheet.Cells[1, 4].Value = "户主姓名";
            workSheet.Cells[1, 5].Value = "联系方式";
            workSheet.Cells[1, 6].Value = "家庭成员数";
            workSheet.Cells[1, 7].Value = "第一针接种";
            workSheet.Cells[1, 8].Value = "第二针接种";

            List<VaccinationHouseholdDto> vaccinations = list.ToList();
            for (int i = 0; i < vaccinations.Count; i++)
            {
                int rowIndex = i + 2;
                VaccinationHouseholdDto vaccination = vaccinations[i];

                workSheet.Cells[rowIndex, 1].Value = i + 1;
                workSheet.Cells[rowIndex, 2].Value = vaccination.HouseName;
                workSheet.Cells[rowIndex, 3].Value = vaccination.HouseNumber;
                workSheet.Cells[rowIndex, 4].Value = vaccination.HouseholdMan;
                workSheet.Cells[rowIndex, 5].Value = vaccination.Mobile;
                workSheet.Cells[rowIndex, 6].Value = vaccination.PeopleCount;
                workSheet.Cells[rowIndex, 7].Value = vaccination.FirstCount;
                workSheet.Cells[rowIndex, 8].Value = vaccination.SecondCount;
            }
            return await package.GetAsByteArrayAsync();
        }

        public async Task<IPagedList<VaccinationInfoDto>> GetVaccinationStatisticsList(int householdId, int page = 1, int limit = 10)
        {
            var sql = " select vc.populationid,vp.realname , vc.isHouseholder, vp.birthday , (select count(id) from VillageVaccination vv where vv.populationid = vp.id and vv.isdeleted=0 and vv.numberStitch = 1) as vaccinated_first," +
                "  (select count(id) from VillageVaccination vv where vv.populationid = vp.id and vv.isdeleted=0 and vv.numberStitch = 2) as vaccinated_second, " +
                "(select count(id) from VillageVaccination vv where vv.populationid = vp.id and vv.isdeleted=0 and vv.numberStitch = 3) as vaccinated_third " +
                "from VillagePopulation vp, VillageHouseCodeMember vc   where vc.populationid = vp.id and vc.householdid = " + householdId + " and vp.isdeleted=0 and vc.isdeleted=0 ";

            var pageData = this.Context.Database.SqlQueryPagedList<VaccinationInfoDto>(page, limit, sql, "", " order by isHouseholder desc, birthday asc");
            return pageData;
        }
    }
}
