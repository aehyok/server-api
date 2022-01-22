using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Application.Services.GIS;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Common.SO;
using DVS.Core.Domains.Common;
using DVS.Core.Domains.GIS;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Village.Farmland;
using DVS.Models.Dtos.Village.Statistics;
using DVS.Models.Enum;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.Village
{
    public class VillageFarmlandService : ServiceBase<VillageFarmland>, IVillageFarmlandService
    {
        IBasicDictionaryService basicDictionaryService;
        IBasicAreaService basicAreaService;
        IHouseholdCodeService householdCodeService;
        IPopulationService populationService;
        IGISPlotItemService gisPlotItemService;
        IServiceBase<VillageHouseCodeMember> memberService;
        public VillageFarmlandService(DbContext context, IMapper mapper,
            IBasicAreaService basicAreaService,
            IBasicDictionaryService basicDictionaryService,
            IHouseholdCodeService householdCodeService,
            IPopulationService populationService,
            IGISPlotItemService gisPlotItemService,
            IServiceBase<VillageHouseCodeMember> memberService
            ) : base(context, mapper)
        {
            this.basicAreaService = basicAreaService;
            this.basicDictionaryService = basicDictionaryService;
            this.householdCodeService = householdCodeService;
            this.populationService = populationService;
            this.gisPlotItemService = gisPlotItemService;
            this.memberService = memberService;
        }
        public async Task<VillageFarmlandDto> GetDetail(int id)
        {
            var query = from farmland in GetQueryable().Where(f => f.Id == id)
                        join householdCode in householdCodeService.GetQueryable() on farmland.HouseholdId equals householdCode.Id
                        into temp
                        from res in temp.DefaultIfEmpty()
                        join m in this.memberService.GetQueryable() on new { HouseholdId = res.Id, IsDeleted = 0, IsHouseholder = 1 } equals new { m.HouseholdId, m.IsDeleted, m.IsHouseholder } into mtemp
                        from mm in mtemp.DefaultIfEmpty()
                        join population in populationService.GetQueryable()
                        on mm.PopulationId equals population.Id
                        into result
                        from leftjoinResult in result.DefaultIfEmpty()
                        select new VillageFarmlandDto()
                        {
                            Id = farmland.Id,
                            AreaId = farmland.AreaId,
                            Name = farmland.Name,
                            Area = farmland.Area,
                            Unit = farmland.Unit,
                            TypeId = farmland.TypeId,
                            Address = farmland.Address,
                            HouseName = res.HouseName,
                            HouseNumber = res.HouseNumber,
                            Householder = BasicSO.Decrypt(leftjoinResult.RealName),
                            HouseholdId = farmland.HouseholdId,
                            Remark = farmland.Remark,
                            UseFor = farmland.UseFor,
                            IsDeleted = farmland.IsDeleted
                        };
            var farmlandInfo = await query.FirstOrDefaultAsync();
            if (farmlandInfo == null)
            {
                return null;
            }
            var dictionary = await this.basicDictionaryService.GetQueryable().Where(a => a.Code == farmlandInfo.TypeId && a.IsDeleted == 0).FirstOrDefaultAsync();
            var area = await this.basicAreaService.GetQueryable().Where(a => a.Id == farmlandInfo.AreaId).FirstOrDefaultAsync();
            farmlandInfo.AreaName = area == null ? "" : area.Name;
            farmlandInfo.TypeName = dictionary == null ? "" : dictionary.Name;

            int plotType = (int)PlotType.FARMLAND;
            if (farmlandInfo.UseFor == 2)
            {
                plotType = (int)PlotType.PLANLAND;
            }
            var plotItem = await gisPlotItemService.GetAsync(plot => plot.ObjectId == farmlandInfo.Id && plot.PlotType == plotType && plot.IsDeleted == 0);
            farmlandInfo.isPloted = plotItem != null ? true : false;
            return farmlandInfo;
        }

        /// <summary>
        /// 导出土地信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<byte[]> GetExcelData(List<int> ids, int areaId, string keyword = "")
        {
            if (areaId <= 0)
            {
                throw new ValidException("无效的行政区域id");
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();

            package.Workbook.Properties.Title = "地块信息";
            var workSheet = package.Workbook.Worksheets.Add("地块信息");
            // 表头
            workSheet.Cells[1, 1].Value = "社区/村";
            workSheet.Cells[1, 2].Value = "门牌号";
            workSheet.Cells[1, 3].Value = "门牌号";
            workSheet.Cells[1, 4].Value = "户主";



            // 获取地块类型
            IEnumerable<BasicDictionary> basicDictionaries = await basicDictionaryService.GetBasicDictionaryList(3010);
            int headIndex = 5;
            // key:列下标，value:typeId
            Dictionary<int, int> farmlandTypeMap = new Dictionary<int, int>();
            foreach (BasicDictionary dictionary in basicDictionaries)
            {
                workSheet.Cells[1, headIndex].Value = $"{dictionary.Name}({(dictionary.Name == "宅基地" ? "平方米" : "亩")})";
                farmlandTypeMap[headIndex] = dictionary.Code;
                headIndex++;
            }

            var farmlandQuery = GetQueryable().Where(f => f.IsDeleted == 0 && f.AreaId == areaId);
            if (ids != null && ids.Count > 0)
            {
                farmlandQuery = farmlandQuery.Where(f => ids.Contains(f.HouseholdId));
            }
            var query = from farmland in farmlandQuery
                        where farmland.HouseholdId>0
                        group farmland by new { HouseholdId = farmland.HouseholdId, AreaId = farmland.AreaId } into farmlandGroup
                        select new
                        {
                            HouseholdId = farmlandGroup.Key.HouseholdId,
                            AreaId = farmlandGroup.Key.AreaId
                        } into farmlandHousehold
                        join household in householdCodeService.GetQueryable()
                        on farmlandHousehold.HouseholdId equals household.Id into householdInfo
                        from houdeholdInfoResult in householdInfo.DefaultIfEmpty()
                        join m in this.memberService.GetQueryable() on new { HouseholdId = houdeholdInfoResult.Id, IsDeleted = 0, IsHouseholder = 1 } equals new { m.HouseholdId, m.IsDeleted, m.IsHouseholder } into mtemp
                        from mm in mtemp.DefaultIfEmpty()
                        join population in populationService.GetQueryable()
                        on new { Id = mm.PopulationId, IsDeleted = 0 } equals new { population.Id, population.IsDeleted } into householder
                        from result in householder.DefaultIfEmpty()
                        select new VillageFarmlandDto()
                        {
                            HouseholdId = farmlandHousehold.HouseholdId,
                            HouseName = houdeholdInfoResult.HouseName,
                            HouseNumber = houdeholdInfoResult.HouseNumber,
                            AreaId = farmlandHousehold.AreaId,
                            Householder = result.RealName
                        };


            if (!keyword.IsNullOrWhiteSpace())
            {
                query = query.Where(item => (EF.Functions.Like(item.HouseName, $"%{keyword}%") || EF.Functions.Like(item.Householder, $"%{keyword}%") || EF.Functions.Like(item.HouseNumber, $"%{keyword}%")));
            }



            List<VillageFarmlandDto> farmlands = await query.ToListAsync();
            for (int i = 0; i < farmlands.Count; i++)
            {
                int rowIndex = i + 2;
                VillageFarmlandDto farmLand = farmlands[i];

                var dictionary = await this.basicDictionaryService.GetQueryable().Where(a => a.Code == farmLand.TypeId && a.IsDeleted == 0).FirstOrDefaultAsync();
                var area = await this.basicAreaService.GetQueryable().Where(a => a.Id == farmLand.AreaId).FirstOrDefaultAsync();
                farmLand.AreaName = area == null ? "" : area.Name;
                farmLand.TypeName = dictionary == null ? "" : dictionary.Name;

                var queryFaramlandArea = from farmland in this.GetQueryable().Where(f => f.IsDeleted == 0 && f.HouseholdId == farmLand.HouseholdId && f.AreaId ==areaId)
                                         group farmland by new { HouseholdId = farmland.HouseholdId, TypeId = farmland.TypeId } into farmlandInfo
                                         select new { Area = farmlandInfo.Sum(f => f.Area), TypeId = farmlandInfo.Key.TypeId, HouseholdId = farmlandInfo.Key.HouseholdId };


                var areaInfos = queryFaramlandArea.ToList();

                workSheet.Cells[rowIndex, 1].Value = farmLand.AreaName ?? "集体用地";
                workSheet.Cells[rowIndex, 2].Value = farmLand.HouseName;
                workSheet.Cells[rowIndex, 3].Value = farmLand.HouseNumber;
                workSheet.Cells[rowIndex, 4].Value = BasicSO.Decrypt(farmLand.Householder);
                foreach (int key in farmlandTypeMap.Keys)
                {
                    var areaInfo = areaInfos.Find(a => a.HouseholdId == farmLand.HouseholdId && a.TypeId == farmlandTypeMap[key]);
                    workSheet.Cells[rowIndex, key].Value = areaInfo == null ? 0 : areaInfo.Area;
                }
            }
            return await package.GetAsByteArrayAsync();
        }


        public async Task<IPagedList<FarmlandAreaSummaryDto>> GetAreaFarmlands(int areaId, string keyword, int page = 1, int limit = 10, List<OrderBy> orderBy = null)
        {
            StringBuilder countBuilder = new StringBuilder(@$"
                                                           SELECT 
                                                              VillageFarmland.householdId
                                                              from VillageFarmland 
                                                              left join VillageHouseholdCode on VillageHouseholdCode.id =householdId   and VillageHouseholdCode.isDeleted=0
															  left join VillageHouseCodeMember on VillageHouseholdCode.id=VillageHouseCodeMember.householdId   and VillageHouseCodeMember.isHouseholder=1  and VillageHouseCodeMember.isDeleted=0 
                                                              left join VillagePopulation on VillageHouseCodeMember.populationId=VillagePopulation.id and VillagePopulation.isDeleted=0  
                                                              WHERE VillageFarmland.householdId>0 and VillageFarmland.areaId={areaId} and VillageFarmland.isDeleted=0 and VillageFarmland.category =1 and VillageFarmland.useFor = 1");

            StringBuilder queryBuilder = new StringBuilder(@$"SELECT 
                                                              VillageFarmland.householdId,
                                                              VillageFarmland.areaId,
                                                              VillageHouseholdCode.houseName,
                                                              VillageHouseholdCode.houseNumber,
                                                              VillagePopulation.realName as householder,
                                                              VillagePopulation.mobile,
                                                              IFNULL(VillageHouseholdCode.HouseNameSequence,0) as HouseNameSequence
                                                              from VillageFarmland 
                                                              left join VillageHouseholdCode on VillageHouseholdCode.id =householdId   and VillageHouseholdCode.isDeleted=0
															  left join VillageHouseCodeMember on VillageHouseholdCode.id=VillageHouseCodeMember.householdId  and VillageHouseCodeMember.isHouseholder=1  and VillageHouseCodeMember.isDeleted=0 
                                                              left join VillagePopulation on VillageHouseCodeMember.populationId=VillagePopulation.id  and VillagePopulation.isDeleted=0  
                                                              WHERE VillageFarmland.householdId>0 and VillageFarmland.areaId={areaId} and VillageFarmland.isDeleted=0 and VillageFarmland.category =1 and VillageFarmland.useFor = 1
                                                               ");
            StringBuilder where = new StringBuilder();
            if (!keyword.IsNullOrWhiteSpace())
            {
                where.Append($"  and ( VillagePopulation.RealName like '%{keyword}%' or VillageHouseholdCode.houseName like '%{keyword}%' or VillageHouseholdCode.houseNumber like '%{keyword}%' )");
            }
            if (where.Length > 0)
            {
                queryBuilder.Append(where);
                countBuilder.Append(where);
            }
            StringBuilder groupBuilder = new StringBuilder(" GROUP BY VillageFarmland.householdId,VillageFarmland.areaId ");
            StringBuilder orderByBuilder = new StringBuilder(this.Context.Database.GetOrderBySql(orderBy));
            queryBuilder.Append(groupBuilder);
            countBuilder.Append(groupBuilder);
            IPagedList<FarmlandAreaSummaryDto> pageInfo = this.Context.Database.SqlQueryPagedList<FarmlandAreaSummaryDto>(page, limit, queryBuilder.ToString(), countBuilder.ToString(), orderByBuilder.ToString());

            //IPagedList<int> pageAreaIds = pageInfo.Select(a => a.AreaId);
            //List<int> areaIds = pageAreaIds == null ? new List<int>() : pageAreaIds.ToList();
            //IPagedList<int> houseIds = pageInfo.Select(a => a.HouseholdId);
            //List<int> householdIds = houseIds == null ? new List<int>() : houseIds.ToList();

            List<int> areaIds = pageInfo.Select(a => a.AreaId).ToList();
            List<int> householdIds = pageInfo.Select(a => a.HouseholdId).ToList();
            IList<BasicArea> areas = await this.basicAreaService.GetListAsync(a => areaIds.Contains(a.Id));
            List<FarmlandAreaSummary> farmlandAreaSummaries = null;
            if (householdIds.Count > 0)
            {
                string householdIdsString = string.Join(',', householdIds);
                StringBuilder summarySQL = new StringBuilder(@$" SELECT sum(area) as area,areaId,typeId,unit,householdId from VillageFarmland WHERE householdId in ({householdIdsString}) and areaId={areaId} and VillageFarmland.isDeleted=0 GROUP BY typeId,householdId ");
                farmlandAreaSummaries = await this.Context.Database.SqlQueryAsync<FarmlandAreaSummary>(summarySQL.ToString());
                List<int> typeIds = farmlandAreaSummaries.Select(a => a.TypeId).ToList();
                IList<BasicDictionary> dictionaries = await this.basicDictionaryService.GetListAsync(a => typeIds.Contains(a.Code) && a.IsDeleted == 0);
                foreach (var item in farmlandAreaSummaries)
                {
                    BasicDictionary typeInfo = dictionaries.FirstOrDefault(a => a.Code == item.TypeId);
                    item.TypeName = typeInfo == null ? "" : typeInfo.Name;
                    item.TypeNameCode = typeInfo == null ? "" : typeInfo.Code.ToString();
                }
            }
            // 解密readName
            foreach (var item in pageInfo)
            {
                List<FarmlandAreaSummary> summaries = farmlandAreaSummaries.FindAll(a => a.HouseholdId == item.HouseholdId);
                var area = areas.FirstOrDefault(a => a.Id == item.AreaId);
                item.Householder = BasicSO.Decrypt(item.Householder);
                item.Mobile = BasicSO.Decrypt(item.Mobile);
                item.AreaName = area == null ? "集体用地" : (area.Name ?? "集体用地");
                item.Summarys = summaries;
            }
            return pageInfo;
        }

        public async Task<IPagedList<VillageFarmlandDto>> GetFarmlands(int areaId, int typeId, string keyword, int objectId = 0, int category = 1, int usefor = 1, int page = 1, int limit = 10, List<OrderBy> orders = null, string ids = "")
        {
            switch (usefor)
            {
                case 1:
                    return await GetNormalFarmlands(areaId, typeId, keyword, objectId, category, page, limit, orders, ids);
                default:
                    return await GetPlanLands(areaId, typeId, keyword, page, limit, orders, ids);
            }
        }

        private async Task<IPagedList<VillageFarmlandDto>> GetNormalFarmlands(int areaId, int typeId, string keyword, int objectId, int category, int page, int limit, List<OrderBy> orders = null, string ids = "")
        {
            // category int(2) NULL DEFAULT 1 COMMENT '地块所属类型 1 区域 ,2 园区 ',
            // useFor int(2) NULL DEFAULT 1 COMMENT '地块用途 1 普通用地 ,2 规划用地 ',
            switch (category)
            {
                case 1:
                    return await GetHouseholdFarmlands(areaId, typeId, keyword, page, limit, objectId, orders, ids);
                case 2:
                    return await GetParkFarmlands(areaId, typeId, keyword, page, limit, objectId, orders, ids);
                default:
                    return null;
            }
        }
        /// <summary>
        /// 获取园区土地列表
        /// </summary>
        /// <param name="areaId"></param>
        /// <param name="typeId"></param>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="parkId"></param>
        /// <returns></returns>
        private async Task<IPagedList<VillageFarmlandDto>> GetParkFarmlands(int areaId, int typeId, string keyword, int page, int limit, int parkId, List<OrderBy> orders = null, string ids = "")
        {

            StringBuilder countBuilder = new StringBuilder(@$"
                                                             select 
                                                             VillageFarmland.id
                                                             from VillageFarmland 
                                                             left join ParkArea on VillageFarmland.parkId=ParkArea.id 
                                                             where VillageFarmland.isDeleted=0 and VillageFarmland.category =2 and VillageFarmland.useFor = 1 ");

            StringBuilder queryBuilder = new StringBuilder(@$"select 
                                                             VillageFarmland.createdAt,
                                                             VillageFarmland.id,
                                                             VillageFarmland.name,
                                                             VillageFarmland.area,
                                                             VillageFarmland.areaId,
                                                             VillageFarmland.unit,
                                                             VillageFarmland.address,
                                                             VillageFarmland.typeId,
                                                             VillageFarmland.remark,
                                                             VillageFarmland.householdId,
                                                             VillageFarmland.parkId,
                                                             ParkArea.enterpriseName as parkName
                                                             from VillageFarmland 
                                                             left join ParkArea on VillageFarmland.parkId=ParkArea.id 
                                                             where VillageFarmland.isDeleted=0 and VillageFarmland.category =2 and VillageFarmland.useFor = 1 ");
            StringBuilder where = new StringBuilder();
            if (typeId > 0)
            {
                where.Append($" and VillageFarmland.typeId= {typeId}");
            }
            if (parkId > 0)
            {

                where.Append($" and  VillageFarmland.parkId={parkId} ");
            }
            if (areaId > 0)
            {

                where.Append($" and  VillageFarmland.areaId={areaId} ");
            }
            if (!keyword.IsNullOrWhiteSpace())
            {
                where.Append($" and (ParkArea.enterpriseName like '%{keyword}%' or VillageFarmland.name like '%{keyword}%')");
            }
            if (!string.IsNullOrWhiteSpace(ids))
            {
                where.Append($" and VillageFarmland.id in ({ids})");
            }
            if (where.Length > 0)
            {
                queryBuilder.Append(where);
                countBuilder.Append(where);
            }
            if (orders == null)
            {
                orders = new List<OrderBy>();
            }
            orders.Add(new OrderBy() { FieldName = "id", Sort = "desc" });
            StringBuilder orderByBuilder = new StringBuilder(this.Context.Database.GetOrderBySql(orders));
            IPagedList<VillageFarmlandDto> pageInfo = this.Context.Database.SqlQueryPagedList<VillageFarmlandDto>(page, limit, queryBuilder.ToString(), countBuilder.ToString(), orderByBuilder.ToString());

            List<int> areaIds = pageInfo.Select(a => a.AreaId).ToList();
            List<int> typeIds = pageInfo.Select(a => a.TypeId).ToList();
            List<int> faramlandIds = pageInfo.Select(a => a.Id).ToList();
            IList<BasicArea> areas = await this.basicAreaService.GetListAsync(a => areaIds.Contains(a.Id));
            IList<BasicDictionary> dictionaries = await this.basicDictionaryService.GetListAsync(a => typeIds.Contains(a.Code) && a.IsDeleted == 0);
            // 查询地块是否已经打点
            IList<GISPlotItem> plotItems = await gisPlotItemService.GetListAsync(plot => faramlandIds.Contains(plot.ObjectId) && plot.PlotType == (int)PlotType.FARMLAND && plot.IsDeleted == 0);
            foreach (var item in pageInfo)
            {
                var area = areas.FirstOrDefault(a => a.Id == item.AreaId);
                var dictionary = dictionaries.FirstOrDefault(a => a.Code == item.TypeId);
                item.TypeName = dictionary == null ? "" : dictionary.Name;
                item.TypeNameCode = dictionary == null ? "" : dictionary.Code.ToString();
                item.TypeDto = dictionary == null ? null : this.mapper.Map<BasicDictionaryDto>(dictionary);
                item.AreaName = area == null ? "" : area.Name;
                item.isPloted = plotItems.FirstOrDefault(plotItem => plotItem.ObjectId == item.Id) != null;
            }
            return pageInfo;
        }

        private async Task<IPagedList<VillageFarmlandDto>> GetHouseholdFarmlands(int areaId, int typeId, string keyword, int page, int limit, int householdId, List<OrderBy> orders = null, string ids = "")
        {
            StringBuilder countBuilder = new StringBuilder(@$"
                                                             select 
                                                             VillageFarmland.id
                                                             from VillageFarmland 
                                                             left join VillageHouseholdCode on VillageFarmland.householdId=VillageHouseholdCode.id and VillageHouseholdCode.isDeleted=0
                                                             left join VillageHouseCodeMember on VillageHouseCodeMember.householdId = VillageHouseholdCode.id  and  VillageHouseCodeMember.isHouseholder=1  and VillageHouseCodeMember.isDeleted=0
                                                             left join VillagePopulation on  VillagePopulation.id=VillageHouseCodeMember.populationId   and VillagePopulation.isDeleted=0    where  VillageFarmland.isDeleted=0 and VillageFarmland.category =1 and VillageFarmland.useFor = 1 ");

            StringBuilder queryBuilder = new StringBuilder(@$"select 
                                                             VillageFarmland.createdAt,
                                                             VillageFarmland.id,
                                                             VillageFarmland.name,
                                                             VillageFarmland.area,
                                                             VillageFarmland.areaId,
                                                             VillageFarmland.unit,
                                                             VillageFarmland.address,
                                                             VillageFarmland.typeId,
                                                             VillageFarmland.remark,
                                                             VillageFarmland.householdId,
                                                             VillagePopulation.realName as householder,
                                                             VillagePopulation.mobile,
                                                             VillageHouseholdCode.houseName,
                                                             VillageHouseholdCode.houseNumber
                                                             from VillageFarmland 
                                                             left join VillageHouseholdCode on VillageFarmland.householdId=VillageHouseholdCode.id and VillageHouseholdCode.isDeleted=0
                                                             left join VillageHouseCodeMember on VillageHouseCodeMember.householdId = VillageHouseholdCode.id and  VillageHouseCodeMember.isHouseholder=1  and VillageHouseCodeMember.isDeleted=0
                                                             left join VillagePopulation on  VillagePopulation.id=VillageHouseCodeMember.populationId  and VillagePopulation.isDeleted=0   where   VillageFarmland.isDeleted=0 and VillageFarmland.category =1 and VillageFarmland.useFor = 1 ");
            StringBuilder where = new StringBuilder();
            if (typeId > 0)
            {
                where.Append($" and  VillageFarmland.typeId= {typeId}");
            }
            if (householdId > 0)
            {
                where.Append($" and VillageFarmland.householdId={householdId} ");
            }
            if (areaId > 0)
            {
                where.Append($" and  VillageFarmland.areaId ={ areaId} ");
            }
            if (!keyword.IsNullOrWhiteSpace())
            {
                // where.Append($" and  ( VillagePopulation.RealName='{BasicSO.Encrypt(keyword)}' or VillageHouseholdCode.houseName like '%{keyword}%' or VillageHouseholdCode.houseNumber like '%{keyword}%' or VillageFarmland.name like '%{keyword}%' )");
                where.Append($" and   VillageFarmland.name like '%{keyword}%' ");
            }
            if (!string.IsNullOrWhiteSpace(ids))
            {
                where.Append($" and VillageFarmland.id in ({ids})");
            }
            if (where.Length > 0)
            {
                queryBuilder.Append(where);
                countBuilder.Append(where);
            }
            if (orders == null)
            {
                orders = new List<OrderBy>();
            }
            orders.Add(new OrderBy() { FieldName = "id", Sort = "desc" });
            StringBuilder orderByStringBuilder = new StringBuilder(this.Context.Database.GetOrderBySql(orders));

            IPagedList<VillageFarmlandDto> pageInfo = this.Context.Database.SqlQueryPagedList<VillageFarmlandDto>(page, limit, queryBuilder.ToString(), countBuilder.ToString(), orderByStringBuilder.ToString());

            List<int> areaIds = pageInfo.Select(a => a.AreaId).ToList();
            List<int> typeIds = pageInfo.Select(a => a.TypeId).ToList();
            List<int> faramlandIds = pageInfo.Select(a => a.Id).ToList();
            IList<BasicArea> areas = await this.basicAreaService.GetListAsync(a => areaIds.Contains(a.Id));
            IList<BasicDictionary> dictionaries = await this.basicDictionaryService.GetListAsync(a => typeIds.Contains(a.Code) && a.IsDeleted == 0);
            // 查询地块是否已经打点
            IList<GISPlotItem> plotItems = await gisPlotItemService.GetListAsync(plot => faramlandIds.Contains(plot.ObjectId) && plot.PlotType == (int)PlotType.FARMLAND && plot.IsDeleted == 0);
            // 解密readName
            foreach (var item in pageInfo)
            {
                var area = areas.FirstOrDefault(a => a.Id == item.AreaId);
                var dictionary = dictionaries.FirstOrDefault(a => a.Code == item.TypeId);
                item.Householder = BasicSO.Decrypt(item.Householder);
                if (item.Householder.IsNullOrWhiteSpace())
                {
                    item.Householder = "村集体";
                }
                item.Mobile = BasicSO.Decrypt(item.Mobile);
                item.TypeName = dictionary == null ? "" : dictionary.Name;
                item.TypeNameCode = dictionary == null ? "" : dictionary.Code.ToString();
                item.TypeDto = dictionary == null ? null : this.mapper.Map<BasicDictionaryDto>(dictionary);
                item.AreaName = area == null ? "" : area.Name;
                item.isPloted = plotItems.FirstOrDefault(plotItem => plotItem.ObjectId == item.Id) != null;
            }

            return pageInfo;
        }

        private async Task<IPagedList<VillageFarmlandDto>> GetPlanLands(int areaId, int typeId, string keyword, int page, int limit, List<OrderBy> orders = null, string ids = "")
        {
            StringBuilder countBuilder = new StringBuilder(@$"
                                                             select 
                                                             VillageFarmland.id
                                                             from VillageFarmland 
                                                             where  VillageFarmland.isDeleted=0 and VillageFarmland.category = 1 and VillageFarmland.useFor = 2 ");

            StringBuilder queryBuilder = new StringBuilder(@$"select 
                                                             VillageFarmland.createdAt,
                                                             VillageFarmland.id,
                                                             VillageFarmland.name,
                                                             VillageFarmland.area,
                                                             VillageFarmland.areaId,
                                                             VillageFarmland.unit,
                                                             VillageFarmland.address,
                                                             VillageFarmland.typeId,
                                                             VillageFarmland.remark,
                                                             VillageFarmland.householdId
                                                             from VillageFarmland 
                                                             where VillageFarmland.isDeleted=0 and VillageFarmland.category = 1 and VillageFarmland.useFor = 2 ");
            StringBuilder where = new StringBuilder();
            if (typeId > 0)
            {
                where.Append($" and  VillageFarmland.typeId= {typeId}");
            }
            if (areaId > 0)
            {
                where.Append($" and  VillageFarmland.areaId ={ areaId} ");
            }
            if (!keyword.IsNullOrWhiteSpace())
            {
                where.Append($" and VillageFarmland.name like '%{keyword}%'");
            }
            if (!string.IsNullOrWhiteSpace(ids))
            {
                where.Append($" and VillageFarmland.id in ({ids})");
            }
            if (where.Length > 0)
            {
                queryBuilder.Append(where);
                countBuilder.Append(where);
            }
            if (orders == null)
            {
                orders = new List<OrderBy>();
            }
            orders.Add(new OrderBy() { FieldName = "id", Sort = "desc" });
            StringBuilder orderByStringBuilder = new StringBuilder(this.Context.Database.GetOrderBySql(orders));
            IPagedList<VillageFarmlandDto> pageInfo = this.Context.Database.SqlQueryPagedList<VillageFarmlandDto>(page, limit, queryBuilder.ToString(), countBuilder.ToString(), orderByStringBuilder.ToString());

            List<int> areaIds = pageInfo.Select(a => a.AreaId).ToList();
            List<int> typeIds = pageInfo.Select(a => a.TypeId).ToList();
            List<int> faramlandIds = pageInfo.Select(a => a.Id).ToList();
            IList<BasicArea> areas = await this.basicAreaService.GetListAsync(a => areaIds.Contains(a.Id));
            IList<BasicDictionary> dictionaries = await this.basicDictionaryService.GetListAsync(a => typeIds.Contains(a.Code) && a.IsDeleted == 0);
            // 查询地块是否已经打点
            IList<GISPlotItem> plotItems = await gisPlotItemService.GetListAsync(plot => faramlandIds.Contains(plot.ObjectId) && plot.PlotType == (int)PlotType.PLANLAND && plot.IsDeleted == 0);
            // 解密readName
            foreach (var item in pageInfo)
            {
                var area = areas.FirstOrDefault(a => a.Id == item.AreaId);
                var dictionary = dictionaries.FirstOrDefault(a => a.Code == item.TypeId);
                item.Householder = BasicSO.Decrypt(item.Householder);
                item.TypeName = dictionary == null ? "" : dictionary.Name;
                item.TypeNameCode = dictionary == null ? "" : dictionary.Code.ToString();
                item.TypeDto = dictionary == null ? null : this.mapper.Map<BasicDictionaryDto>(dictionary);
                item.AreaName = area == null ? "" : area.Name;
                item.isPloted = plotItems.FirstOrDefault(plotItem => plotItem.ObjectId == item.Id) != null;
            }
            return pageInfo;
        }

        public async Task<bool> Remove(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                throw new ValidException("请选择要删除的地块");
            }
            int count = await this.GetQueryable().Where(farmland => ids.Contains(farmland.Id)).UpdateFromQueryAsync(farmlan => new VillageFarmland()
            {
                IsDeleted = (int)IsDeleted.Deleted
            });
            return count > 0;
        }

        public async Task<int> Save(VillageFarmland villageFarmland, int userId)
        {
            if (villageFarmland.Id > 0)
            {
                VillageFarmland farmlandQueryByName = null;
                if (villageFarmland.ParkId > 0)
                {
                    farmlandQueryByName = await this.GetQueryable().Where(farmland => farmland.Id != villageFarmland.Id && farmland.Name == villageFarmland.Name && farmland.ParkId == villageFarmland.ParkId && farmland.Category==2 && farmland.IsDeleted == 0).FirstOrDefaultAsync();
                }
                else if (villageFarmland.AreaId > 0)
                {
                    farmlandQueryByName = await this.GetQueryable().Where(farmland => farmland.UseFor == villageFarmland.UseFor && farmland.Id != villageFarmland.Id && farmland.Name == villageFarmland.Name && farmland.AreaId == villageFarmland.AreaId && farmland.Category == 1&& farmland.IsDeleted == 0).FirstOrDefaultAsync();
                }

                if (farmlandQueryByName != null)
                {
                    throw new ValidException("名称已存在");
                }
                int count = await this.GetQueryable().Where(farmland => farmland.Id == villageFarmland.Id).UpdateFromQueryAsync(farmland => new VillageFarmland
                {
                    Name = villageFarmland.Name,
                    TypeId = villageFarmland.TypeId,
                    Area = villageFarmland.Area,
                    HouseholdId = villageFarmland.HouseholdId,
                    Unit = villageFarmland.Unit,
                    Address = villageFarmland.Address,
                    Remark = villageFarmland.Remark,
                    UpdatedBy = userId
                });
                return villageFarmland.Id;
            }
            else
            {
                VillageFarmland farmlandQueryByName = null;
                if (villageFarmland.ParkId > 0)
                {
                    farmlandQueryByName = await this.GetQueryable().Where(farmland => farmland.Name == villageFarmland.Name && farmland.ParkId == villageFarmland.ParkId && farmland.Category == 2 && farmland.IsDeleted == 0).FirstOrDefaultAsync();
                }
                else  if (villageFarmland.AreaId > 0)
                {
                    farmlandQueryByName = await this.GetQueryable().Where(farmland => farmland.Name == villageFarmland.Name && farmland.AreaId == villageFarmland.AreaId && farmland.Category == 1 && farmland.IsDeleted == 0).FirstOrDefaultAsync();
                }

                if (farmlandQueryByName != null)
                {
                    throw new ValidException("名称已存在");
                }
                villageFarmland.CreatedBy = userId;
                VillageFarmland land = await this.InsertAsync(villageFarmland);
                return land.Id;
            }
        }

        public async Task<List<StatisticsFarmlandDto>> GetStatisticsFarmland(int householdId)
        {
            var list = from f in this.GetQueryable().Where(a => a.HouseholdId == householdId && a.IsDeleted == 0)
                       join d in this.basicDictionaryService.GetQueryable() on f.TypeId equals d.Code
                       group f by new { f.TypeId, d.Name } into g
                       select new StatisticsFarmlandDto()
                       {
                           TypeId = g.Key.TypeId,
                           HouseholdId = householdId,
                           Area = g.Sum(a => a.Area),
                           TypeName = g.Key.Name
                       };
            return await list.ToListAsync();
        }

        public async Task<int> SaveImport(VillageFarmland villageFarmland, int userId)
        {
            try
            {
                VillageFarmland farmlandQueryByName = null;
                if (villageFarmland.ParkId > 0)
                {
                    farmlandQueryByName = await this.GetQueryable().Where(farmland => farmland.Name == villageFarmland.Name && farmland.ParkId == villageFarmland.ParkId && farmland.IsDeleted == 0).FirstOrDefaultAsync();
                    
                }
                else if (villageFarmland.AreaId > 0)
                {
                    farmlandQueryByName = await this.GetQueryable().Where(farmland => farmland.Name == villageFarmland.Name && farmland.AreaId == villageFarmland.AreaId && farmland.IsDeleted == 0 && farmland.UseFor == villageFarmland.UseFor && farmland.Category == villageFarmland.Category && farmland.HouseholdId == villageFarmland.HouseholdId && farmland.ParkId == 0).FirstOrDefaultAsync();
                }
                if (farmlandQueryByName != null)
                {
                    int count = await this.GetQueryable().Where(farmland => farmland.Id == farmlandQueryByName.Id).UpdateFromQueryAsync(farmland => new VillageFarmland
                    {
                        Name = villageFarmland.Name,
                        TypeId = villageFarmland.TypeId,
                        Area = villageFarmland.Area,
                        HouseholdId = villageFarmland.HouseholdId,
                        Unit = villageFarmland.Unit,
                        Address = villageFarmland.Address,
                        Remark = villageFarmland.Remark,
                        UpdatedBy = userId
                    });
                    return farmlandQueryByName.Id;
                }
                else
                {
                    villageFarmland.CreatedBy = userId;
                    VillageFarmland land = await this.InsertAsync(villageFarmland);
                    return land.Id;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
    }
}
