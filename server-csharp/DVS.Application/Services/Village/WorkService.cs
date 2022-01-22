using AutoMapper;
using DVS.Core.Domains.Common;
using DVS.Core.Domains.Village;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using X.PagedList;
using Lychee.Extensions;
using LinqKit;
using DVS.Models.Dtos.Village.Query;
using DVS.Common.Services;
using DVS.Common.Infrastructures;
using DVS.Models.Dtos.Village;
using DVS.Application.Services.Common;
using DVS.Common.SO;
using DVS.Models.Enum;
using DVS.Models.Dtos.Common;

namespace DVS.Application.Services.Village
{
    public class WorkService : ServiceBase<VillageWork>, IWorkService
    {

        readonly int isDeleted = 0;
        readonly int isHouseholder = 1;
        private readonly ISunFileInfoService sunFileInfoService;
        private readonly IBasicDictionaryService basicDictionaryService;
        private readonly IPopulationAddressService populationAddressService;
        private readonly IServiceBase<VillageLatestTime> latestTimeService;
        public WorkService(DbContext dbContext, IMapper mapper, ISunFileInfoService sunFileInfoService,
            IBasicDictionaryService basicDictionaryService,
            IPopulationAddressService populationAddressService,
            IServiceBase<VillageLatestTime> latestTimeService
            )
            : base(dbContext, mapper)
        {
            this.sunFileInfoService = sunFileInfoService;
            this.basicDictionaryService = basicDictionaryService;
            this.populationAddressService = populationAddressService;
            this.latestTimeService = latestTimeService;
        }

