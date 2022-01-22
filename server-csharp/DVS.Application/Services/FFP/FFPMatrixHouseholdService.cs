using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Common;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Common.SO;
using DVS.Core.Domains.Common;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.FFP.Query;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.FFP
{
    public class FFPMatrixHouseholdService : ServiceBase<FFPMatrixHousehold>, IFFPMatrixHouseholdService
    {
        private readonly IBasicUserService basicUserService;
        private readonly IBasicAreaService basicAreaService;
        private readonly IFFPHouseholdCodeService fFPHouseholdCodeService;
        private readonly IFFPMoPaiLogService moPaiLogService;
        private readonly IModuleDictionaryService moduleDictionaryService;

        public FFPMatrixHouseholdService(DbContext dbContext, IMapper mapper, IBasicUserService basicUserService, IBasicAreaService basicAreaService, IFFPHouseholdCodeService fFPHouseholdCodeService, IFFPMoPaiLogService moPaiLogService, IModuleDictionaryService moduleDictionaryService) : base(dbContext, mapper)
        {
            this.basicUserService = basicUserService;
            this.basicAreaService = basicAreaService;
            this.fFPHouseholdCodeService = fFPHouseholdCodeService;
            this.moPaiLogService = moPaiLogService;
            this.moduleDictionaryService = moduleDictionaryService;
        }

        /// <summary>
        /// 查询网格所属户码列表
        /// </summary>
        public async Task<IPagedList<FFPMatrixHouseholdDto>> ListMatrixHousehold(string keyword, int areaId,int matrixId, List<OrderBy> orders, int page, int limit)
        {
            string sqlWhere = "";
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sqlWhere = string.Format(" and (p.realName like '%{0}%' or h.houseName  LIKE '%{0}%'  or h.houseNumber  LIKE '%{0}%' ) ", keyword);
            }

            string areaid_v = "";
            if (areaId > 0)
            {
                var areaIds = await this.basicAreaService.FindChildrenAreaIds(areaId);
                areaid_v = " and v.areaId in (" + string.Join(",", areaIds) + ")";
                sqlWhere += " and h.areaId in (" + string.Join(",", areaIds) + ")";
            }
            string sql = string.Format(@" 
                SELECT fmh.id, fmh.matrixId, t.*,
                 (select count(vp.id) from VillagePopulation vp,VillageHouseCodeMember vc where vc.populationId = vp.id and vc.householdId = t.householdId and vc.isDeleted=0 and vp.isDeleted=0) as peopleCount 
                from 
                (
                    SELECT 
                    h.id as householdId,
                    h.areaId,
                    h.houseName,
                    h.houseNumber,
                    p.realName as householdMan,
                    IFNULL(p.sex,0)  as sex,
                    p.mobile,
                    p.nation,
                    p.populationId
                    from VillageHouseholdCode h
                    LEFT JOIN  (SELECT v.realName,v.sex,v.mobile,v.nation, m.householdId,m.populationId FROM VillagePopulation v, VillageHouseCodeMember m WHERE v.id = m.populationId {0} and m.isHouseholder=1 and v.isDeleted=0 and m.isDeleted=0 )p  on h.id= p.householdId
                    where h.isDeleted=0 and h.isDeleted=0 {1}
                )t, FFPMatrixHousehold fmh where t.householdId = fmh.householdId and fmh.isDeleted = 0 and fmh.matrixId = {2} ", areaid_v, sqlWhere, matrixId);


            var orderby = " order by convert(houseName using gbk) asc, convert(houseName using gbk) asc ";
            if (orders != null)
            {
                var orderlist = new List<string>();
                foreach (var order in orders)
                {
                    orderlist.Add("convert(" + order.FieldName + " using gbk) " + order.Sort);
                }
                orderby = " order by " + string.Join(",", orderlist);
            }
            var pageData = this.Context.Database.SqlQueryPagedList<FFPMatrixHouseholdDto>(page, limit, sql, "", orderby);
            foreach (var item in pageData)
            {
                item.HouseholdMan = BasicSO.Decrypt(item.HouseholdMan);
                item.Mobile = BasicSO.Decrypt(item.Mobile);
                var arealist = this.basicAreaService.FindParentAreas(item.AreaId);
                if (arealist != null)
                {
                    var area = arealist.Find(a => a.Id == item.AreaId);
                    if (area != null)
                    {
                        item.AreaName = area != null ? area.Name : "";
                        var parentrea = arealist.Find(a => a.Id == area.Pid);
                        item.ParentAreaName = parentrea != null ? parentrea.Name : "";
                    }
                }
            }

            return pageData;
        }

        /// <summary>
        /// 查询户码列表
        /// </summary>
        public async Task<IPagedList<FFPMatrixHouseholdDto>> ListHousehold(string keyword, int areaId, int isUsed, List<OrderBy> orders, int page, int limit)
        {
            string sqlWhere = "";
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sqlWhere = string.Format(" and (p.realName like '%{0}%' or h.houseName  LIKE '%{0}%'  or h.houseNumber  LIKE '%{0}%' ) ", keyword);
            }

            string areaid_v, areaid_h;
            if (areaId > 0)
            {
                var areaIds = await this.basicAreaService.FindChildrenAreaIds(areaId);
                areaid_v = " v.areaId in (" + string.Join(",", areaIds) + ")";
                areaid_h = " h.areaId in (" + string.Join(",", areaIds) + ")";
            }
            else {
                throw new ValidException("区域id不能为空");
            }
            string sql;
            // 所有
            if (isUsed == -1)
            {
                sql = string.Format(@" 
                SELECT distinct 0 as id, fmh.matrixId, t.*,
                    (select count(vp.id) from VillagePopulation vp,VillageHouseCodeMember vc where vc.populationId = vp.id and vc.householdId = t.householdId and vc.isDeleted=0 and vp.isDeleted=0) as peopleCount ,
                      CASE ifnull(fmh.id,0) WHEN 0 THEN 0 ELSE 1 END as isUsed
                from 
                (
                    SELECT 
                    h.id as householdId,
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
                    {0} and m.isHouseholder=1 and v.isDeleted=0 and m.isDeleted=0 )p  on h.id= p.householdId
                    where h.isDeleted=0 and h.isDeleted=0 and {1} {2}
                )t left join FFPMatrixHousehold fmh on t.householdId = fmh.householdId and fmh.isDeleted = 0 ", areaid_v, areaid_h, sqlWhere);
            }
            else if (isUsed == 0)
            {  // 未添加
                sql = string.Format(@" 
                SELECT distinct 0 as id, 0 as matrixId, t.*,
                    (select count(vp.id) from VillagePopulation vp,VillageHouseCodeMember vc where vc.populationId = vp.id and vc.householdId = t.householdId and vc.isDeleted=0 and vp.isDeleted=0) as peopleCount ,
                      0 as isUsed
                from 
                (
                    SELECT 
                    h.id as householdId,
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
                    {0} and m.isHouseholder=1 and v.isDeleted=0 and m.isDeleted=0 )p  on h.id= p.householdId
                    where h.isDeleted=0 and h.isDeleted=0 and {1} {2}
                )t where t.householdId not in (select householdId from FFPMatrixHousehold where isDeleted = 0) ", areaid_v, areaid_h, sqlWhere);
            }
            else if (isUsed == 1)
            {
                // 已添加
                sql = string.Format(@" 
                SELECT distinct 0 as id, fmh.matrixId, t.*,
                    (select count(vp.id) from VillagePopulation vp,VillageHouseCodeMember vc where vc.populationId = vp.id and vc.householdId = t.householdId and vc.isDeleted=0 and vp.isDeleted=0) as peopleCount ,
                      CASE ifnull(fmh.id,0) WHEN 0 THEN 0 ELSE 1 END as isUsed
                from 
                (
                    SELECT 
                    h.id as householdId,
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
                    {0} and m.isHouseholder=1 and v.isDeleted=0 and m.isDeleted=0 )p  on h.id= p.householdId
                    where h.isDeleted=0 and h.isDeleted=0 and {1} {2}
                )t , FFPMatrixHousehold fmh where t.householdId = fmh.householdId and fmh.isDeleted = 0", areaid_v, areaid_h, sqlWhere);
            }
            else {
                throw new ValidException("状态参数值无效");
            }

            var orderby = " order by convert(houseName using gbk) asc, convert(houseName using gbk) asc ";
            if (orders != null)
            {
                var orderlist = new List<string>();
                foreach (var order in orders)
                {
                    orderlist.Add("convert(" + order.FieldName + " using gbk) " + order.Sort);
                }
                orderby = " order by " + string.Join(",", orderlist);
            }
            var pageData = this.Context.Database.SqlQueryPagedList<FFPMatrixHouseholdDto>(page, limit, sql, "", orderby);
            foreach (var item in pageData)
            {
                item.HouseholdMan = BasicSO.Decrypt(item.HouseholdMan);
                item.Mobile = BasicSO.Decrypt(item.Mobile);
            }

            return pageData;
        }

        /// <summary>
        /// 查询花名册
        /// </summary>
        public async Task<IPagedList<FFPMatrixHouseholdDto>> ListHouseholdByMatrix(string keyword, int areaId, int matrixId, string householdTypes, int inspector, List<OrderBy> orders, int page, int limit, int householdId = 0, string ids = "")
        {
            string sqlWhere = "";
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sqlWhere = string.Format(" and (p.realName like '%{0}%' or h.houseName  LIKE '%{0}%'  or h.houseNumber  LIKE '%{0}%' ) ", keyword);
            }

            string sqlWhere_fn = "";
            if (inspector > 0) {
                sqlWhere_fn = string.Format(" and (fm.inspector RLIKE '(^|,){0}(,|$)' or fm.inspectorManager RLIKE '(^|,){0}(,|$)') ", inspector);
            }
            
            if (!string.IsNullOrWhiteSpace(householdTypes))
            {                
                sqlWhere_fn += string.Format(" and fh.householdType in ('{0}') ", string.Join("','", householdTypes.Split(",")));
            }

            if (!string.IsNullOrWhiteSpace(ids)) { 
                sqlWhere_fn += string.Format(" and t.householdId in ({0}) ", ids);
            }

            string areaid_v = "";
            if (areaId > 0)
            {
                var areaIds = await this.basicAreaService.FindChildrenAreaIds(areaId);
                areaid_v = " and v.areaId in (" + string.Join(",", areaIds) + ")";
                sqlWhere += " and h.areaId in (" + string.Join(",", areaIds) + ")";
            }

            if (matrixId > 0)
            {
                sqlWhere_fn += " and fmh.matrixId = " + matrixId;
            }
            if (householdId > 0)
            {
                sqlWhere_fn += " and t.householdId = " + householdId;
            }
            string sql = string.Format(@" 
                SELECT fmh.id, fmh.matrixId, t.*,
                    (select count(vp.id) from VillagePopulation vp,VillageHouseCodeMember vc where vc.populationId = vp.id and vc.householdId = t.householdId and vc.isDeleted=0 and vp.isDeleted=0) as peopleCount ,fm.inspector, fm.inspectorManager, fh.householdType
                from 
                (
                    SELECT 
                    h.id as householdId,
                    h.areaId,
                    h.houseName,
                    h.houseNumber,
                    p.realName as householdMan,
                    IFNULL(p.sex,0)  as sex,
                    p.mobile,
                    p.nation,
                    p.populationId
                    from VillageHouseholdCode h
                    LEFT JOIN  (SELECT v.realName,v.sex,v.mobile,v.nation, m.householdId,m.populationId FROM VillagePopulation v, VillageHouseCodeMember m WHERE v.id = m.populationId   
                    and m.isHouseholder=1 and v.isDeleted=0 and m.isDeleted=0 {0})p  on h.id= p.householdId
                    where h.isDeleted=0 and h.isDeleted=0 {1}
                )t , FFPMatrixHousehold fmh,FFPMatrix fm,FFPHouseholdCode fh where t.householdId = fmh.householdId and t.householdId = fh.householdId and 
                    fmh.matrixId = fm.id and fm.isDeleted=0 and fmh.isDeleted = 0 and fh.isDeleted = 0 {2} ", areaid_v, sqlWhere, sqlWhere_fn);

            var orderby = " order by convert(houseName using gbk) asc, convert(houseName using gbk) asc ";
            if (orders != null)
            {
                var orderlist = new List<string>();
                foreach (var order in orders)
                {
                    orderlist.Add("convert(" + order.FieldName + " using gbk) " + order.Sort);
                }
                orderby = " order by " + string.Join(",", orderlist);
            }
            var pageData = this.Context.Database.SqlQueryPagedList<FFPMatrixHouseholdDto>(page, limit, sql, "", orderby);
            var dictionarys100A03 = await this.moduleDictionaryService.GetModuleDictionaryAsync("100A03");
            foreach (var item in pageData)
            {
                item.HouseholdMan = BasicSO.Decrypt(item.HouseholdMan);
                item.Mobile = BasicSO.Decrypt(item.Mobile);

                var arealist = this.basicAreaService.FindParentAreas(item.AreaId);
                if (arealist != null)
                {
                    var area = arealist.Find(a => a.Id == item.AreaId);
                    if (area != null)
                    {
                        item.AreaName = area != null ? area.Name : "";
                        var parentrea = arealist.Find(a => a.Id == area.Pid);
                        item.ParentAreaName = parentrea != null ? parentrea.Name : "";
                    }
                }

                Expression<Func<BasicUser, bool>> expression_inspector = a=>a.IsDeleted == 0;
                Expression<Func<BasicUser, bool>> expression_manager = a => a.IsDeleted == 0;
                if (!string.IsNullOrWhiteSpace(item.Inspector)) {
                    var ids_inspector = item.Inspector.Split(",");
                    expression_inspector = expression_inspector.And(a => ids_inspector.Contains(a.Id.ToString()));
                    var inspectorlist = await this.basicUserService.GetListAsync(expression_inspector);
                    item.InspectorName = string.Join(",", inspectorlist.Select(a => a.NickName).ToList());
                }

                if (!string.IsNullOrWhiteSpace(item.InspectorManager))
                {
                    var ids_inspectormanager = item.InspectorManager.Split(",");
                    expression_manager = expression_manager.And(a => ids_inspectormanager.Contains(a.Id.ToString()));
                    var inspectormangerlist = await this.basicUserService.GetListAsync(expression_manager);
                    item.InspectorManagerName = string.Join(",", inspectormangerlist.Select(a => a.NickName).ToList());
                }
                if (matrixId == 0)
                {
                    //item.FFPMoPaiLogs = await this.ListMoPai(item.HouseholdId);
                    var sql_workflow = "select * from FFPWorkflow where householdId = " + item.HouseholdId + " and isDeleted = 0 and flowStatus < 9 order by moPaiId desc ";
                    var workflow_list = this.Context.Database.SqlQueryPagedList<FFPWorkflowDto>(1, 1, sql_workflow, "");
                    if (workflow_list != null)
                    {
                        foreach (var workflow in workflow_list)
                        {
                            var user = await this.basicUserService.GetAsync(a => a.Id == workflow.CreatedBy);
                            workflow.CreatedByName = user != null ? Utils.Decrypt(user.NickName) : "";
                        }
                        item.FFPWorkflows = workflow_list.ToList();
                    }
                }
                else
                {
                    var dic100A03 = dictionarys100A03.FirstOrDefault(a => a.Code == item.HouseholdType);
                    if (dic100A03 != null)
                    {
                        item.HouseholdTypeDto = this.mapper.Map<ModuleDictionaryDto>(dic100A03);
                    }
                }
            }

            return pageData;
        }

        private async Task<List<FFPMoPaiLogDto>> ListMoPai(int householdId)
        {
            //var today = DateTime.Now;
            //// 花名册上只查询当月的摸排记录
            //var data = await this.moPaiLogService.GetListAsync(a => a.HouseholdId == householdId && a.IsDeleted == 0 && a.MoPaiDate.Year == today.Year && a.MoPaiDate.Month == today.Month);
            var data = await this.moPaiLogService.GetListAsync(a => a.HouseholdId == householdId && a.IsDeleted == 0);
            if (data == null)
                return new List<FFPMoPaiLogDto>();

            data.OrderByDescending(a => a.MoPaiDate);
            var result = mapper.Map<List<FFPMoPaiLogDto>>(data);

            foreach (var item in result)
            {
                var user = await this.basicUserService.GetAsync(a => a.Id == item.CreatedBy);
                if (user != null)
                {
                    item.CreatedByName = user.NickName;
                }
                user = await this.basicUserService.GetAsync(a => a.Id == item.UpdatedBy);
                if (user != null)
                {
                    item.UpdatedByName = user.NickName;
                }
            }
            return result;
        }

        public async Task<int> RemoveMatrixHousehold(int matrixId, string householdIds, int userid)
        {

            var ids = householdIds.Split(",").ToList();
            var ret = await this.GetQueryable().Where(a => a.MatrixId == matrixId && ids.Contains(a.HouseholdId.ToString())).UpdateFromQueryAsync(a=> new FFPMatrixHousehold()
            { 
                IsDeleted = 1,
                UpdatedBy = userid
            });
            if (ret > 0)
            {
                var cnt = await this.CountAsync(a => a.MatrixId == matrixId && a.IsDeleted == 0);
                await this.ExecuteSqlAsync($"update FFPMatrix set houseCount = {cnt}, updatedBy = {userid} where id = {matrixId} and isdeleted = 0");
            }
            return ret;
        }

        public async Task<int> SaveMatrixHousehold(int matrixId, string householdIds, int userid)
        {
            var ids = householdIds.Split(",").ToList();
            FFPMatrixHousehold ret = null;
            foreach (var householdid in ids)
            {
                var cnt = await this.CountAsync(a => a.IsDeleted == 0 && a.HouseholdId == int.Parse(householdid)); // 一户只能对应一个区域网格
                if (cnt == 0)
                {
                    ret = await this.InsertAsync(new FFPMatrixHousehold()
                    {
                        MatrixId = matrixId,
                        HouseholdId = int.Parse(householdid),
                        CreatedBy = userid
                    });
                }

                var household = await this.fFPHouseholdCodeService.GetAsync(a => a.HouseholdId == int.Parse(householdid) && a.IsDeleted == 0);
                if (household == null)
                {
                    _ = await this.fFPHouseholdCodeService.InsertAsync(new FFPHouseholdCode()
                    {
                        HouseholdId = int.Parse(householdid),
                        Mobile = "",
                        MobileShort = "",
                        HouseholdType = "100A03A01", // 默认为普通农户
                        Remark = "",
                    });
                }
            }

            var housecnt = await this.CountAsync(a => a.MatrixId == matrixId && a.IsDeleted == 0);
            await this.ExecuteSqlAsync($"update FFPMatrix set houseCount = {housecnt}, updatedBy = {userid} where id = {matrixId} and isdeleted = 0");

            return ret != null ? ret.Id : 0;
        }

        public async Task<FFPHouseholdStatisticDto> StatisticHousehold(int inspector)
        {
            string sqlWhere = string.Format(" and (fm.inspector RLIKE '(^|,){0}(,|$)' or fm.inspectorManager RLIKE '(^|,){0}(,|$)') ", inspector);

            string sql = string.Format(@" 
                SELECT count(fmh.id) as total
                from FFPMatrixHousehold fmh,FFPMatrix fm 
                where fmh.matrixId = fm.id and fm.isDeleted=0 and fmh.isDeleted = 0 {0}", sqlWhere);

            var pageData = await this.Context.Database.SqlQueryAsync<FFPHouseholdStatisticDto>(sql);
            return pageData.FirstOrDefault();
        }

        public async Task<FFPHouseholdStatisticDto> StatisticMonitorHousehold(int inspector)
        {
            string sqlWhere = string.Format(" and (fm.inspector RLIKE '(^|,){0}(,|$)' or fm.inspectorManager RLIKE '(^|,){0}(,|$)') ", inspector);

            // 统计脱贫不稳定户、边缘易致贫户、突发严重困难户这三类
            string sql = string.Format(@" 
                SELECT count(fmh.id) as total
                from FFPMatrixHousehold fmh, FFPMatrix fm, FFPHouseholdCode fh
                where fmh.matrixId = fm.id and fh.householdId = fmh.householdId and fh.householdType in ('100A03001','100A03002','100A03003') and 
                    fm.isDeleted=0 and fmh.isDeleted = 0 and fh.isDeleted = 0 {0}", sqlWhere);

            var pageData = await this.Context.Database.SqlQueryAsync<FFPHouseholdStatisticDto>(sql);
            return pageData.FirstOrDefault();
        }

        public async Task<List<FFPHouseholdStatisticDto>> StatisticMonitorHouseholdByType(int inspector)
        {
            string sqlWhere = string.Format(" and (fm.inspector RLIKE '(^|,){0}(,|$)' or fm.inspectorManager RLIKE '(^|,){0}(,|$)') ", inspector);

            string sql = string.Format(@" 
                SELECT householdType, count(fmh.id) as cnt
                from FFPMatrixHousehold fmh, FFPMatrix fm, FFPHouseholdCode fh
                where fmh.matrixId = fm.id and fh.householdId = fmh.householdId and fm.isDeleted=0 and fmh.isDeleted = 0 and fh.isDeleted = 0 {0} group by fh.householdType", sqlWhere);

            var pageData = await this.Context.Database.SqlQueryAsync<FFPHouseholdStatisticDto>(sql);
            return pageData;
        }

        public async Task<byte[]> GetExcelData(int matrixId, int inspector = 0, string keyword = "", int areaId = 0, string householdTypes = "", string houseName = "", string houseNumber = "", string ids = "")
        {
            List<OrderBy> orders = new List<OrderBy>();
            if (!string.IsNullOrWhiteSpace(houseName)) {
                orders.Add(new OrderBy()
                {
                    FieldName = "houseName",
                    Sort = houseName
                });
            }
            if (!string.IsNullOrWhiteSpace(houseNumber))
            {
                orders.Add(new OrderBy()
                {
                    FieldName = "houseNumber",
                    Sort = houseNumber
                });
            }
            IPagedList<FFPMatrixHouseholdDto> list = await this.ListHouseholdByMatrix(keyword, areaId, matrixId, householdTypes, inspector, orders, 1, 10000,0, ids);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();

            package.Workbook.Properties.Title = "花名册";
            var workSheet = package.Workbook.Worksheets.Add("花名册");
            // 表头
            workSheet.Cells[1, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells[1, 1].Value = "序号";
            workSheet.Cells[1, 2].Value = "所属区域";
            workSheet.Cells[1, 3].Value = "社区/村";
            workSheet.Cells[1, 4].Value = "门牌名";
            workSheet.Cells[1, 5].Value = "门牌号";
            workSheet.Cells[1, 6].Value = "联系电话";
            workSheet.Cells[1, 7].Value = "人员数量";
            workSheet.Cells[1, 8].Value = "户属性";
            workSheet.Cells[1, 9].Value = "网格员";
            workSheet.Cells[1, 10].Value = "网格长";

            List<FFPMatrixHouseholdDto> items = list.ToList();
            for (int i = 0; i < items.Count; i++)
            {
                int rowIndex = i + 2;
                FFPMatrixHouseholdDto item = items[i];

                workSheet.Cells[rowIndex, 1].Value = i + 1;
                workSheet.Cells[rowIndex, 2].Value = item.ParentAreaName;
                workSheet.Cells[rowIndex, 3].Value = item.AreaName;
                workSheet.Cells[rowIndex, 4].Value = item.HouseName;
                workSheet.Cells[rowIndex, 5].Value = item.HouseNumber;
                workSheet.Cells[rowIndex, 6].Value = item.Mobile;
                workSheet.Cells[rowIndex, 7].Value = item.PeopleCount;
                workSheet.Cells[rowIndex, 8].Value = item.HouseholdTypeDto != null ? item.HouseholdTypeDto.Name : "";
                workSheet.Cells[rowIndex, 9].Value = item.InspectorName;
                workSheet.Cells[rowIndex, 10].Value = item.InspectorManagerName;
            }
            return await package.GetAsByteArrayAsync();
        }
    }
}
