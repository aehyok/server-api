using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Application.Services.Village;
using DVS.Common;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Common.SO;
using DVS.Core.Domains.Common;
using DVS.Core.Domains.FFP;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.FFP.Query;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.FFP
{
    public class FFPMoPaiLogService : ServiceBase<FFPMoPaiLog>, IFFPMoPaiLogService
    {
        private readonly IBasicUserService basicUserService;
        private readonly ISunFileInfoService fileService;
        private readonly IFFPWorkflowService workflowService;
        private readonly IHouseholdCodeService householdCodeService;
        private readonly IBasicAreaService basicAreaService;

        public FFPMoPaiLogService(DbContext dbContext, IMapper mapper,
            IBasicUserService basicUserService,
            ISunFileInfoService fileService,
            IFFPWorkflowService workflowService,
            IHouseholdCodeService householdCodeService,
            IBasicAreaService basicAreaService
            )
          : base(dbContext, mapper)
        {
            this.basicUserService = basicUserService;
            this.fileService = fileService;
            this.workflowService = workflowService;
            this.householdCodeService = householdCodeService;
            this.basicAreaService = basicAreaService;
        }

        public async Task<FFPMoPaiLogDto> DetailMoPai(int id)
        {
            var data = await this.GetAsync(a => a.Id == id && a.IsDeleted == 0);
            if (data == null)
                throw new ValidException("数据不存在");

            var result = mapper.Map<FFPMoPaiLogDto>(data);

            var user = await this.basicUserService.GetAsync(a => a.Id == data.CreatedBy);
            if (user != null)
            {
                result.CreatedByName = user.NickName;
            }
            user = await this.basicUserService.GetAsync(a => a.Id == data.UpdatedBy);
            if (user != null)
            {
                result.UpdatedByName = user.NickName;
            }
            var files = await this.fileService.GetSunFileInfoList(result.Images);
            result.ImageFiles = files;
            files = await this.fileService.GetSunFileInfoList(result.VoiceUrl);
            result.VoiceFiles = files;
            return result;
        }

        public async Task<IPagedList<FFPMatrixHouseholdDto>> ListMoPai(string keyword, int inspector, int page, int limit)
        {
            string sqlWhere = "";
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sqlWhere = string.Format(" and (p.realName like '%{0}%' or h.houseName  LIKE '%{0}%'  or h.houseNumber  LIKE '%{0}%' ) ", keyword);
            }

            string sqlWhere_fn = "";
            if (inspector > 0)
            {
                sqlWhere_fn = string.Format(" and (fm.inspector RLIKE '(^|,){0}(,|$)' or fm.inspectorManager RLIKE '(^|,){0}(,|$)') ", inspector);
            }

            string sql = string.Format(@" 
                SELECT distinct 0 as id, fmh.matrixId, t.*,
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
                    and m.isHouseholder=1 and v.isDeleted=0 and m.isDeleted=0)p  on h.id= p.householdId
                    where h.isDeleted=0 and h.isDeleted=0 {0}
                )t , FFPMatrixHousehold fmh,FFPMatrix fm,FFPHouseholdCode fh where t.householdId = fmh.householdId and t.householdId = fh.householdId and 
                    fmh.matrixId = fm.id and fm.isDeleted=0 and fmh.isDeleted = 0 and fh.isDeleted = 0 and t.householdId in (select distinct householdId from FFPMoPaiLog where isDeleted = 0) {1} ", sqlWhere, sqlWhere_fn);

            var orderby = " order by convert(houseName using gbk) asc, convert(houseName using gbk) asc ";

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

                Expression<Func<BasicUser, bool>> expression_inspector = a => a.IsDeleted == 0;
                Expression<Func<BasicUser, bool>> expression_manager = a => a.IsDeleted == 0;
                if (!string.IsNullOrWhiteSpace(item.Inspector))
                {
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
                item.FFPMoPaiLogs = await this.ListMoPai(item.HouseholdId);

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

            return pageData;
        }

        private async Task<List<FFPMoPaiLogDto>> ListMoPai(int householdId)
        {
            var data = await this.GetListAsync(a => a.HouseholdId == householdId && a.IsDeleted == 0);
            if (data == null)
                return new List<FFPMoPaiLogDto>();

            data.OrderByDescending(a => a.Id);
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
                //var files = await this.fileService.GetSunFileInfoList(item.Images);
                //item.ImageFiles = files;
                //files = await this.fileService.GetSunFileInfoList(item.VoiceUrl);
                //item.VoiceFiles = files;
            }
            return result;
        }

        public async Task<IPagedList<FFPMatrixHouseholdDto>> ListMoPaiByStatus(string keyword, int inspector, int workflowstatus, string householdType, int page, int limit)
        {
            string sqlWhere = "";
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sqlWhere = string.Format(" and (p.realName like '%{0}%' or h.houseName  LIKE '%{0}%'  or h.houseNumber  LIKE '%{0}%' ) ", keyword);
            }

            string sqlWhere_fn = "";
            if (inspector > 0)
            {
                sqlWhere_fn = string.Format(" and (fm.inspector RLIKE '(^|,){0}(,|$)' or fm.inspectorManager RLIKE '(^|,){0}(,|$)') ", inspector);
            }

            if (workflowstatus > 0)
            {
                sqlWhere_fn += string.Format(" and fw.flowStatus = {0} ", workflowstatus);
            }

            if (!string.IsNullOrWhiteSpace(householdType))
            {
                sqlWhere_fn += string.Format(" and fh.householdType = '{0}' ", householdType);
            }

            string sql = string.Format(@" 
                SELECT distinct 0 as id, fmh.matrixId, t.*,
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
                    and m.isHouseholder=1 and v.isDeleted=0 and m.isDeleted=0)p  on h.id= p.householdId
                    where h.isDeleted=0 and h.isDeleted=0 {0}
                )t , FFPMatrixHousehold fmh,FFPMatrix fm,FFPHouseholdCode fh,FFPWorkflow fw,FFPMoPaiLog fml where t.householdId = fmh.householdId and t.householdId = fh.householdId and 
                    t.householdId = fw.householdId and fml.id = fw.moPaiId and fmh.matrixId = fm.id and fm.isDeleted=0 and fmh.isDeleted = 0 and fh.isDeleted = 0 and fw.isDeleted = 0 and
                    fml.isDeleted = 0 {1} ", sqlWhere, sqlWhere_fn);

            var orderby = " order by convert(houseName using gbk) asc, convert(houseName using gbk) asc ";

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

                Expression<Func<BasicUser, bool>> expression_inspector = a => a.IsDeleted == 0;
                Expression<Func<BasicUser, bool>> expression_manager = a => a.IsDeleted == 0;
                if (!string.IsNullOrWhiteSpace(item.Inspector))
                {
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
                item.FFPMoPaiLogs = await this.ListMoPai(item.HouseholdId);

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

            return pageData;
        }

        public async Task<int> SaveMoPai(FFPMoPaiLog mopailog)
        {
            if (mopailog.HouseholdId == 0 || mopailog.MatrixId == 0)
                throw new ValidException("参数无效");

            // 存在未完成的摸排流程
            var workflow = this.workflowService.GetQueryable().Where(a => a.HouseholdId == mopailog.HouseholdId && a.IsDeleted == 0 && a.FlowStatus < 9).OrderByDescending(a => a.MoPaiId);
            if (workflow != null && workflow.Count() > 0)
            {
                throw new ValidException("重复摸排");
            }
            var householderInfo = await this.householdCodeService.GetHouseholderInfo(mopailog.HouseholdId);
            var householderName = "";
            if (householderInfo != null) {
                householderName = householderInfo.FullName;
            }
            if (mopailog.Id > 0)
            {
                var data = await this.GetAsync(a => a.Id == mopailog.Id && a.IsDeleted == 0);
                if (data == null)
                {
                    throw new ValidException("摸排记录不存在");
                }
                var ExistRisk = data.ExistRisk;
                data.HouseholdId = mopailog.HouseholdId;
                data.Images = mopailog.Images;
                data.VoiceUrl = mopailog.VoiceUrl;
                data.ExistRisk = mopailog.ExistRisk;
                data.UpdatedBy = mopailog.UpdatedBy;
                data.Describe = mopailog.Describe;
                var ret = await this.UpdateAsync(data);

                if (mopailog.ExistRisk == 0)
                {
                    await this.workflowService.GetQueryable().Where(a => a.MoPaiId == mopailog.Id).UpdateFromQueryAsync(a => new FFPWorkflow()
                    {
                        IsDeleted = 1,
                        UpdatedBy = mopailog.UpdatedBy,
                    });
                }
                else if (ExistRisk == 0)
                { //原来无风险，现在有风险
                    await this.workflowService.SaveWorkflow(new FFPWorkflow()
                    {
                        HouseholdId = mopailog.HouseholdId,
                        MoPaiId = mopailog.Id,
                        FlowStatus = 1,
                        Describe = mopailog.Describe,
                        CreatedBy = mopailog.CreatedBy,
                        UpdatedBy = mopailog.CreatedBy,
                        MatrixId = mopailog.MatrixId,
                        Name = householderName,
                    });
                }

                return ret;
            }
            else
            {
                var result = await this.InsertAsync(mopailog);
                if (result != null)
                {
                    var flowstatus = 9; // 流程结束
                    var isMoPai = 0; // 未摸排
                    var describe = "不存在返贫风险";
                    if (mopailog.ExistRisk == 1)
                    {
                        flowstatus = 1; // 摸排确认
                        isMoPai = 1; // 已摸排
                        describe = "";
                    }
                    await this.workflowService.SaveWorkflow(new FFPWorkflow()
                    {
                        HouseholdId = mopailog.HouseholdId,
                        MoPaiId = result.Id,
                        FlowStatus = flowstatus,
                        CreatedBy = mopailog.CreatedBy,
                        UpdatedBy = mopailog.CreatedBy,
                        MatrixId = mopailog.MatrixId,
                        Describe = describe,
                        Name = householderName,
                    });

                    _ = await this.householdCodeService.GetQueryable().Where(a => a.Id == mopailog.HouseholdId && a.IsDeleted == 0).UpdateFromQueryAsync(a => new VillageHouseholdCode()
                    {
                        IsMoPai = isMoPai
                    });
                    return result.Id;
                }
                return 0;
            }
        }

        public async Task<FFPHouseholdStatisticDto> StatisticHousehold(int inspector)
        {
            string sqlWhere = string.Format(" and (fm.inspector RLIKE '(^|,){0}(,|$)' or fm.inspectorManager RLIKE '(^|,){0}(,|$)') ", inspector);

            string sql = string.Format(@" 
                SELECT count(distinct(fmp.householdId)) as total
                from FFPMatrixHousehold fmh,FFPMatrix fm ,FFPMoPaiLog fmp
                where fmh.matrixId = fm.id and fmp.householdId = fmh.householdId and fm.isDeleted=0 and fmh.isDeleted = 0 and fmp.isDeleted = 0 {0}", sqlWhere);

            var pageData = await this.Context.Database.SqlQueryAsync<FFPHouseholdStatisticDto>(sql);
            return pageData.FirstOrDefault();
        }

        public async Task<List<FFPHouseholdStatisticDto>> StatisticHouseholdByMonth(int inspector)
        {
            string sqlWhere = string.Format(" and (fm.inspector RLIKE '(^|,){0}(,|$)' or fm.inspectorManager RLIKE '(^|,){0}(,|$)') ", inspector);

            string sql = string.Format(@" 
                SELECT isMoPai, count(distinct(fmp.householdId)) as cnt
                from FFPMatrixHousehold fmh,FFPMatrix fm ,FFPMoPaiLog fmp ,VillageHouseholdCode vh
                where fmh.matrixId = fm.id and fmp.householdId = fmh.householdId and vh.id = fmp.householdId and
                 fm.isDeleted=0 and fmh.isDeleted = 0 and fmp.isDeleted = 0 and vh.isDeleted = 0 and date_format(moPaiDate,'%Y-%m') = date_format(now(),'%Y-%m') {0}", sqlWhere);

            var pageData = await this.Context.Database.SqlQueryAsync<FFPHouseholdStatisticDto>(sql);
            return pageData;
        }

        public async Task<IPagedList<FFPMatrixHouseholdDto>> ListMoPaiMonth(string keyword, int inspector, string householdType, int page, int limit, int isMoPai = -1)
        {
            string sqlWhere = "";
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sqlWhere = string.Format(" and (p.realName like '%{0}%' or h.houseName  LIKE '%{0}%'  or h.houseNumber  LIKE '%{0}%' ) ", keyword);
            }

            string sqlWhere_fn = "";
            if (inspector > 0)
            {
                sqlWhere_fn = string.Format(" and (fm.inspector RLIKE '(^|,){0}(,|$)' or fm.inspectorManager RLIKE '(^|,){0}(,|$)') ", inspector);
            }

            if (isMoPai > -1)
            {
                sqlWhere += string.Format(" and h.isMoPai = {0} ", isMoPai);
            }

            if (!string.IsNullOrWhiteSpace(householdType))
            {
                sqlWhere_fn += string.Format(" and fh.householdType = '{0}' ", householdType);
            }
            string sql = string.Format(@" 
                SELECT distinct 0 as id, fmh.matrixId, t.*,
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
                    and m.isHouseholder=1 and v.isDeleted=0 and m.isDeleted=0)p  on h.id= p.householdId
                    where h.isDeleted=0 and h.isDeleted=0 {0}
                )t , FFPMatrixHousehold fmh,FFPMatrix fm,FFPHouseholdCode fh,FFPMoPaiLog fml where t.householdId = fmh.householdId and t.householdId = fh.householdId and 
                    fmh.matrixId = fm.id and fm.isDeleted=0 and fmh.isDeleted = 0 and fh.isDeleted = 0 and 
                    fml.isDeleted = 0 and date_format(fml.moPaiDate,'%Y-%m') = date_format(now(),'%Y-%m') {1} ", sqlWhere, sqlWhere_fn);

            if (isMoPai == 0)
            {
                sql = string.Format(@" 
                SELECT distinct 0 as id, fmh.matrixId, t.*,
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
                    and m.isHouseholder=1 and v.isDeleted=0 and m.isDeleted=0)p  on h.id= p.householdId
                    where h.isDeleted=0 and h.isDeleted=0 {0}
                )t , FFPMatrixHousehold fmh,FFPMatrix fm,FFPHouseholdCode fh where t.householdId = fmh.householdId and t.householdId = fh.householdId and 
                    fmh.matrixId = fm.id and fm.isDeleted=0 and fmh.isDeleted = 0 and fh.isDeleted = 0 and t.householdId not in (select householdId from FFPWorkflow where isDeleted = 0 and flowStatus = 9 and date_format(createdAt,'%Y-%m') = date_format(now(),'%Y-%m')) {1} ", sqlWhere, sqlWhere_fn);
            }
            var orderby = " order by convert(houseName using gbk) asc, convert(houseName using gbk) asc ";

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

                Expression<Func<BasicUser, bool>> expression_inspector = a => a.IsDeleted == 0;
                Expression<Func<BasicUser, bool>> expression_manager = a => a.IsDeleted == 0;
                if (!string.IsNullOrWhiteSpace(item.Inspector))
                {
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
                item.FFPMoPaiLogs = await this.ListMoPai(item.HouseholdId);

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

            return pageData;
        }
    }
}
