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
using DVS.Common.Services;
using DVS.Common.Infrastructures;
using DVS.Models.Dtos.Village.Query;
using DVS.Models.Dtos.Village;
using DVS.Common.SO;

namespace DVS.Application.Services.Village
{
    public class IncomeService : ServiceBase<VillageIncome>, IIncomeService
    {

        readonly int isDeleted = 0;
        readonly int isHouseholder = 1;
        public IncomeService(DbContext dbContext, IMapper mapper
            )
            : base(dbContext, mapper)
        {

        }

        public async Task<VillageIncome> GetIncomeDetail(int householdId, int year)
        {
            var data = await this.GetAsync(a => a.HouseholdId == householdId && a.Year == year && a.IsDeleted == 0);
            if (data == null) {
                data = new VillageIncome();
            }
            return data;
        }




        public async Task<IPagedList<VillageIncomeDto>> GetIncomeList(PagePostBody body,string ids="")
        {
            string sqlWhere = "";

            if (!string.IsNullOrWhiteSpace(body.Keyword))
            {
                sqlWhere = string.Format(" and (p.realName like '%{0}%' or h.houseName  LIKE '%{0}%'  or h.houseNumber  LIKE '%{0}%' ) ", body.Keyword);
            }

            if (!string.IsNullOrWhiteSpace(ids))
            {
                sqlWhere += $" and i.id in ({ids})";
            }

            string sql = string.Format(@" SELECT 
										i.id,
										i.householdId,
										i.areaId,
										i.totalIncome,
										i.`year`,
										h.houseName,
										h.houseNumber,
										h.HouseNameSequence,
										p.realName as householdMan,
										p.mobile,
										'' as areaName
										FROM VillageIncome i 
										LEFT JOIN VillageHouseholdCode h on i.householdId = h.id
										LEFT JOIN VillageHouseCodeMember m on i.householdId = m.householdId and m.isHouseholder=1 and m.isDeleted=0
										LEFT JOIN VillagePopulation p on m.populationId = p.id
										WHERE i.`year`={0} and i.areaId={1} and i.IsDeleted=0  {2} ", body.Year, body.AreaId, sqlWhere);

            string sqlForCount = string.Format(@" SELECT 
										i.id
										FROM VillageIncome i 
										LEFT JOIN VillageHouseholdCode h on i.householdId = h.id
										LEFT JOIN VillageHouseCodeMember m on i.householdId = m.householdId and m.isHouseholder=1 and m.isDeleted=0
										LEFT JOIN VillagePopulation p on m.populationId = p.id
										WHERE i.`year`={0} and i.areaId={1} and i.IsDeleted=0  {2} ", body.Year, body.AreaId, sqlWhere);
            var pageData = this.Context.Database.SqlQueryPagedList<VillageIncomeDto>(body.Page, body.Limit, sql, "", this.Context.Database.GetOrderBySql(body.Orders));

            foreach (var item in pageData)
            {
                item.HouseholdMan = BasicSO.Decrypt(item.HouseholdMan);
                item.Mobile = BasicSO.Decrypt(item.Mobile);
            }

            return pageData;
        }

        public async Task<MessageResult<int>> SaveIncome(SaveIncomeBody body)
        {
            var result = new MessageResult<int>("失败", false, 0);

            if (body.HouseholdId <= 0)
            {
                result.Message = "请传入户码Id";
                return result;
            }
            if (body.Year <= 0)
            {
                result.Message = "请选择年度";
                return result;
            }

            var income = await this.GetAsync(a => a.HouseholdId == body.HouseholdId && a.IsDeleted == isDeleted && a.Year == body.Year);
            if (income != null && body.Id <= 0)
            {
                result.Message = "本年度记录已存在";
                return result;
                // body.Id = income.Id;
            }

            if (body.Id > 0)
            {
                var res = await this.GetQueryable().Where(a => a.Id == body.Id && a.AreaId == body.AreaId && a.IsDeleted == 0)
                                .UpdateFromQueryAsync(a => new VillageIncome()
                                {
                                    Product = body.Product,
                                    HouseRental = body.HouseRental,
                                    CollectiveDividend = body.CollectiveDividend,
                                    Distribution = body.Distribution,
                                    LandCirculation = body.LandCirculation,
                                    WorkIncome = body.WorkIncome,
                                    GovSubsidy = body.GovSubsidy,
                                    Other = body.Other,
                                    TotalIncome = body.Product + body.HouseRental + body.CollectiveDividend + body.Distribution + body.LandCirculation + body.WorkIncome + body.GovSubsidy + body.Other,
                                    IsSync = 0,
                                });
                if (res > 0)
                {
                    result.Message = "成功";
                    result.Flag = true;
                    result.Data = body.Id;
                    return result;
                }
            }
            else
            {
                var post = mapper.Map<VillageIncome>(body);
                post.TotalIncome = post.Product + post.HouseRental + post.CollectiveDividend + post.Distribution + post.LandCirculation + post.WorkIncome + post.GovSubsidy + post.Other;
                var res = await this.InsertAsync(post);
                if (res != null)
                {
                    result.Message = "成功";
                    result.Flag = true;
                    result.Data = res.Id;
                    return result;
                }
            }

            return result;
        }


        public async Task<bool> DeleteIncome(int id, int householdId)
        {
            string sql = "update VillageIncome set isDeleted=1,isSync=0, updatedAt={0} where householdId={1} and id={2}";
            var res = await this.Context.Database.ExecuteSqlRawAsync(sql, DateTime.Now, householdId, id);
            return res > 0;
        }

    }
}
