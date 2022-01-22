using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Application.Services.Village;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Common.SO;
using DVS.Core.Domains.FFP;
using DVS.Core.Domains.Village;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.FFP.Query;
using DVS.Models.Dtos.FFP.Submit;
using DVS.Models.Enum;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.FFP
{
    public class FFPWorkflowService : ServiceBase<FFPWorkflow>, IFFPWorkflowService
    {
        private readonly IBasicUserService basicUserService;
        private readonly IPopulationAddressService populationAddressService;
        private readonly IHouseholdCodeTagService householdCodeTagService;
        private readonly IFFPPublicityManageService publicityManageService;
        private readonly IFFPPublicityHouseholdService publicityHouseholdService;
        private readonly IFFPApplicationService ffpApplicationService;
        private readonly IServiceBase<FFPFeedback> feedbackService;
        private readonly IFFPAutoNumberService autoNumberService;
        private readonly IModuleDictionaryService moduleDictionaryService;
        private readonly IFFPHouseholdCodeService householdCodeService;
        private readonly IServiceBase<VillageHouseCodeMember> memberService;
        private readonly ISunFileInfoService sunFileService;
        public FFPWorkflowService(DbContext dbContext, IMapper mapper,
            IBasicUserService basicUserService,
            IHouseholdCodeTagService householdCodeTagService,
            IFFPPublicityManageService publicityManageService,
            IFFPPublicityHouseholdService publicityHouseholdService,
             IFFPApplicationService ffpApplicationService,
            IPopulationAddressService populationAddressService,
            IServiceBase<FFPFeedback> feedbackService,
            IFFPAutoNumberService autoNumberService,
            IModuleDictionaryService moduleDictionaryService,
            IFFPHouseholdCodeService householdCodeService,
            IServiceBase<VillageHouseCodeMember> memberService,
            ISunFileInfoService sunFileService
            )
        : base(dbContext, mapper)
        {
            this.basicUserService = basicUserService;
            this.populationAddressService = populationAddressService;
            this.householdCodeTagService = householdCodeTagService;
            this.publicityManageService = publicityManageService;
            this.publicityHouseholdService = publicityHouseholdService;
            this.ffpApplicationService = ffpApplicationService;
            this.feedbackService = feedbackService;
            this.moduleDictionaryService = moduleDictionaryService;
            this.autoNumberService = autoNumberService;
            this.householdCodeService = householdCodeService;
            this.memberService = memberService;
            this.sunFileService = sunFileService;
        }

        /// <summary>
        /// 取会议评议列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IPagedList<ReviewListDto>> GetReviewList(ReviewListReq model)
        {

            string sql = string.Format(@"select   w.id,  f.householdType,
                                h.houseName, h.houseNumber, p.mobile, p.realName as householdMan,
                                ifnull(t1.peopleCount,0) as peopleCount, w.createdAt as checkDate 
                                from  FFPWorkflow w 
                                left  join FFPHouseholdCode f on w.householdId=f.householdId and f.isDeleted=0 
                                left  join VillageHouseholdCode h on h.id =w.householdId and h.isDeleted=0 
                                left  join  VillageHouseCodeMember m on h.id=m.householdId and m.isHouseholder=1 and m.isDeleted=0
                                left  join  VillagePopulation p on m.populationId= p.id and p.isDeleted=0
                                left  join (
                                       select n.householdId,COUNT(n.id) as peopleCount from VillageHouseCodeMember n 
                                       where n.isDeleted=0 and n.householdId>0  group by n.householdId
                                )t1 on w.householdId = t1.householdId
                                where w.flowStatus={0} and w.isDeleted=0 and h.areaId={1} ", model.Status, model.AreaId);

            if (!string.IsNullOrWhiteSpace(model.Keyword))
            {
                model.Keyword = model.Keyword.Replace("'", "").Replace("\"", "").Replace("-", "");
                string like = string.Format(" and (h.houseName like '%{0}%' or h.houseNumber like '%{0}%' or p.realName like '%{0}%')", model.Keyword);
                sql += like;
            }
            if (!string.IsNullOrWhiteSpace(model.HouseholdType))
            {
                string type = string.Format(" and f.householdType = '{0}' ", model.HouseholdType);
                sql += type;
            }

            var data = this.Context.Database.SqlQueryPagedList<ReviewListDto>(model.Page, model.Limit, sql, "", this.Context.Database.GetOrderBySql(model.Orders));

            var dictionarys100A03 = await this.moduleDictionaryService.GetModuleDictionaryAsync("100A03");

            foreach (var item in data)
            {
                // 评议类型：致贫风险  对应脱贫不稳定户、边缘易致贫户、突发严重困难户、脱贫不享受政策户
                if (item.HouseholdType == "100A03001" || item.HouseholdType == "100A03002" || item.HouseholdType == "100A03003" || item.HouseholdType == "100A03A02")
                {
                    item.ReviewType = "致贫风险";
                }
                else
                {
                    item.ReviewType = "";
                }
                var dic100A03 = dictionarys100A03.FirstOrDefault(a => a.Code == item.HouseholdType);
                if (dic100A03 != null)
                {
                    item.HouseholdType = dic100A03.Name;
                }
                item.Mobile = BasicSO.Decrypt(item.Mobile);
            }

            return data;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> SaveWorkflow(FFPWorkflow model)
        {
            if (model == null)
            {
                throw new ValidException("无效参数");
            }

            if (model.Id == 0)
            {
                var ret = await this.InsertAsync(model);
                if (ret != null)
                {

                    var name = await this.autoNumberService.GetWorkflowAutoNumber();
                    var feedback = new FFPFeedback()
                    {
                        Reason = "",
                        UpdatedBy = model.CreatedBy,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        CreatedBy = model.CreatedBy,
                        NotifyType = "风险上报",
                        IsDeleted = 0,
                        Status = (int)DVS.Models.Enum.FeedbackStatusEnum.TobeReview,
                        WorkflowId = ret.Id,
                        Name = name,
                        FlowStatus = ret.FlowStatus,
                    };
                    await this.feedbackService.InsertAsync(feedback);
                }
                return ret != null ? ret.Id : 0;
            }
            else
            {
                return await this.GetQueryable().Where(a => a.Id == model.Id).UpdateFromQueryAsync(a => model);
            }
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status">1摸排确认2待评议3公示4待上报乡镇9结束</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> UpdateStatus(int id, int status, int userId)
        {
            var data = await GetAsync(a => a.Id == id && a.IsDeleted == 0);
            if (data == null)
            {
                throw new ValidException("数据不存在");
            }

            data.FlowStatus = status;
            data.UpdatedAt = DateTime.Now;
            data.UpdatedBy = userId;
            var ret = await this.UpdateAsync(data);
            if (status == (int)WorkflowStatus.MopaiPassed)
            {
                // 产生申请书
                await ffpApplicationService.SaveApplication(new FFPApplicationEditReq()
                {
                    WorkflowId = data.Id,
                    HouseholdId = data.HouseholdId,
                    YearIncome = data.YearIncome
                }, userId);
            }
            return ret;
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ReviewDetailDto> DetailWorkflow(int id)
        {
            ReviewDetailDto ret = new ReviewDetailDto();
            var data = await this.GetAsync(a => a.Id == id && a.IsDeleted == 0);
            if (data == null)
            {
                throw new ValidException("数据不存在");
            }
            string sql = string.Format(@"select   w.id,  a.`name` as areaName, f.householdId, f.householdType, 
                            h.houseName, h.houseNumber, p.mobile, p.realName as householdMan,f.id as hid, 
                            ifnull(t1.peopleCount, 0) as peopleCount, g.moPaiDate as checkDate , 
                            '' checkPerson,  ifnull(w.povertyRisk, '') as  povertyRisk,  
                            w.povertyReason, ifnull(b.reason , '')  as reviewInfo, 
                            w.voteCount, w.agree, w.disagree, 
							ifnull(b.result, '') as result, '' as  images, 0 as yearIncome, '' as householdTags, '' as address   
                            from  FFPWorkflow w
                            left join FFPMoPaiLog g on w.moPaiId = g.id
                            left join FFPFeedback b on b.workflowId = w.id  and b.type = w.flowStatus
                            left join FFPHouseholdCode f on w.householdId = f.householdId and f.isDeleted = 0
                            left join VillageHouseholdCode h on h.id = w.householdId and h.isDeleted = 0
                            left join  VillageHouseCodeMember m on h.id = m.householdId and m.isHouseholder = 1 and m.isDeleted = 0
                            left join  VillagePopulation p on m.populationId = p.id and p.isDeleted = 0
                            left join  BasicArea a on a.id = h.areaId
                            left join(
                                   select n.householdId, COUNT(n.id) as peopleCount from VillageHouseCodeMember n
                                    where n.isDeleted = 0 and n.householdId > 0  group by n.householdId
                            )t1 on w.householdId = t1.householdId
                            where w.id = {0}", id);

            var s = await this.Context.Database.SqlQueryAsync<ReviewDetailDto>(sql);
            if (s.Count() > 0)
            {
                ret = s[0];
                if (ret.Hid > 0)
                {
                    var ad = await this.populationAddressService.GetAddressDetail(ret.Hid, PopulationAddressTypeEnum.防返贫家庭地址);
                    if (ad != null)
                    {
                        ret.Address = ad.Address;
                    }
                }
                ret.Mobile = BasicSO.Decrypt(ret.Mobile);
                var ids = new List<int>();
                ids.Add(ret.HouseholdId);
                var tagNames = await this.householdCodeTagService.GetTags(ids);
                if (tagNames != null)
                {
                    var tags = tagNames.Select(a => a.Name);
                    ret.HouseholdTags = string.Join(",", tags);
                }
                var dictionarys100A03 = await this.moduleDictionaryService.GetModuleDictionaryAsync("100A03");
                var dic100A03 = dictionarys100A03.FirstOrDefault(a => a.Code == ret.HouseholdType);
                if (dic100A03 != null)
                {
                    ret.HouseholdType = dic100A03.Name;
                }
            }
            else
            {
                ret = null;
            }
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<IPagedList<FFPPublicityManage>> GetPublicityManageList(PublicityManageListReq model, int type)
        {

            Expression<Func<FFPPublicityManage, bool>> filter = a => a.IsDeleted == 0 && a.AreaId == model.AreaId && a.Type == type;
            Expression<Func<FFPPublicityManage, object>> orderby = a => a.CreatedAt;

            if (!model.Keyword.IsNullOrEmpty())
            {
                filter = filter.And(a => a.CreatedUser.Contains(model.Keyword));
            }

            var data = await publicityManageService.GetPagedListAsync(filter, orderBy: orderby, model.Page, model.Limit, asc: false);

            return new StaticPagedList<FFPPublicityManage>(data, model.Page, model.Limit, data.TotalItemCount);

        }

        public async Task<IPagedList<FFPPublicityHousehold>> GetPublicityListManageList(PublicityManageListReq model)
        {
            Expression<Func<FFPPublicityHousehold, bool>> filter = a => a.IsDeleted == 0 && a.AreaId == model.AreaId;
            Expression<Func<FFPPublicityHousehold, object>> orderby = a => a.CreatedAt;
            bool asc = false;
            if (model.Orders != null && model.Orders.Count() > 0)
            {
                var t = model.Orders[0];
                if (t.FieldName.ToLower() == "housenumber")
                {
                    orderby = a => a.HouseNumber;
                }
                if (t.FieldName.ToLower() == "housename")
                {
                    orderby = a => a.HouseName;
                }
                if (t.Sort.ToLower() == "asc")
                {
                    asc = true;
                }
                if (t.Sort.ToLower() == "desc")
                {
                    asc = false;
                }
            }
            if (!model.Keyword.IsNullOrEmpty())
            {
                filter = filter.And(a => a.HouseName.ToLower().Contains(model.Keyword)
                || a.HouseNumber.ToLower().Contains(model.Keyword)
                || a.HouseholdMan.ToLower().Contains(model.Keyword));
            }
            var data = await publicityHouseholdService.GetPagedListAsync(filter, orderBy: orderby, model.Page, model.Limit, asc: asc);

            return new StaticPagedList<FFPPublicityHousehold>(data, model.Page, model.Limit, data.TotalItemCount);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PublicityListDetailDto> GetPublicityListDetail(int id)
        {
            PublicityListDetailDto ret = new PublicityListDetailDto();
            var data = await publicityHouseholdService.GetAsync(a => a.Id == id && a.IsDeleted == 0);
            if (data == null)
            {
                throw new ValidException("数据不存在");
            }
            string sql = string.Format(@"select s.id, s.householdId,a.`name` as areaName,  s.householdMan,s.houseName,s.houseNumber,
                            s.mobile,s.householdType,s.peopleCount, f.id as hid,
		                    ifnull(b.result, '') as AuditResult,ifnull(b.reason, '') as remark 
                            from  FFPPublicityHousehold s
                            left join FFPWorkflow w on w.householdId = s.householdId and w.flowStatus !=9
                            left join FFPFeedback b on b.workflowId = w.id  and b.type = w.flowStatus
                            left join FFPHouseholdCode f on w.householdId = f.householdId and f.isDeleted = 0 
                            left join  BasicArea a on a.id = s.areaId                          
                            where s.id = {0}", id);

            var s = await this.Context.Database.SqlQueryAsync<PublicityListDetailDto>(sql);
            if (s.Count() > 0)
            {
                ret = s[0];
                if (ret.Hid > 0)
                {
                    var ad = await this.populationAddressService.GetAddressDetail(ret.Hid, PopulationAddressTypeEnum.防返贫家庭地址);
                    if (ad != null)
                    {
                        ret.Address = ad.Address;
                    }
                }
                ret.Mobile = BasicSO.Decrypt(ret.Mobile);
                var ids = new List<int>();
                ids.Add(ret.HouseholdId);
                var tagNames = await this.householdCodeTagService.GetTags(ids);
                if (tagNames != null)
                {
                    var tags = tagNames.Select(a => a.Name);
                    ret.HouseholdTags = string.Join(",", tags);
                }
                var dictionarys100A03 = await this.moduleDictionaryService.GetModuleDictionaryAsync("100A03");
                var dic100A03 = dictionarys100A03.FirstOrDefault(a => a.Code == ret.HouseholdType);
                if (dic100A03 != null)
                {
                    ret.HouseholdType = dic100A03.Name;
                }
            }
            else
            {
                ret = null;
            }
            return ret;
        }


        public async Task<IEnumerable<FFPPublicityHousehold>> GetPublicityHouseholdList(int id)
        {
            var ret = await publicityHouseholdService.GetListAsync(a => a.PublicityManageId == id);
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IPagedList<FFPWorkflowListDto>> GetWorkflowPageListAsync(WorkflowListQueryModel model, int userId)
        {
            string sql = string.Format(@"SELECT 
                                        w.id,
                                        IFNULL(f.`name`,'') as `name`,
                                        IFNULL(f.id,0) as feedbackId,
                                        u.nickName as CreatedByName,
                                        w.createdAt,
                                        IFNULL(f.`status`,0) as `status`,
                                        IFNULL(f.`NotifyType`,'风险上报') as `NotifyType`,
                                        w.householdId
                                        FROM FFPWorkflow w
                                        INNER JOIN FFPMatrix fm on w.matrixId = fm.id  and  (fm.inspector RLIKE '(^|,){0}(,|$)' or fm.inspectorManager RLIKE '(^|,){0}(,|$)')
                                        LEFT JOIN FFPFeedback f on w.id = f.workflowId and f.isDeleted=0
                                        LEFT JOIN BasicUser u on w.createdBy = u.id and u.isDeleted=0
                                        WHERE w.isDeleted=0 ", userId);

            if (!string.IsNullOrWhiteSpace(model.Keyword))
            {

            }

            if (model.StatusList != null && model.StatusList.Length > 0)
            {

                sql += string.Format(" AND f.`status` in({0}) ", string.Join(',', model.StatusList));
            }

            var res = this.Context.Database.SqlQueryPagedList<FFPWorkflowListDto>(model.Page, model.Limit, sql, "", this.Context.Database.GetOrderBySql(model.Orders));
            foreach (var item in res)
            {
                item.CreatedByName = BasicSO.Decrypt(item.CreatedByName);
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workflowId"></param>
        /// <param name="feedbackId"></param>
        /// <returns></returns>
        public async Task<FFPWorkflowDetailDto> GetWorkflowDetailAsync(int workflowId, int feedbackId)
        {

            string sql = string.Format(@"   SELECT 
                                            w.id,
                                            w.matrixId,
                                            w.householdId,
                                            w.moPaiId,
                                            w.`name` as workflowName,                                      
                                            w.`describe`,
                                            w.flowStatus,
                                            u.nickName as CreatedByName,
                                            w.createdAt,
                                            f.result,
                                            f.info,
                                            m.voiceUrl,
                                            m.images,
                                            NULL as ImageFiles,
                                            NULL as VoiceFiles,
                                            IFNULL(f.id,0) as feedbackId,
                                            IFNULL(f.`status`,0) as `status`,
                                            IFNULL(f.`NotifyType`,'风险上报') as `NotifyType` FROM FFPWorkflow w
                                            LEFT JOIN FFPFeedback f on w.id= f.workflowId and f.isDeleted=0
                                            LEFT JOIN BasicUser u on w.createdBy = u.id and u.isDeleted=0
                                            LEFT JOIN FFPMoPaiLog m on w.moPaiId = m.id
                                            WHERE w.isDeleted=0
                                            AND w.id={0} ", workflowId);

            if (feedbackId > 0)
            {
                sql += string.Format(" AND f.id={0} ", feedbackId);
            }

            var res = (await this.Context.Database.SqlQueryAsync<FFPWorkflowDetailDto>(sql)).FirstOrDefault();

            if (res != null && feedbackId <= 0)
            {
                var feedback = await this.feedbackService.GetAsync(a=>a.WorkflowId==res.Id&& a.FlowStatus ==res.FlowStatus&&a.IsDeleted==0);
                if (feedback != null) {

                    res.FeedbackId = feedback.Id;
                    res.Info = feedback.Info;
                    res.Result = feedback.Result;
                
                }
            }

            if (res != null) {

                var files = await this.sunFileService.GetSunFileInfoList(res.Images);
                res.ImageFiles = files;
                files = await this.sunFileService.GetSunFileInfoList(res.VoiceUrl);
                res.VoiceFiles = files;

            }
            

            return res;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<bool> RejectWorkflowAsync(RejectWorkflowBody body, int userId)
        {

            int flowStatus = (int)WorkflowStatus.Rejected;
            int updatedBy = userId;
            int status = (int)FeedbackStatusEnum.TobeFeedback;
            

            var data = await this.GetAsync(body.Id);
            if (data == null)
            {
                throw new ValidException("数据不存在");
            }
            if (data.FlowStatus != (int)WorkflowStatus.Submit)
            {
                throw new ValidException("不能进行此操作");
            }
            string resultDescribe = $"你上报的致贫风险户：{data.Name}家，经村委认定不予通过，请告知相关村民。";
            var res = await this.Context.Database.ExecuteSqlRawAsync(" UPDATE FFPWorkflow w SET flowStatus={0},updatedBy={1} WHERE w.id={2} ", flowStatus, updatedBy, body.Id);
            if (res > 0)
            {
                await this.Context.Database.ExecuteSqlRawAsync(" UPDATE FFPFeedback w SET flowStatus={0},updatedBy={1},status={2},reason={3},resultDescribe={4} WHERE w.id={5} ", flowStatus, updatedBy, status, body.Reason, resultDescribe, body.FeedbackId);
            }
            return res > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<bool> AcceptWorkflowAsync(AcceptWorkflowBody body, int userId)
        {
            var workflow = await this.GetAsync(a => a.Id == body.Id && a.IsDeleted == 0);
            if (workflow == null)
            {
                throw new ValidException("数据不存在");
            }
            if (workflow.FlowStatus != (int)WorkflowStatus.Submit)
            {
                throw new ValidException("不能进行此操作");
            }

            int updatedBy = userId;
            int status = (int)FeedbackStatusEnum.TobeFeedback;
            string resultDescribe = $"经过村委认定，{workflow.Name}的返贫无异议，请进入评议通过名单，进行信息采集和填写。";

            workflow.UpdatedAt = DateTime.Now;
            workflow.UpdatedBy = updatedBy;
            workflow.FlowStatus = (int)WorkflowStatus.MopaiPassed;
            workflow.YearIncome = body.YearIncome;
            workflow.PovertyRisk = body.PovertyRisk;
            workflow.PovertyReason = body.PovertyReason;
            var res = await this.UpdateAsync(workflow);
            if (res > 0)
            {
                // 产生申请书
                await ffpApplicationService.SaveApplication(new FFPApplicationEditReq()
                {
                    WorkflowId = workflow.Id,
                    HouseholdId = workflow.HouseholdId,
                    YearIncome = workflow.YearIncome
                }, userId);


                await this.Context.Database.ExecuteSqlRawAsync(" UPDATE FFPFeedback w SET updatedBy={0},status={1},flowStatus={2},resultDescribe={3} WHERE w.id={4} ", updatedBy, status, workflow.FlowStatus, resultDescribe, body.FeedbackId);
                await this.householdCodeService.UpdateFFPHouseholdCodeInfo(body.HouseholdId, body.HouseholdType, body.FamilyAddressInfo, userId);
            }
            return res > 0;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="workflowId"></param>
        /// <param name="feedbackId"></param>
        /// <returns></returns>
        public async Task<bool> ConfirmWorkflowAsync(ConfirmWorkflowBody body, int userId)
        {
            var workflow = await this.GetAsync(a => a.Id == body.Id && a.IsDeleted == 0);
            if (workflow == null)
            {
                throw new ValidException("数据不存在");
            }

            int updatedBy = userId;
            int status = (int)FeedbackStatusEnum.Confirmed;
            var res = await this.Context.Database.ExecuteSqlRawAsync(" UPDATE FFPFeedback w SET updatedBy={0},status={1} WHERE w.id={2} ", updatedBy, status, body.FeedbackId);
            return res > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workflowId"></param>
        /// <param name="feedbackId"></param>
        /// <returns></returns>
        public async Task<FFPWorkflowHouseholdDto> GetWorkflowHouseholdDetailAsync(int workflowId, int feedbackId)
        {

            string sql = string.Format(@"SELECT
                                        w.id,
                                        w.moPaiId,
                                        w.matrixId,
                                        w.`name` as workflowName,
                                        w.images,
                                        w.povertyRisk,
                                        w.povertyReason,
                                        w.yearIncome,
                                        w.`describe`,
                                        w.flowStatus,
                                        w.voteCount,
                                        w.agree,
                                        w.disagree,
                                        IFNULL(f.result, '') as result,
                                        IFNULL(f.reason,'') as reason,
                                        0 as householdMemberCount,
                                        t.*,
                                        fh.householdType,
                                        NULL as ImageFiles
                                        FROM FFPWorkflow w
                                        LEFT JOIN FFPFeedback f  on f.workflowId=w.id and f.isDeleted=0
                                        LEFT JOIN (
                                        SELECT 
                                        p.realName as householdName,
                                        m.householdId,
                                        p.mobile as householdMobile,
                                        p.headImageId,
                                        p.headImageUrl,
                                        CONCAT(a.`name`,h.houseName,h.houseNumber) as householdNumber
                                        FROM VillageHouseholdCode h
                                        LEFT JOIN BasicArea a on h.areaId = a.id and a.IsDeleted=0
                                        LEFT JOIN VillageHouseCodeMember m on h.id= m.householdId  and m.IsDeleted=0 and m.isHouseholder=1
                                        LEFT JOIN VillagePopulation p on m.populationId = p.id and p.IsDeleted=0
                                        WHERE h.IsDeleted=0
                                        )t on w.householdId = t.householdId
                                        LEFT JOIN FFPHouseholdCode fh on w.householdId = fh.householdId and fh.IsDeleted=0
                                        WHERE w.isDeleted=0 and w.id={0} ", workflowId);
            if (feedbackId > 0)
            {
                sql += string.Format(" and f.id={0} ", feedbackId);

            }
            var data = (await this.Context.Database.SqlQueryAsync<FFPWorkflowHouseholdDto>(sql)).FirstOrDefault();
            if (data != null)
            {
                // string sqlCount = "SELECT COUNT(*) as householdMemberCount FROM VillageHouseCodeMember m WHERE  m.IsDeleted=0 and m.householdId=" + data.HouseholdId;
                data.HouseholdName = BasicSO.Decrypt(data.HouseholdName);
                data.HouseholdMobile = BasicSO.Decrypt(data.HouseholdMobile);
                var query = from m in this.memberService.GetQueryable() where m.IsDeleted == 0 && m.HouseholdId == data.HouseholdId select m;
                data.HouseholdMemberCount = await query.CountAsync();
                data.HeadImageUrl = this.sunFileService.ToAbsolutePath(data.HeadImageUrl);
                if (feedbackId <= 0)
                {
                    data.Result = data.FlowStatus == (int)WorkflowStatus.Submit ? "" : (data.FlowStatus == (int)WorkflowStatus.Rejected ? "通过" : "不通过");



                    var feedback = await this.feedbackService.GetAsync(a => a.WorkflowId == data.Id && a.FlowStatus == data.FlowStatus && a.IsDeleted == 0);
                    if (feedback != null)
                    {

                        // data.FeedbackId = feedback.Id;
                        data.Reason = feedback.Reason;
                        data.Result = feedback.Result;

                    }
                }
                var files = await this.sunFileService.GetSunFileInfoList(data.Images);
                data.ImageFiles = files;
            }
            return data;
        }



        /// <summary>
        /// 待评审名单中，录入评议结果
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<bool> AuditReviewWorkflowAsync(ReviewWorkflowBody body, int userId) {
            var workflow = await this.GetAsync(a => a.Id == body.Id && a.IsDeleted == 0);
            if (workflow == null)
            {
                throw new ValidException("数据不存在");
            }

            if (workflow.FlowStatus != (int)WorkflowStatus.MopaiPassed)
            {
                throw new ValidException("不是公示状态不能进行此操作");
            }
            int status = (int)FeedbackStatusEnum.ToBeConfirm;
            int flowStatus = (int)WorkflowStatus.Rejected;
            string resultDescribe = "经过村委会议评议、{name}的返贫风险有异议，已被打回，请告知相关村民。";

            if (body.Result == "通过")
            {
                flowStatus = (int)WorkflowStatus.Publicity;
                resultDescribe = "你上报的致贫风险户：{name}家，已通过民主评议，请前往评议通过名单，进行信息采集和填写。";

            }
            else {
                body.Result = "不通过";
            }

            workflow.UpdatedAt = DateTime.Now;
            workflow.UpdatedBy = userId;
            workflow.FlowStatus = flowStatus;
            workflow.Agree = body.Agree;
            workflow.Disagree = body.Disagree;
            workflow.Images = body.Images;
            workflow.VoteCount = body.VoteCount;

            var ret = await this.UpdateAsync(workflow);
            if (ret > 0)
            {
                var name = await this.autoNumberService.GetWorkflowAutoNumber();
                var feedback = new FFPFeedback()
                {
                    Reason = body.Reason,
                    Result = body.Result,
                    UpdatedBy = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = userId,
                    NotifyType = "网格员反馈",
                    IsDeleted = 0,
                    Status = status,
                    WorkflowId = body.Id,
                    Name = name,
                    FlowStatus = flowStatus,
                    ResultDescribe = resultDescribe.Replace("{name}", workflow.Name),
                };
                await this.feedbackService.InsertAsync(feedback);
            }
            return ret > 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<bool> AuditPublicityWorkflowAsync(PublicityWorkflow body, int userId)
        {
            if (string.IsNullOrWhiteSpace(body.Reason))
            {
                throw new ValidException("请输入原因");
            }

            var workflow = await this.GetAsync(a => a.Id == body.Id && a.IsDeleted == 0);
            if (workflow == null)
            {
                throw new ValidException("数据不存在");
            }

            if (workflow.FlowStatus != (int)WorkflowStatus.Publicity)
            {
                throw new ValidException("不是公示状态不能进行此操作");
            }

            int status = (int)FeedbackStatusEnum.ToBeConfirm;
            int flowStatus = (int)WorkflowStatus.Rejected;
            string resultDescribe = "经过村栏目公示、{name}家的返贫风险有异议，已被打回，请告知相关村民。";

            if (body.Result == "通过")
            {
                flowStatus = (int)WorkflowStatus.SubmitDistrict;
                resultDescribe = "经过村栏目公示、{name}家的返贫无异议，请进入评议通过名单，进行信息采集和填写。";

            }
            else
            {
                body.Result = "不通过";
            }

            workflow.UpdatedAt = DateTime.Now;
            workflow.UpdatedBy = userId;
            workflow.FlowStatus = flowStatus;
            var ret = await this.UpdateAsync(workflow);
            if (ret > 0)
            {
                var name = await this.autoNumberService.GetWorkflowAutoNumber();
                var feedback = new FFPFeedback()
                {
                    Reason = body.Reason,
                    Result = body.Result,
                    UpdatedBy = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = userId,
                    NotifyType = "网格员反馈",
                    IsDeleted = 0,
                    Status = status,
                    WorkflowId = body.Id,
                    Name = name,
                    FlowStatus = flowStatus,
                    ResultDescribe = resultDescribe.Replace("{name}", workflow.Name),
                };
                await this.feedbackService.InsertAsync(feedback);
            }
            return ret > 0;

        }
    }
}
