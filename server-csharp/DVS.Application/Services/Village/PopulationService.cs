using AutoMapper;
using DVS.Common.Infrastructures;
using DVS.Common.Services;
using DVS.Core.Domains.Common;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.Village.Query;
using DVS.Models.Dtos.Village.Statistics;
using LinqKit;
using Lychee.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;
using DVS.Models.Enum;
using AutoMapper.QueryableExtensions;
using DVS.Models.Dtos.Village;
using DVS.Application.Services.Common;
using DVS.Common.SO;
using System.IO;
using System.Data;
using DVS.Common;
using DVS.Models.Dtos.Village.Export;
using System.Text;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Village.Household;
using DVS.Common.Models;

namespace DVS.Application.Services.Village
{
    public class PopulationService : ServiceBase<VillagePopulation>, IPopulationService
    {
        private readonly IServiceBase<VillageHouseCodeMember> memberService;
        private readonly IBasicAreaService basicAreaService;
        private readonly IServiceBase<BasicUser> _basicUserService;
        private readonly IPopulationTagService populationTagService;
        private readonly IPopulationAddressService populationAddressService;
        private readonly ISunFileInfoService sunFileService;
        private readonly IBasicDictionaryService basicDictionaryService;
        private readonly IServiceBase<VillageHouseholdCode> householdCodeService;
        private readonly IHouseholdCodeTagService householdCodeTagService;
        private readonly int isDeleted = 0;

        public PopulationService(DbContext dbContext, IMapper mapper,
            IServiceBase<BasicUser> basicUserService,
            IBasicAreaService basicAreaService,
            IPopulationTagService populationTagService,
            IPopulationAddressService populationAddressService,
            ISunFileInfoService sunFileService,
            IBasicDictionaryService basicDictionaryService,
            IServiceBase<VillageHouseholdCode> householdCodeService,
            IServiceBase<VillageHouseCodeMember> memberService,
            IHouseholdCodeTagService householdCodeTagService
            )
            : base(dbContext, mapper)
        {
            this.basicAreaService = basicAreaService;
            this._basicUserService = basicUserService;
            this.populationTagService = populationTagService;
            this.populationAddressService = populationAddressService;
            this.sunFileService = sunFileService;
            this.basicDictionaryService = basicDictionaryService;
            this.householdCodeService = householdCodeService;
            this.memberService = memberService;
            this.householdCodeTagService = householdCodeTagService;
        }

        public async Task<PopulationDetailDto> GetPopulationDetail(int id, string idCard = "", int isConvert = 0, int includeDeleted = 0, int householdId = 0, int areaId = 0)
        {
            VillagePopulation data;
            if (id > 0)
            {
                if (includeDeleted == 0)
                {
                    data = await this.GetAsync(a => a.Id == id && a.IsDeleted == isDeleted);
                }
                else
                {
                    data = await this.GetAsync(a => a.Id == id);
                }
                // data = await this.GetQueryable().Where(a => a.Id == id).ProjectTo<PopulationDetailDto>(mapper.ConfigurationProvider).ToListAsync();
            }
            else
            {
                string _idCard = BasicSO.Encrypt(idCard);
                data = await this.GetAsync(a => a.IdCard == _idCard && a.IsDeleted == isDeleted && a.AreaId == areaId);
            }

            if (data != null)
            {

                var population = mapper.Map<PopulationDetailDto>(data);
                if (population.Birthday != null)
                {
                    population.Birthday = data.Birthday.ToString("yyyy-MM-dd");
                }
                population.HeadImageUrl = this.sunFileService.ToAbsolutePath(population.HeadImageUrl);
                population.TagNames = await this.populationTagService.GetTags(population.Id);
                population.RegisterAddressInfo = await this.populationAddressService.GetAddressDetail(population.Id, PopulationAddressTypeEnum.户籍地);
                population.LiveAddressInfo = await this.populationAddressService.GetAddressDetail(population.Id, PopulationAddressTypeEnum.居住地);
                population.NativePlaceInfo = await this.populationAddressService.GetAddressDetail(population.Id, PopulationAddressTypeEnum.籍贯);
                population.RealName = BasicSO.Decrypt(population.RealName);
                if (isConvert == 1)
                {
                    List<string> dirs = new List<string>();
                    dirs.Add(population.Education);
                    dirs.Add(population.Income);
                    dirs.Add(population.Marital);
                    dirs.Add(population.Nation);
                    dirs.Add(population.Political);
                    dirs.Add(population.Relationship);
                    dirs.Add(population.Religion);
                    var dirNames = await this.basicDictionaryService.GetBasicDictionaryCodeList(dirs);

                    population.Education = await this.basicDictionaryService.GetNameByCode(population.Education, dirNames);
                    population.Income = await this.basicDictionaryService.GetNameByCode(population.Income, dirNames);
                    population.Marital = await this.basicDictionaryService.GetNameByCode(population.Marital, dirNames);
                    population.Nation = await this.basicDictionaryService.GetNameByCode(population.Nation, dirNames);
                    population.Political = await this.basicDictionaryService.GetNameByCode(population.Political, dirNames);
                    population.Relationship = await this.basicDictionaryService.GetNameByCode(population.Relationship, dirNames);
                    population.Religion = await this.basicDictionaryService.GetNameByCode(population.Religion, dirNames);
                }

                var houseLinq = from m in this.memberService.GetQueryable()
                                join h in this.householdCodeService.GetQueryable() on new { Id = m.HouseholdId, IsDeleted = 0 } equals new { h.Id, h.IsDeleted }
                                where m.IsDeleted == 0 && m.PopulationId == population.Id
                                orderby m.CreatedAt ascending
                                select new
                                {
                                    m.HouseholdId,
                                    m.IsHouseholder,
                                    h.HouseName,
                                    h.HouseNumber,
                                };
                /// var house = await this.householdCodeService.GetAsync(a => a.Id == householdId);

                if (householdId > 0)
                {
                    var house = await houseLinq.FirstOrDefaultAsync(a => a.HouseholdId == householdId);
                    if (house != null)
                    {
                        population.HouseName = house.HouseName;
                        population.HouseNumber = house.HouseNumber;
                        population.IsHouseholder = house.IsHouseholder;
                        population.HouseholdId = house.HouseholdId;
                    }
                }
                else
                {
                    var house = await houseLinq.FirstOrDefaultAsync();
                    if (house != null)
                    {
                        population.HouseName = house.HouseName;
                        population.HouseNumber = house.HouseNumber;
                        population.IsHouseholder = house.IsHouseholder;
                        population.HouseholdId = house.HouseholdId;
                    }
                }




                // population.Mobile = BasicSO.Decrypt(population.Mobile);
                // population.IdCard = BasicSO.Decrypt(population.IdCard);

                return population;
            }
            return null;
        }


