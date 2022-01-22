using DVS.Application.Services.Common;
using DVS.Application.Services.Village;
using DVS.Common.Services;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.Village.Statistics;
using DVS.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DVS.Village.Api.Controllers
{
    /// <summary>
    /// 后台首页乡村治理相关统计
    /// </summary>
    [Route("/api/village/console/statistics")]
    public class StatisticsController : DvsControllerBase
    {
        private readonly IPopulationService _populationService;
        private readonly IBasicAreaService basicAreaService;

        public StatisticsController(IPopulationService populationService, IBasicAreaService basicAreaService)
        {
            this._populationService = populationService;
            this.basicAreaService = basicAreaService;
        }

        /// <summary>
        /// 人口概况 男女比例
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        [HttpGet("GetPopuSexPer")]
        public async Task<IEnumerable<StatisticsCommonDto>> GetPopuSexPer(int areaId)
        {
            var areaIds = await basicAreaService.FindChildrenAreaIds(areaId);
            return await this._populationService.GetQueryable().Where(a => areaIds.Contains(a.AreaId) && a.IsDeleted == 0).GroupBy(a => a.Sex).Select(a => new StatisticsCommonDto
            {
                Name = a.Key.ToString(),
                Value = a.Count()
            }).ToListAsync();
        }

        /// <summary>
        /// 人口概况 人口类型
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        [HttpGet("GetPopulationTypeBar")]
        public async Task<IEnumerable<StatisticsPopulationTypeDto>> GetPopulationTypeBar(int areaId)
        {
            var areaIds = await basicAreaService.FindChildrenAreaIds(areaId);
            //return await this._populationService.GetQueryable().Where(a => a.AreaId == areaId && a.IsDeleted == 0).GroupBy(a => a.PopulationType)
            //     .Select(a => new StatisticsPopulationTypeDto
            //     {
            //         Name = a.Key.ToString(),
            //         FemaleVaule = a.Count(c => c.Sex == PopulationGender.女),
            //         MaleValue = a.Count(c => c.Sex == PopulationGender.男)
            //     }).ToListAsync();

            var data = this._populationService.Context.Database.SqlQuery<StatisticsPopulationTypeDto>(string.Format(@"SELECT t0.populationType,
                                                        t0.name,
                                                        IFNULL(t1.MaleValue,0) as maleValue,
                                                        IFNULL(t2.FemaleVaule,0) as femaleVaule FROM
                                                        (SELECT 1 as populationType,'户籍人口' as `name`
                                                        UNION
                                                        SELECT 2 as populationType,'流动人口' as `name`
                                                        UNION
                                                        SELECT 3 as populationType,'外籍人口' as `name`)t0
                                                        LEFT JOIN                   
                                                        (SELECT 
                                                        p.populationType,
                                                        COUNT(p.populationType) as MaleValue 
                                                        FROM VillagePopulation p 
                                                        WHERE p.isDeleted=0 and p.sex=1 and p.areaId in({0})  GROUP BY p.populationType)t1
                                                        on t0.populationType= t1.populationType
                                                         LEFT JOIN
                                                        (SELECT 
                                                        p.populationType,
                                                        COUNT(p.populationType) as FemaleVaule 
                                                        FROM VillagePopulation p 
                                                        WHERE p.isDeleted=0 and p.sex=2 and p.areaId in({0})  GROUP BY p.populationType)t2
                                                        on  t0.populationType= t2.populationType", string.Join(',', areaIds.ToArray())));

            return data;
        }

        /// <summary>
        /// 农村居民平均年收入
        /// </summary>
        /// <param name="service"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        [HttpGet("GetYearIncome")]
        public async Task<IEnumerable<StatisticsCommonDto>> GetYearIncome([FromServices] IServiceBase<VillageIncome> service, int areaId)
        {
            var areaIds = await basicAreaService.FindChildrenAreaIds(areaId);
            return await service.GetQueryable().Where(a => areaIds.Contains(a.AreaId) && a.IsDeleted == 0)
                .GroupBy(a => a.Year).OrderBy(a => a.Key).Select(a => new StatisticsCommonDto
                {
                    Name = a.Key.ToString(),
                    Value =(long) a.Average(c => c.Product + c.HouseRental + c.CollectiveDividend + c.Distribution + c.LandCirculation + c.WorkIncome + c.GovSubsidy + c.Other)
                }).ToListAsync();
        }

        /// <summary>
        /// 乡村治理荣誉
        /// </summary>
        /// <param name="service"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        [HttpGet("GetVillageHonor")]
        public async Task<IEnumerable<StatisticsCommonDto>> GetVillageHonor([FromServices] IHouseholdCodeService service, int areaId)
        {
            return await service.GetVillageHonor(areaId);
        }

        /// <summary>
        /// 外出务工情况
        /// </summary>
        /// <param name="service"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        [HttpGet("GetOutWorkInfo")]
        public async Task<IEnumerable<StatisticsCommonDto>> GetOutWorkInfo([FromServices] IServiceBase<VillageWork> service, int areaId)
        {
            var areaIds = await basicAreaService.FindChildrenAreaIds(areaId);
            //var query = from w in service.GetQueryable()
            //            join p in this._populationService.GetQueryable() on w.PopulationId equals p.Id
            //            where areaIds.Contains(w.AreaId) && w.IsDeleted == 0 && p.IsDeleted == 0
            //            group p by new p.Sex into g
            //            select new StatisticsCommonDto
            //            {
            //                Name = g.Key.ToString(),
            //                Value = g.Count()
            //            };
            var sql = string.Format(@"SELECT CAST(t.sex as char) AS `name`,CAST(COUNT(*) AS signed) AS `value` FROM
                        (SELECT p.sex, w.populationId
                        FROM VillageWork AS w
                        INNER JOIN VillagePopulation AS p ON w.PopulationId = p.Id
                        WHERE (w.areaId IN ({0}) AND (w.IsDeleted = 0)) AND (p.IsDeleted = 0)
                        GROUP BY w.populationId)t
                        GROUP BY t.sex", string.Join(',', areaIds.ToArray()));
            var data = await service.Context.Database.SqlQueryAsync<StatisticsCommonDto>(sql);

            foreach (var item in data)
            {
                if (item.Name == "1")
                {
                    item.Name = "男";
                }
                else if (item.Name == "2")
                {
                    item.Name = "女";
                }
                else
                {
                    item.Name = "其他";
                }

            }
            return data;
        }

        /// <summary>
        /// 乡村治理居民参与情况 男女
        /// </summary>
        /// <param name="service"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        [HttpGet("GetVillagePartakeSex")]
        public async Task<IEnumerable<StatisticsCommonDto>> GetVillagePartakeSex([FromServices] IPopulationService service, int areaId)
        {
            return await service.GetVillagePartakeSex(areaId);
        }

        /// <summary>
        /// 乡村治理居民参与情况 年龄段
        /// </summary>
        /// <param name="service"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        [HttpGet("GetVillagePartakeAge")]
        public async Task<IEnumerable<StatisticsCommonDto>> GetVillagePartakeAge([FromServices] IPopulationService service, int areaId)
        {
            return await service.GetVillagePartakeAge(areaId);
        }
    }
}