        private async Task<List<VillageWorkInfoDto>> GetWorkInfoList(int householdId = 0, int year = 0, int populationId = 0)
        {
            StringBuilder sb = new StringBuilder(@"SELECT 
                                        w.*,
                                        p.realName,
                                        p.sex,
                                        p.relationship,
                                        p.mobile,
                                        p.headImageUrl,
                                        p.id as populationId
                                        FROM VillageWork w
                                        LEFT JOIN VillagePopulation p 
                                        on w.populationId= p.id where  w.IsDeleted=0  ");

            if (householdId > 0)
            {
                sb.Append(" and w.householdId=" + householdId);
            }
            if (year > 0)
            {
                sb.Append(" and w.year=" + year);
            }
            if (populationId > 0)
            {
                sb.Append(" and w.populationId=" + populationId);
            }
            sb.Append(" ORDER BY w.year desc,w.createdAt DESC ");
            var data = this.Context.Database.SqlQuery<VillageWorkInfoDto>(sb.ToString());
            List<string> dirs = new List<string>();
            foreach (var item in data)
            {
                item.HeadImageUrl = this.sunFileInfoService.ToAbsolutePath(item.HeadImageUrl);
                item.RealName = BasicSO.Decrypt(item.RealName);
                item.Mobile = BasicSO.Decrypt(item.Mobile);
                dirs.Add(item.Relationship);
                dirs.Add(item.Salary);
                dirs.Add(item.Industry);
                dirs.Add(item.Occupation);
            }
            var dirNames = await this.basicDictionaryService.GetBasicDictionaryCodeList(dirs);
            foreach (var item in data)
            {
                _ = int.TryParse(item.Salary, out int salaryId);
                var dic = dirNames.FirstOrDefault(a => a.Code == salaryId);
                if (dic != null)
                {
                    item.SalaryDto = mapper.Map<BasicDictionaryDto>(dic);
                }
                _ = int.TryParse(item.Industry, out int industryId);
                dic = dirNames.FirstOrDefault(a => a.Code == industryId);
                if (dic != null)
                {
                    item.IndustryDto = mapper.Map<BasicDictionaryDto>(dic);
                }
                _ = int.TryParse(item.Occupation, out int occupationId);
                dic = dirNames.FirstOrDefault(a => a.Code == occupationId);
                if (dic != null)
                {
                    item.OccupationDto = mapper.Map<BasicDictionaryDto>(dic);
                }

                item.Relationship = await this.basicDictionaryService.GetNameByCode(item.Relationship, dirNames);
                item.Salary = await this.basicDictionaryService.GetNameByCode(item.Salary, dirNames);
                item.Industry = await this.basicDictionaryService.GetNameByCode(item.Industry, dirNames);
                item.Occupation = await this.basicDictionaryService.GetNameByCode(item.Occupation, dirNames);
            }

            return data;
        }
        public async Task<List<VillageWorkInfoDto>> GetWorkInfoList(int householdId, int year)
        {
            return await GetWorkInfoList(householdId, year, 0);
        }
        public async Task<List<VillageWorkInfoDto>> GetWorkInfoList(int populationId)
        {
            return await GetWorkInfoList(0, 0, populationId);
        }

        public async Task<IPagedList<VillageWorkDto>> GetWorkList(PagePostBody body, string ids = "")
        {
            string sqlWhere = "";
            if (!string.IsNullOrWhiteSpace(body.Keyword))
            {
                sqlWhere = string.Format(" and (p.realName like '%{0}%' or h.houseName  LIKE '%{0}%'  or h.houseNumber  LIKE '%{0}%') ", body.Keyword);
            }

            if (!string.IsNullOrWhiteSpace(ids)) {
                sqlWhere += $" and h.id in({ids})";
            }

            string sql = @"SELECT 
                                    h.id,
                                    h.areaId,
                                    h.houseName,
                                    h.houseNumber,
                                    p.realName as householdMan,
                                    IFNULL(p.sex,0) as sex,
                                    p.nation, 
                                    p.mobile,
                                    a.`name` as areaName,
                                    l.updatedAt,
                                    IFNULL(l.peopleCount,0) as peopleCount,
                                    IFNULL(n.sequence,0) as houseNameSequence
                                    FROM VillageHouseholdCode h
                                    LEFT JOIN VillageHouseCodeMember m on h.id=m.householdId and m.isHouseholder=1 and m.IsDeleted=0
                                    LEFT JOIN VillageLatestTime l on h.id= l.householdId and l.tableType=1 and l.year={0}
                                    LEFT JOIN VillagePopulation p on m.populationId= p.id and p.IsDeleted=0
                                    LEFT JOIN BasicArea a on a.id = h.areaId  and a.IsDeleted=0
                                    LEFT JOIN VillageHouseName n on n.id=h.houseNameId  and n.IsDeleted=0
                                    WHERE l.peopleCount>0 and h.IsDeleted=0 and h.areaId={1} {2}";

            sql = string.Format(sql, body.Year, body.AreaId, sqlWhere);
            string sqlForCount = @"
                                    SELECT 
                                    h.id,
                                    h.houseName,
                                    h.houseNumber,
                                    p.realName as householdMan
                                    FROM VillageHouseholdCode h
                                    LEFT JOIN VillageHouseCodeMember m on h.id=m.householdId and m.isHouseholder=1 and m.IsDeleted=0
                                    LEFT JOIN VillageLatestTime l on h.id= l.householdId and l.tableType=1 and l.year={0}
                                    LEFT JOIN VillagePopulation p on m.populationId= p.id and p.IsDeleted=0
                                    WHERE l.peopleCount>0 and h.IsDeleted=0 and h.areaId={1} {2} ";
            sqlForCount = string.Format(sqlForCount, body.Year, body.AreaId, sqlWhere);
            var pageData = this.Context.Database.SqlQueryPagedList<VillageWorkDto>(body.Page, body.Limit, sql, sqlForCount, this.Context.Database.GetOrderBySql(body.Orders));
            List<string> dirs = new List<string>();
            foreach (var item in pageData)
            {
                item.HouseholdMan = BasicSO.Decrypt(item.HouseholdMan);
                item.Mobile = BasicSO.Decrypt(item.Mobile);
                dirs.Add(item.Nation);
            }
            var dirNames = await this.basicDictionaryService.GetBasicDictionaryCodeList(dirs);
            foreach (var item in pageData)
            {
                item.Nation = await this.basicDictionaryService.GetNameByCode(item.Nation, dirNames);
            }
            return pageData;
        }



        public async Task<VillageWorkInfoDto> GetWorkInfoDetail(int id)
        {
            StringBuilder sb = new StringBuilder(@"SELECT 
                                        w.*,
                                        p.realName,
                                        p.sex,
                                        p.relationship,
                                        p.mobile,
                                        p.headImageUrl,
                                        p.id as populationId,
                                        null as WorkAddressInfo
                                        FROM VillageWork w
                                        LEFT JOIN VillagePopulation p 
                                        on w.populationId= p.id where  w.IsDeleted=0 and w.id=" + id);

            var datas = await this.Context.Database.SqlQueryAsync<VillageWorkInfoDto>(sb.ToString());
            if (datas.Count() > 0)
            {
                var data = datas.FirstOrDefault();
                data.HeadImageUrl = this.sunFileInfoService.ToAbsolutePath(data.HeadImageUrl);
                data.RealName = BasicSO.Decrypt(data.RealName);
                data.Mobile = BasicSO.Decrypt(data.Mobile);
                data.Relationship = await this.basicDictionaryService.GetNameByCode(data.Relationship);
                data.WorkAddressInfo = await this.populationAddressService.GetAddressDetail(data.Id, PopulationAddressTypeEnum.工作地);
                return data;
            }
            return null;
        }


        /// <summary>
        /// 设置最新操作时间
        /// </summary>
        /// <param name="householdId"></param>
        /// <returns></returns>
        private async Task<bool> SetLatestTime(int householdId, long year)
        {

            int tableType = 1;
            int peopleCount = await this.CountAsync(a => a.HouseholdId == householdId && a.Year == year && a.IsDeleted == 0);
            var data = await this.latestTimeService.GetAsync(a => a.HouseholdId == householdId && a.Year == year && a.TableType == tableType);
            if (data == null)
            {
                var res = await this.latestTimeService.InsertAsync(new VillageLatestTime()
                {
                    HouseholdId = householdId,
                    TableType = tableType,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    PeopleCount = peopleCount,
                    Year = year,

                });
                return res != null;
            }
            else
            {
                var res = await this.latestTimeService.Context.Database.ExecuteSqlRawAsync("update VillageLatestTime set UpdatedAt={0}, PeopleCount={1} where HouseholdId={2} and Year={3}", DateTime.Now, peopleCount, householdId, year);
                return res > 0;
            }
        }

        public async Task<MessageResult<bool>> SaveWorkInfo(SaveWorkBody body)
        {
            var result = new MessageResult<bool>("失败", false, false);
            if (body.HouseholdId <= 0)
            {
                result.Message = "请输入户码Id";
                return result;
            }

            if (body.PopulationId <= 0)
            {
                result.Message = "请输入人口Id";
                return result;
            }

            if (body.Year <= 0)
            {
                body.Year = DateTime.Now.Year;
            }
            string workOrg = $"{body.WorkAddressInfo.Province},{body.WorkAddressInfo.City},{body.WorkAddressInfo.District}";
            string workAddress = body.WorkAddressInfo.Address;
            if (body.Id > 0)
            {
                var data = await this.GetAsync(body.Id);
                var res = await this.GetQueryable().Where(a => a.Id == body.Id)
                    .UpdateFromQueryAsync(a => new VillageWork()
                    {
                        WorkOrgCodes = workOrg,
                        WorkAddress = workAddress,
                        HouseholdId = body.HouseholdId,
                        AreaId= body.AreaId,
                        PopulationId = body.PopulationId,
                        Year = body.Year,
                        Industry = body.Industry,
                        Salary = body.Salary,
                        Company = body.Company,
                        UpdatedAt = DateTime.Now,
                        Occupation = body.Occupation,
                        IsSync = 0,
                    });
                if (res > 0)
                {
                    await this.populationAddressService.SaveAddress(body.Id, PopulationAddressTypeEnum.工作地, body.WorkAddressInfo);
                    result.Message = "成功";
                    result.Flag = true;
                    result.Data = true;
                    await SetLatestTime(body.HouseholdId, body.Year);

                    if (data != null && data.Year != body.Year) {
                        // 再计算一下被改的那一年
                        await SetLatestTime(data.HouseholdId, data.Year);
                    }

                    return result;
                }
            }
            else
            {
                var ddd = mapper.Map<VillageWork>(body);
                ddd.CreatedAt = DateTime.Now;
                ddd.WorkOrgCodes = workOrg;
                ddd.WorkAddress = workAddress;
                var res = await this.InsertAsync(ddd);
                if (res != null)
                {
                    body.WorkAddressInfo.Id = 0;
                    await this.populationAddressService.SaveAddress(res.Id, PopulationAddressTypeEnum.工作地, body.WorkAddressInfo);
                    result.Message = "成功";
                    result.Flag = true;
                    result.Data = true;
                    await SetLatestTime(body.HouseholdId, body.Year);
                    return result;
                }
            }
            return result;
        }


        public async Task<bool> DeleteWrokInfo(int id) {

            var data = await this.GetAsync(id);
            if (data == null) {
                return false;
            }
            var res = await this.Context.Database.ExecuteSqlRawAsync("update VillageWork set isDeleted=1, isSync = 0 where id={0} ", id);
            if (res > 0)
            {
                await SetLatestTime(data.HouseholdId, data.Year);
            }
            return res > 0;
        }
    
    }
}