        public async Task<PopulationDetailDto> GetHouseholdManDetail(int householdId, int isConvert = 0)
        {

            var data = await this.memberService.GetAsync(a => a.HouseholdId == householdId && a.IsDeleted == 0 && a.IsHouseholder == 1);
            if (data == null)
            {
                return null;
            }

            var population = await GetPopulationDetail(data.PopulationId, "", isConvert, 0, householdId);
            return population;
        }

        public async Task<PopulationDetailDto> GetPopulationList(int id, string idCard = "", int isConvert = 0)
        {
            VillagePopulation data;
            if (id > 0)
            {
                data = await this.GetAsync(a => a.Id == id && a.IsDeleted == isDeleted);
                // data = await this.GetQueryable().Where(a => a.Id == id).ProjectTo<PopulationDetailDto>(mapper.ConfigurationProvider).ToListAsync();
            }
            else
            {
                string _idCard = BasicSO.Encrypt(idCard);
                data = await this.GetAsync(a => a.IdCard == _idCard && a.IsDeleted == isDeleted);
            }

            if (data != null)
            {

                var population = mapper.Map<PopulationDetailDto>(data);
                population.HeadImageUrl = this.sunFileService.ToAbsolutePath(population.HeadImageUrl);
                population.TagNames = await this.populationTagService.GetTags(population.Id);
                population.RegisterAddressInfo = await this.populationAddressService.GetAddressDetail(population.Id, PopulationAddressTypeEnum.户籍地);
                population.LiveAddressInfo = await this.populationAddressService.GetAddressDetail(population.Id, PopulationAddressTypeEnum.居住地);
                population.NativePlaceInfo = await this.populationAddressService.GetAddressDetail(population.Id, PopulationAddressTypeEnum.籍贯);

                if (isConvert == 1)
                {
                    List<string> dirs = new List<string>();
                    dirs.Add(population.Education);
                    dirs.Add(population.Income);
                    dirs.Add(population.Marital);
                    dirs.Add(population.Nation);
                    dirs.Add(population.Political);
                    dirs.Add(population.Relationship);
                    dirs.Add(population.Religion);
                    var dirNames = await this.basicDictionaryService.GetBasicDictionaryCodeList(dirs);

                    population.Education = await this.basicDictionaryService.GetNameByCode(population.Education, dirNames);
                    population.Income = await this.basicDictionaryService.GetNameByCode(population.Income, dirNames);
                    population.Marital = await this.basicDictionaryService.GetNameByCode(population.Marital, dirNames);
                    population.Nation = await this.basicDictionaryService.GetNameByCode(population.Nation, dirNames);
                    population.Political = await this.basicDictionaryService.GetNameByCode(population.Political, dirNames);
                    population.Relationship = await this.basicDictionaryService.GetNameByCode(population.Relationship, dirNames);
                    population.Religion = await this.basicDictionaryService.GetNameByCode(population.Religion, dirNames);

                    if (population.HouseholdId > 0)
                    {

                        var house = await this.householdCodeService.GetAsync(a => a.Id == population.HouseholdId);
                        if (house != null)
                        {
                            population.HouseName = house.HouseName;
                            population.HouseNumber = house.HouseNumber;
                        }

                    }
                }
                // population.RealName = BasicSO.Decrypt(population.RealName);
                // population.Mobile = BasicSO.Decrypt(population.Mobile);
                // population.IdCard = BasicSO.Decrypt(population.IdCard);

                return population;
            }
            return null;
        }

        private async Task<List<VillageHouseCodeMember>> GetPopulationMembers(int householdId)
        {

            var data = await this.memberService.GetListAsync(a => a.IsDeleted == 0 && a.HouseholdId == householdId);
            return data.ToList();
        }

        private async Task<List<VillageHouseCodeMember>> GetPopulationMembersByPopulationId(int populationId)
        {

            var query = from m in this.memberService.GetQueryable()
                        join p in this.memberService.GetQueryable() on new { m.HouseholdId, IsDeleted = 0 } equals new { p.HouseholdId, p.IsDeleted }
                        where m.PopulationId == populationId && m.IsDeleted == 0
                        select p;

            return await query.ToListAsync();
        }



        public async Task<IPagedList<PopulationDto>> GetPopulationList(PopulationListBody body)
        {

            string sql = string.Format(@" SELECT 
													 p.id,
													 p.realName,
													 p.sex,
													 p.nation,
													 p.idCard,
													 p.education,
													 p.mobile,
													 p.relationship,
													 p.areaId,
													 p.headImageUrl,
                                                     p.createdAt,
													 0 as isHouseholder,
													 CONCAT(a1.province,' ',a1.city,' ',a1.district,' ',a1.address) as registerAddress, 
													 CONCAT(a2.province,' ',a2.city,' ',a2.district,' ',a2.address) as liveAddress
													 from VillagePopulation p
													 LEFT JOIN VillagePopulationAddress a1 on p.id = a1.populationId and a1.type=1 
													 LEFT JOIN  VillagePopulationAddress a2 on p.id = a2.populationId and a2.type=2
													 WHERE  p.isDeleted=0 and p.areaId={0} ", body.AreaId);
            string sqlCount = string.Format(@" SELECT  p.id from VillagePopulation p WHERE  p.isDeleted=0 and p.areaId={0} ", body.AreaId);
            List<VillageHouseCodeMember> members = new List<VillageHouseCodeMember>();
            if (body.HouseholdId > -1)
            {
                members = await GetPopulationMembers(body.HouseholdId);
                StringBuilder sb = new StringBuilder();
                foreach (var item in members)
                {
                    sb.Append(item.PopulationId);
                    sb.Append(",");
                }

                string ins = sb.ToString().Trim(',');

                sql += " and p.id in(" + (ins == "" ? "0" : ins) + ")";
                sqlCount += " and p.id in(" + (ins == "" ? "0" : ins) + ")";
            }

            if (body.PopulationId > 0)
            {
                members = await GetPopulationMembersByPopulationId(body.PopulationId);
                StringBuilder sb = new StringBuilder();
                foreach (var item in members)
                {
                    sb.Append(item.PopulationId);
                    sb.Append(",");
                }
                string ins = sb.ToString().Trim(',');

                sql += " and p.id in(" + (ins == "" ? "0" : ins) + ")";
                sqlCount += " and p.id in(" + (ins == "" ? "0" : ins) + ")";
            }


            if (!string.IsNullOrWhiteSpace(body.Ids))
            {

                sql += $" and p.id in ({body.Ids.Replace("-", "").Replace("delete", "").Replace("drop", "").Replace("update", "")}) ";
                sqlCount += $" and p.id in ({body.Ids.Replace("-", "").Replace("delete", "").Replace("drop", "").Replace("update", "")}) ";

            }

            if (!string.IsNullOrWhiteSpace(body.Tags))
            {

                sql += $" and p.tags in ({body.Tags})";
                sqlCount += $" and p.tags in ({body.Tags})";

            }

            if (!string.IsNullOrWhiteSpace(body.Keyword))
            {
                body.Keyword = body.Keyword.Replace("'", "").Replace("\"", "").Replace("-", "");
                // body.Keyword = BasicSO.Encrypt(body.Keyword);
                // string like = string.Format(" and (p.mobile LIKE '%{0}%' or p.idCard LIKE '%{0}%' or p.realName LIKE '%{0}%')", body.Keyword);
                string like = string.Format(" and (p.mobile='{0}' or p.idCard='{0}' or p.realName LIKE '%{1}%' or p.mobileShort LIKE '%{1}%') ", BasicSO.Encrypt(body.Keyword), body.Keyword);
                sql += like;
                sqlCount += like;
            }


            // sql += " order by p.id desc ";


            var pageData = this.Context.Database.SqlQueryPagedList<PopulationDto>(body.Page, body.Limit, sql, sqlCount, this.Context.Database.GetOrderBySql(body.Orders));
            List<int> ids = new List<int>();
            List<string> dirs = new List<string>();
            foreach (var item in pageData)
            {
                ids.Add(item.Id);
                item.HeadImageUrl = this.sunFileService.ToAbsolutePath(item.HeadImageUrl);
                dirs.Add(item.Education);
                dirs.Add(item.Nation);
                dirs.Add(item.Relationship);
                var member = members.FirstOrDefault(a => a.PopulationId == item.Id);
                item.IsHouseholder = member != null ? member.IsHouseholder : 0;
            }

            var tagNames = await this.populationTagService.GetTags(ids);
            var dirNames = await this.basicDictionaryService.GetBasicDictionaryCodeList(dirs);
            foreach (var item in pageData)
            {

                item.TagNames = tagNames.Where(a => a.Pid == item.Id).ToList();

                item.NationDto = await this.basicDictionaryService.GetOneByCode(item.Nation, dirNames);
                item.EducationDto = await this.basicDictionaryService.GetOneByCode(item.Education, dirNames);
                item.RelationshipDto = await this.basicDictionaryService.GetOneByCode(item.Relationship, dirNames);
                item.NationCode = item.Nation;
                item.Nation = await this.basicDictionaryService.GetNameByCode(item.Nation, dirNames);
                item.Education = await this.basicDictionaryService.GetNameByCode(item.Education, dirNames);
                item.Relationship = await this.basicDictionaryService.GetNameByCode(item.Relationship, dirNames);



                item.RealName = BasicSO.Decrypt(item.RealName);
                item.Mobile = BasicSO.Decrypt(item.Mobile);
                item.IdCard = BasicSO.Decrypt(item.IdCard);
            }
            return pageData;
        }


        private async Task<int> SaveHouseCodeMember(int householdId, int populationId, int updatedBy, string remark = "")
        {
            int id = 0;
            var data = await this.memberService.GetAsync(a => a.IsDeleted == 0 && a.HouseholdId == householdId && a.PopulationId == populationId);
            if (data == null)
            {
                VillageHouseCodeMember member = new VillageHouseCodeMember()
                {
                    HouseholdId = householdId,
                    IsDeleted = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    PopulationId = populationId,
                    UpdatedBy = updatedBy,
                    CreatedBy = updatedBy,
                    Remark = remark,
                    IsHouseholder = 0,
                };
                var res = await this.memberService.InsertAsync(member);
                if (res != null)
                {
                    id = res.Id;
                }
            }
            else
            {
                id = data.Id;
            }
            return id;
        }

        public async Task<MessageResult<int>> SavePopulation(PopulationDetailDto po, int updatedBy = 0)
        {
            var result = new MessageResult<int>("失败", false, po.Id);
            var population = await this.GetAsync(a => a.IdCard == BasicSO.Encrypt(po.IdCard) && a.IsDeleted == isDeleted && a.AreaId == po.AreaId);
            if (population != null && population.Id != po.Id)
            {
                result.Message = "已存在相同的身份证号，请重新输入";
                return result;
            }

            //po.RealName = BasicSO.Encrypt(po.RealName);
            //po.IdCard = BasicSO.Encrypt(po.IdCard);
            //po.Mobile = BasicSO.Encrypt(po.Mobile);
            string mobileShort = "";
            if (!string.IsNullOrWhiteSpace(po.Mobile) && po.Mobile.Length == 11)
            {
                mobileShort = po.Mobile.Substring(0, 3) + po.Mobile.Substring(7, 4);
            }

            po.HeadImageUrl = await this.sunFileService.GetSunFileRelativeUrls(po.HeadImageId);
            if (po.Id > 0)
            {
                var ddd = mapper.Map<VillagePopulation>(po);
                // ddd.UpdatedAt = DateTime.Now;
                var res = this.GetQueryable().Where(a => a.Id == po.Id).UpdateFromQuery(a => new VillagePopulation()
                {
                    AreaId = po.AreaId,
                    Birthday = DateTime.Parse(po.Birthday),
                    Education = po.Education,
                    // HouseholdId = po.HouseholdId,
                    IdCard = BasicSO.Encrypt(po.IdCard),
                    Income = po.Income,
                    Marital = po.Marital,
                    Mobile = BasicSO.Encrypt(po.Mobile),
                    Nation = po.Nation,
                    Political = po.Political,
                    RealName = po.RealName,
                    Relationship = po.Relationship,
                    Religion = po.Religion,
                    Remark = po.Remark,
                    Sex = po.Sex,
                    Tags = po.Tags,
                    UpdatedAt = DateTime.Now,
                    HeadImageId = po.HeadImageId,
                    HeadImageUrl = po.HeadImageUrl,
                    IsSync = 0,
                    UpdatedBy = updatedBy,
                    MobileShort = mobileShort,
                });
                if (res > 0)
                {

                    await this.populationTagService.SaveTags(po.Id, po.Tags);
                    await this.populationAddressService.SaveAddress(po.Id, PopulationAddressTypeEnum.户籍地, po.RegisterAddressInfo);
                    await this.populationAddressService.SaveAddress(po.Id, PopulationAddressTypeEnum.居住地, po.LiveAddressInfo);
                    await this.populationAddressService.SaveAddress(po.Id, PopulationAddressTypeEnum.籍贯, po.NativePlaceInfo);

                    if (po.HouseholdId > 0)
                    {
                        await SaveHouseCodeMember(po.HouseholdId, po.Id, updatedBy);
                    }

                    result.Message = "成功";
                    result.Flag = true;
                    result.Data = po.Id;
                }
            }
            else
            {
                var ddd = mapper.Map<VillagePopulation>(po);
                ddd.MobileShort = mobileShort;
                ddd.CreatedAt = DateTime.Now;
                var res = await this.InsertAsync(ddd);
                if (res != null)
                {
                    await this.populationTagService.SaveTags(res.Id, po.Tags);
                    await this.populationAddressService.SaveAddress(res.Id, PopulationAddressTypeEnum.户籍地, po.RegisterAddressInfo);
                    await this.populationAddressService.SaveAddress(res.Id, PopulationAddressTypeEnum.居住地, po.LiveAddressInfo);
                    await this.populationAddressService.SaveAddress(res.Id, PopulationAddressTypeEnum.籍贯, po.NativePlaceInfo);
                    if (po.HouseholdId > 0)
                    {
                        await SaveHouseCodeMember(po.HouseholdId, res.Id, updatedBy);
                    }
                    result.Message = "成功";
                    result.Flag = true;
                    result.Data = res.Id;
                }
            }
            if (result.Flag)
            {
                await this._basicUserService.GetQueryable().Where(user => user.PopulationId == result.Data).UpdateFromQueryAsync(user => new BasicUser() { HouseholdId = po.HouseholdId });
            }
            return result;
        }

        public async Task<MessageResult<bool>> SetFromHousehold(SetFromHouseholdBody body)
        {
            var result = new MessageResult<bool>("失败");
            int delete = 0;

            if (body.Action != 0 && body.Action != 1)
            {
                result.Message = "参数不合法";
                return result;
            }

            if (body.PopulationId <= 0)
            {
                result.Message = "请传入人口Id";
                return result;
            }



            //if (body.DeleteReason == "删除")
            //{
            //    delete = 1;
            //}

            var res = 0;

            // 移除户码
            if (body.Action == 0)
            {
                //res = await this.memberService.GetQueryable().Where(a => a.Id == body.PopulationId)
                //      .UpdateFromQueryAsync(a => new VillageHouseCodeMember()
                //      {
                //          // DeleteReason = string.IsNullOrWhiteSpace(body.DeleteReason) == true ? "移除户码" : body.DeleteReason,
                //          // HouseholdId = 0,
                //          // IsSync = 0,
                //          // SyncId = "",
                //          // IsDeleted = delete,
                //      });
                // ExecuteSqlAsync

                if (body.HouseholdId <= 0)
                {
                    result.Message = "请传入户码Id";
                    return result;
                }
                res = this.memberService.Context.Database.ExecuteSqlRaw(" update VillageHouseCodeMember set isdeleted=1,remark={0} where HouseholdId={1} and PopulationId={2} ", body.DeleteReason, body.HouseholdId, body.PopulationId);
                int count = await this._basicUserService.GetQueryable().Where(user => user.HouseholdId == body.HouseholdId && user.PopulationId == body.PopulationId).UpdateFromQueryAsync(user => new BasicUser() { HouseholdId = 0 });
                if (count > 0)
                { // TODO:退出登录

                }
            }
            else if (body.Action == 1)
            {
                // 移除人口
                if (string.IsNullOrEmpty(body.DeleteReason))
                {
                    result.Message = "请选择原因";
                    return result;
                }
                res = await this.GetQueryable().Where(a => a.Id == body.PopulationId)
                         .UpdateFromQueryAsync(a => new VillagePopulation()
                         {
                             DeleteReason = body.DeleteReason,
                             // HouseholdId = 0,
                             IsDeleted = 1,
                             IsSync = 0,
                             SyncId = "",
                         });

                if (res > 0)
                {
                    await this.memberService.Context.Database.ExecuteSqlRawAsync(" update VillageHouseCodeMember set isdeleted=1,remark={0} where PopulationId={1} ", body.DeleteReason, body.PopulationId);
                }

            }
            if (res > 0)
            {
                result.Message = "成功";
                result.Flag = true;
                result.Data = true;
                return result;
            }
            return result;
        }

        // 切换户码
        public async Task<bool> SwitchHouseholdCodeAsync(int householdId, int loginUserId)
        {
            BasicUser basicUser = await _basicUserService.GetQueryable().Where(user => user.Id == loginUserId).FirstOrDefaultAsync();
            if (basicUser == null)
            {
                throw new ValidException("用户不存在");
            }
            int populationId = basicUser.PopulationId;
            var memberInfo = await memberService.GetQueryable().Where(member => member.HouseholdId == householdId && member.PopulationId == populationId).FirstOrDefaultAsync();
            if (memberInfo == null)
            {
                throw new ValidException("您不属于该户，不允许切换");
            }
            int count = await _basicUserService.GetQueryable().Where(user => user.Id == basicUser.Id).UpdateFromQueryAsync(user => new BasicUser() { HouseholdId = householdId });
            return count > 0;
        }


        public async Task<MessageResult<bool>> SetHouseholdRelationship(SetRelationshipBody body)
        {
            var result = new MessageResult<bool>("失败");
            if (body.PopulationId <= 0)
            {
                result.Message = "请传入人口Id";
                return result;
            }

            if (body.HouseholdId <= 0)
            {
                result.Message = "请传入户码Id";
                return result;
            }

            //if (string.IsNullOrEmpty(body.Relationship))
            //{
            //    result.Message = "请传入与户主关系";
            //    return result;
            //}

            await this.memberService.Context.Database.ExecuteSqlRawAsync(" update VillageHouseCodeMember set IsHouseholder=0 where HouseholdId=" + body.HouseholdId);
            var res = await this.memberService.Context.Database.ExecuteSqlRawAsync(" update VillageHouseCodeMember set IsHouseholder=1 where HouseholdId={0} and PopulationId={1} ", body.HouseholdId, body.PopulationId);

            //var res = await this.GetQueryable().Where(a => a.Id == body.PopulationId && a.HouseholdId == body.HouseholdId)
            //      .UpdateFromQueryAsync(a => new VillagePopulation()
            //      {
            //          IsHouseholder = 1,
            //          IsSync = 0,
            //      });
            if (res > 0)
            {
                result.Message = "成功";
                result.Flag = true;
                result.Data = true;
                return result;
            }
            return result;
        }

        /// <summary>
        /// 乡村治理居民参与情况 男女比例
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<StatisticsCommonDto>> GetVillagePartakeSex(int areaId)
        {

            var areaIds = await basicAreaService.FindChildrenAreaIds(areaId);
            var query = from p in this.GetQueryable()
                        join u in this._basicUserService.GetQueryable() on p.Id equals u.PopulationId
                        where u.IsDeleted == 0 && u.IsAuth == UserAuthAuditStatusEnum.Passed && areaIds.Contains(u.AreaId) && u.Type == 1 && p.IsDeleted == 0
                        group p by p.Sex into g
                        select new StatisticsCommonDto
                        {
                            Name = g.Key.ToString(),
                            Value = g.Count()
                        };

            return await query.ToListAsync();
        }

        /// <summary>
        /// 乡村治理居民参与情况 年龄段
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<StatisticsCommonDto>> GetVillagePartakeAge(int areaId)
        {
            var areaIds = await basicAreaService.FindChildrenAreaIds(areaId);
            var query = from p in this.GetQueryable()
                        join u in this._basicUserService.GetQueryable() on p.Id equals u.PopulationId
                        where u.IsDeleted == 0 && u.IsAuth == UserAuthAuditStatusEnum.Passed && areaIds.Contains(u.AreaId) && u.Type == 1 && p.IsDeleted == 0
                        select p;

            var result = new List<StatisticsCommonDto>();
            var now = DateTime.Now;

            // 15-30
            var range1 = await query.Where(a => a.Birthday >= now.AddYears(-30) && a.Birthday < now.AddYears(-15)).CountAsync();
            result.Add(new StatisticsCommonDto { Name = "15~30岁", Value = range1 });
            // 31-45
            var range2 = await query.Where(a => a.Birthday >= now.AddYears(-45) && a.Birthday < now.AddYears(-30)).CountAsync();
            result.Add(new StatisticsCommonDto { Name = "31~45岁", Value = range2 });

            // 46-60
            var range3 = await query.Where(a => a.Birthday >= now.AddYears(-60) && a.Birthday < now.AddYears(-45)).CountAsync();
            result.Add(new StatisticsCommonDto { Name = "46~60岁", Value = range3 });

            // 其他
            var range4 = await query.Where(a => a.Birthday > now.AddYears(-15) || a.Birthday < now.AddYears(-60)).CountAsync();
            result.Add(new StatisticsCommonDto { Name = "其他", Value = range4 });

            return result;
        }


        public async Task<int> EncryptAllPopulation()
        {


            var list = await this.GetListAsync(a => a.IsDeleted == 0);

            int i = 0;

            foreach (var item in list)
            {


                if (!string.IsNullOrWhiteSpace(item.IdCard) && item.IdCard.Length < 19)
                {
                    item.IdCard = BasicSO.Encrypt(item.IdCard);
                }

                if (!string.IsNullOrWhiteSpace(item.Mobile) && item.Mobile.Length < 12)
                {
                    item.Mobile = BasicSO.Encrypt(item.Mobile);
                }
            }

            foreach (var item in list)
            {
                // var res = await this.Table.UpdateFromQuery()

                var res = await this.GetQueryable().Where(a => a.Id == item.Id)
               .UpdateFromQueryAsync(a => new VillagePopulation()
               {
                   Mobile = item.Mobile,
                   IdCard = item.IdCard,
                   RealName = item.RealName
               });

                if (res > 0)
                {
                    i += 1;
                }
            }

            return i;


        }

        public async Task<int> DecryptAllPopulation()
        {


            var list = await this.GetListAsync(a => a.IsDeleted >= 0);

            int i = 0;

            foreach (var item in list)
            {


                if (!string.IsNullOrWhiteSpace(item.RealName))
                {
                    item.RealName = BasicSO.Decrypt(item.RealName);
                }

                if (!string.IsNullOrWhiteSpace(item.Mobile) && item.Mobile.Length < 12)
                {
                    string mobile = BasicSO.Decrypt(item.Mobile);
                    item.MobileShort = mobile.Substring(0, 3) + mobile.Substring(7, 4);

                }
            }

            foreach (var item in list)
            {
                // var res = await this.Table.UpdateFromQuery()

                var res = await this.GetQueryable().Where(a => a.Id == item.Id)
               .UpdateFromQueryAsync(a => new VillagePopulation()
               {
                   MobileShort = item.MobileShort,
                   RealName = item.RealName
               });

                if (res > 0)
                {
                    i += 1;
                }
            }

            return i;


        }

        public string DictionaryGetCode(string name, IList<BasicDictionary> dictionaries, int typeCode, bool isTags = false)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "";
            }
            if (isTags)
            {
                var tags = name.Split('|');
                var ds = dictionaries.Where(a => tags.Contains(a.Name) && a.TypeCode == typeCode).ToList();
                StringBuilder sb = new StringBuilder();
                foreach (var item in ds)
                {

                    sb.Append(item.Code.ToString());
                    sb.Append(",");
                }
                return sb.ToString().TrimEnd(',');
            }
            else
            {

                var d = dictionaries.FirstOrDefault(a => a.Name == name && a.TypeCode == typeCode);
                return d == null ? "" : d.Code.ToString();
            }
            return "";
        }

        /// <summary>
        /// 导入户籍人口
        /// </summary>
        /// <param name="excelFile"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public async Task<MessageResult<ImportResultDto>> ImportPopulation(Stream fileStream, int areaId)
        {
            MessageResult<ImportResultDto> result = new MessageResult<ImportResultDto>();
            if (areaId <= 0)
            {
                result.Message = "缺少必要参数";
                return result;
            }

            DataSet dataset = Utils.ImportExcel(fileStream, 5, 18);
            int cnt = dataset.Tables.Count;
            if (cnt == 0)
            {
                result.Message = "无任何数据可导入";
                return result;
            }

            int i = 5;
            ImportResultDto importResult = new ImportResultDto();
            importResult.Errors = new List<ImportResultMessage>();

            Dictionary<string, int> householdIdList = new Dictionary<string, int>();
            int[] typeCodes = new int[] { 1010, 1011, 1012, 1013, 1014, 1015, 1016, 1017 };
            var dictionarys = await this.basicDictionaryService.GetListAsync(a => typeCodes.Contains(a.TypeCode) && a.IsDeleted == 0);

            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                int householdId = 0;

                string realName = row.ItemArray[0].ToString().Trim();
                string sex = row.ItemArray[1].ToString().Trim();
                string idCard = row.ItemArray[2].ToString().Trim();
                // string birthDay = row.ItemArray[3].ToString().Trim();
                string mobile = row.ItemArray[4].ToString().Trim();

                string relationship = row.ItemArray[5]?.ToString().Trim();
                string nation = row.ItemArray[6]?.ToString().Trim();
                string marital = row.ItemArray[7]?.ToString().Trim();
                string education = row.ItemArray[8]?.ToString().Trim();
                string political = row.ItemArray[9]?.ToString().Trim();
                string income = row.ItemArray[10]?.ToString().Trim();
                string tags = row.ItemArray[11]?.ToString().Trim();
                string religion = row.ItemArray[12]?.ToString().Trim();

                string registerAddress = row.ItemArray[13].ToString().Trim();
                string liveAddress = row.ItemArray[14].ToString().Trim();
                string remark = row.ItemArray[15].ToString().Trim();
                string houseName = row.ItemArray[16]?.ToString().Trim();
                string houseNumber = row.ItemArray[17]?.ToString().Trim();

                if (!string.IsNullOrWhiteSpace(houseName) && !string.IsNullOrWhiteSpace(houseNumber))
                {
                    var hid = householdIdList.FirstOrDefault(a => a.Key == houseName + houseNumber);

                    if (hid.Value > 0)
                    {
                        householdId = hid.Value;
                    }
                    else
                    {
                        var hh = await this.householdCodeService.GetAsync(a => a.AreaId == areaId && a.HouseName == houseName && a.HouseNumber == houseNumber && a.IsDeleted == 0);
                        if (hh != null)
                        {
                            householdId = hh.Id;
                            householdIdList.Add(houseName + houseNumber, hh.Id);
                        }
                    }
                }


                if (string.IsNullOrWhiteSpace(realName) && string.IsNullOrWhiteSpace(idCard))
                {
                    // failCount += 1;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(realName) || string.IsNullOrWhiteSpace(idCard))
                {
                    importResult.FailCount += 1;
                    importResult.Errors.Add(new ImportResultMessage() { Row = i, Description = "数据错误", DetailError = "请输入姓名或身份证" });
                    continue;
                }

                var idCardInfo = Utils.ValidIdCard(idCard);
                if (idCardInfo == null)
                {
                    importResult.FailCount += 1;
                    importResult.Errors.Add(new ImportResultMessage() { Row = i, Description = "数据错误", DetailError = "身份证格式不合法" });
                    continue;
                }


                var registers = Utils.SplitAddress(registerAddress);
                var lives = Utils.SplitAddress(liveAddress);

                var data = new PopulationDetailDto()
                {
                    AreaId = areaId,
                    RealName = realName,
                    IdCard = idCard,
                    Sex = sex == "男" ? PopulationGender.男 : PopulationGender.女,
                    Birthday = idCardInfo.Birthday.ToString("yyyy-MM-dd"),
                    Mobile = mobile,
                    Relationship = DictionaryGetCode(relationship, dictionarys, 1010, false),
                    Marital = DictionaryGetCode(marital, dictionarys, 1014, false),
                    Political = DictionaryGetCode(political, dictionarys, 1015, false),
                    Nation = DictionaryGetCode(nation, dictionarys, 1013, false),
                    Religion = DictionaryGetCode(religion, dictionarys, 1017, false),
                    Education = DictionaryGetCode(education, dictionarys, 1016, false),
                    Tags = DictionaryGetCode(tags, dictionarys, 1012, true),
                    Income = DictionaryGetCode(income, dictionarys, 1011, false),
                    Remark = remark,
                    Status = 1,
                    HouseholdId = householdId,
                    RegisterAddressInfo = new PopulationAddressDto() { Province = registers[0], City = registers[1], District = registers[2], Address = registers[3] },
                    LiveAddressInfo = new PopulationAddressDto() { Province = lives[0], City = lives[1], District = lives[2], Address = lives[3] },
                };

                var res = await this.SavePopulation(data);
                if (res.Flag)
                {
                    importResult.SuccessCount += 1;
                }
                else
                {
                    importResult.FailCount += 1;
                    importResult.Errors.Add(new ImportResultMessage() { Row = i, Description = "数据错误", DetailError = res.Message });
                }
                i++;
            }
            result.Flag = true;
            result.Data = importResult;
            return result;
        }

        /// <summary>
        /// 导入户籍人口
        /// </summary>
        /// <param name="excelFile"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public async Task<string> ImportPopulationAndHouseName(Stream fileStream, int areaId)
        {
            if (areaId <= 0)
            {
                return "缺少必要参数";
            }

            DataSet dataset = Utils.ImportExcel(fileStream, 5, 10);
            int cnt = dataset.Tables.Count;
            if (cnt == 0)
            {
                return "无任何数据可导入";
            }
            int count = 0;
            int failCount = 0;
            Dictionary<string, int> householdIdList = new Dictionary<string, int>();
            foreach (DataRow row in dataset.Tables[0].Rows)
            {
                int householdId = 0;

                string realName = row.ItemArray[0].ToString().Trim();
                string sex = row.ItemArray[1].ToString().Trim();
                string idCard = row.ItemArray[2].ToString().Trim();
                // string birthDay = row.ItemArray[3].ToString().Trim();
                string mobile = row.ItemArray[4].ToString().Trim();
                string registerAddress = row.ItemArray[5].ToString().Trim();
                string liveAddress = row.ItemArray[6].ToString().Trim();
                string remark = row.ItemArray[7].ToString().Trim();
                string houseName = row.ItemArray[8]?.ToString().Trim();
                string houseNumber = row.ItemArray[9]?.ToString().Trim();

                if (!string.IsNullOrWhiteSpace(houseName) && !string.IsNullOrWhiteSpace(houseNumber))
                {
                    var hid = householdIdList.FirstOrDefault(a => a.Key == houseName + houseNumber);

                    if (hid.Value > 0)
                    {
                        householdId = hid.Value;
                    }
                    else
                    {
                        var hh = await this.householdCodeService.GetAsync(a => a.AreaId == areaId && a.HouseName == houseName && a.HouseNumber == houseNumber && a.IsDeleted == 0);
                        if (hh != null)
                        {
                            householdId = hh.Id;
                            householdIdList.Add(houseName + houseNumber, hh.Id);
                        }
                    }
                }


                if (string.IsNullOrWhiteSpace(realName) && string.IsNullOrWhiteSpace(idCard))
                {
                    // failCount += 1;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(realName) || string.IsNullOrWhiteSpace(idCard))
                {
                    failCount += 1;
                    continue;
                }

                var idCardInfo = Utils.ValidIdCard(idCard);
                if (idCardInfo == null)
                {
                    failCount += 1;
                    continue;
                }


                var registers = Utils.SplitAddress(registerAddress);
                var lives = Utils.SplitAddress(liveAddress);

                var data = new PopulationDetailDto()
                {
                    AreaId = areaId,
                    RealName = realName,
                    IdCard = idCard,
                    Sex = sex == "男" ? PopulationGender.男 : PopulationGender.女,
                    Birthday = idCardInfo.Birthday.ToString("yyyy-MM-dd"),
                    Mobile = mobile,
                    Remark = remark,
                    Status = 1,
                    HouseholdId = householdId,
                    RegisterAddressInfo = new PopulationAddressDto() { Province = registers[0], City = registers[1], District = registers[2], Address = registers[3] },
                    LiveAddressInfo = new PopulationAddressDto() { Province = lives[0], City = lives[1], District = lives[2], Address = lives[3] },
                };

                var res = await this.SavePopulation(data);
                if (res.Flag)
                {
                    count += 1;
                }
                else
                {
                    failCount += 1;
                }
            }
            return $"成功导入{count}条信息，失败导入{failCount}条信息";
        }

        public async Task<List<ExportPopulationDto>> ExportPopulation(string ids)
        {
            string sql = string.Format(@" SELECT p.id,p.realName,p.sex,p.idCard,p.mobile,p.birthday, 
                            CONCAT(a1.province,a1.city,a1.district,a1.address) as registerAddress, 
                            CONCAT(a2.province,a2.city,a2.district,a2.address) as liveAddress,
                            p.remark
                            FROM VillagePopulation p
                            LEFT JOIN VillagePopulationAddress a1 on p.id = a1.populationId AND a1.type=1
                            LEFT JOIN VillagePopulationAddress a2 on p.id = a2.populationId AND a2.type=2
                            WHERE p.isDeleted=0 AND p.id in({0})", ids.Replace("-", "").Replace("delete", "").Replace("drop", "").Replace("update", ""));

            var data = this.Context.Database.SqlQuery<ExportPopulationDto>(sql);

            foreach (var item in data)
            {
                item.RealName = BasicSO.Decrypt(item.RealName);
                item.IdCard = BasicSO.Decrypt(item.IdCard);
                item.Mobile = BasicSO.Decrypt(item.Mobile);
            }
            return data;
        }


        /// <summary>
        /// 获取户主信息
        /// </summary>
        /// <param name="householdId"></param>
        /// <returns></returns>
        public async Task<VillagePopulation> GetHouseholderInfo(int householdId)
        {
            var member = await this.memberService.GetAsync(a => a.IsDeleted == 0 && a.HouseholdId == householdId && a.IsHouseholder == 1);
            if (member == null)
            {
                return null;
            }
            var population = await this.GetQueryable().AsNoTracking().FirstOrDefaultAsync(a => a.Id == member.PopulationId);// this.GetAsync(member.PopulationId);
            if (population != null)
            {
                population.RealName = BasicSO.Decrypt(population.RealName);
                population.IdCard = BasicSO.Decrypt(population.IdCard);
                population.Mobile = BasicSO.Decrypt(population.Mobile);
            }
            return population;
        }



        public async Task<List<KeyValueDto>> GetMembersSimple(int householdId)
        {

            var data = from m in this.memberService.GetQueryable()
                       join p in this.GetQueryable() on new { m.PopulationId, IsDeleted = 0 } equals new { PopulationId = p.Id, p.IsDeleted } into ptemp
                       from pp in ptemp.DefaultIfEmpty()
                       orderby m.IsHouseholder descending
                       where m.HouseholdId == householdId && m.IsDeleted == 0
                       select new KeyValueDto
                       {
                           Value = pp.Id,
                           Name = pp.RealName
                       };
            var list = await data.ToListAsync();

            foreach (var item in list)
            {
                item.Name = BasicSO.Decrypt(item.Name);
            }
            return list;
        }

        public async Task<List<UserHouseholdListRes>> GetHouseholdByUserIdAsync(int userId)
        {
            var basicUser = await _basicUserService.GetQueryable().Where(user => user.Id == userId).FirstOrDefaultAsync();
            if (basicUser != null && basicUser.PopulationId > 0)
            {
                List<VillageHouseholdCode> householdCodes = await GetHouseholdList(basicUser.PopulationId);
            IEnumerable< UserHouseholdListRes> households=    householdCodes.Select(code => new UserHouseholdListRes
                {
                    HouseName = code.HouseName,
                    HouseNumber = code.HouseNumber,
                    Id = code.Id,
                    IsCurrent=basicUser.HouseholdId==code.Id
                });
                return households.ToList();
            }
            else
            {
                return new List<UserHouseholdListRes>();
            }
        }

        public async Task<List<VillageHouseholdCode>> GetHouseholdList(int populationId)
        {

            var data = await this.memberService.GetListAsync(a => a.PopulationId == populationId && a.IsDeleted == 0);
            if (data == null)
            {
                return null;
            }

            var ids = data.Select(a => a.HouseholdId).Distinct().ToList();
            var list = await householdCodeService.GetListAsync(a => ids.Contains(a.Id) && a.IsDeleted == 0);

            var members = await this.memberService.GetListAsync(a => a.PopulationId == populationId && a.IsDeleted == 0 && a.IsHouseholder == 1);
            foreach (var item in list)
            {
                var id = item.Id;
                if (members != null)
                {
                    var member = members.FirstOrDefault(a => a.HouseholdId == id);
                    if (member != null)
                    {
                        var pop = await this.GetAsync(a => a.Id == member.PopulationId);
                        item.HouseholdMan = BasicSO.Decrypt(pop.RealName);
                        item.Mobile = BasicSO.Decrypt(pop.Mobile);
                        item.TagNames = await this.householdCodeTagService.GetTags(item.Id);
                        item.ImageUrls = this.sunFileService.ToAbsolutePath(item.ImageUrls);
                        item.HeadImageUrl = this.sunFileService.ToAbsolutePath(item.HeadImageUrl);
                    }
                }

            }
            return mapper.Map<List<VillageHouseholdCode>>(list);

        }


        public async Task<List<WechatMessagePopulationDto>> GetPopulationList(WechatMessagePopulationQuery query)
        {

            string householderSql = "";
            string areaSql = "";
            string houseNameSql = "";
            if (query.IsHouseholder > 0)
            {
                householderSql = " and m.isHouseholder=1 ";
            }
            //if (query.AreaId > 0)
            //{
            //    areaSql = " and u.areaId=" + query.AreaId;
            //}
            if (query.AreaId != null && query.AreaId.Length > 0)
            {
                areaSql = string.Format(" AND u.areaId in({0})", string.Join(',', query.AreaId));
            }

            if (query.HouseNameId != null && query.HouseNameId.Length > 0)
            {
                houseNameSql = " and h.houseNameId in(" + string.Join(',', query.HouseNameId) + ")";
            }

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"SELECT
                        p.realName,
                        p.mobile,
                        p.birthday,
                        p.sex,
                        p.education,
                        p.marital,
                        p.political,
                        p.income,
                        p.relationship,
						u.populationId,
                        u.account as openId,
						m.householdId,
						m.isHouseholder,
						h.areaId,
                        h.houseNameId,
                        h.houseName,
                        h.houseNumber
						FROM BasicUser u 
						INNER JOIN VillagePopulation p on u.populationId= p.id and p.IsDeleted=0
						LEFT JOIN VillageHouseCodeMember  m on m.populationId = p.id and m.isDeleted=0
						LEFT JOIN VillageHouseholdCode h on h.id= m.householdId and h.IsDeleted=0
						WHERE u.type=1 and u.populationId>0 and m.populationId>0  {0} {1} {2} ", householderSql, areaSql, houseNameSql);

            int year = DateTime.Now.Year;
            if (query.BeginAge > 0 && query.EndAge > 0)
            {
                sql.AppendFormat(" AND p.birthday BETWEEN '{0}-01-01' AND '{1}-12-31'", year - query.EndAge, year - query.BeginAge);
            }

            if (query.Sex > 0)
            {
                sql.AppendFormat(" AND p.sex={0}", query.Sex);
            }
            if (query.Education != null && query.Education.Length > 0)
            {
                sql.AppendFormat(" AND p.education in({0})", string.Join(',', query.Education));
            }

            if (query.Marital != null && query.Marital.Length > 0)
            {
                sql.AppendFormat(" AND p.marital in({0})", string.Join(',', query.Marital));
            }

            if (query.Political != null && query.Political.Length > 0)
            {
                sql.AppendFormat(" AND p.political in({0})", string.Join(',', query.Political));
            }
            if (query.Income != null && query.Income.Length > 0)
            {
                sql.AppendFormat(" AND p.income in({0})", string.Join(',', query.Income));
            }

            if (query.Relationship != null && query.Relationship.Length > 0)
            {
                sql.AppendFormat(" AND p.relationship in({0}) ", string.Join(',', query.Relationship));
            }

            var data = await this.Context.Database.SqlQueryAsync<WechatMessagePopulationDto>(sql.ToString());
            foreach (var item in data)
            {
                item.Mobile = BasicSO.Decrypt(item.Mobile);
            }
            return data;
        }


    }
}