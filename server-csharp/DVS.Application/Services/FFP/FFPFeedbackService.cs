using AutoMapper;
using DVS.Application.Services.Common;
using DVS.Common.Models;
using DVS.Common.Services;
using DVS.Common.SO;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.FFP.Query;
using DVS.Models.Dtos.FFP.Submit;
using DVS.Models.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.FFP
{
    public class FFPFeedbackService : ServiceBase<FFPFeedback>, IFFPFeedbackService
    {

        private readonly IBasicUserService basicUserService;
        public FFPFeedbackService(DbContext dbContext, IMapper mapper, IBasicUserService basicUserService)
        : base(dbContext, mapper)
        {
            this.basicUserService = basicUserService;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> SaveFeedback(FFPFeedback model)
        {
            return 0;
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status">1待反馈2已反馈3已确认</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<int> UpdateStatus(int id, int status, int userId)
        {
            var data = await GetAsync(a => a.Id == id && a.IsDeleted == 0);
            if (data == null)
            {
                throw new ValidException("数据不存在");
            }

            data.Status = status;
            data.UpdatedAt = DateTime.Now;
            data.UpdatedBy = userId;
            var ret = await this.UpdateAsync(data);
            return ret;
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FFPFeedback> DetailFeedback(int id)
        {
            var data = await this.GetAsync(a => a.Id == id && a.IsDeleted == 0);
            if (data == null)
            {
                throw new ValidException("数据不存在");
            }
            return data;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IPagedList<FFPFeedbackLisDto>> GetFeedbackPageListAsync(FeedbackListQueryModel model, int userId)
        {
            string sql = string.Format(@"SELECT 
                                         f.id,
                                         f.`name`,
                                         f.createdAt,
                                         f.`status`,
                                         f.notifyType,
                                         u.nickName as createdByName
                                         FROM FFPFeedback f
                                         LEFT JOIN FFPWorkflow w on f.workflowId= w.id and w.isDeleted=0
                                         LEFT JOIN BasicUser u on f.createdBy = u.id and u.isDeleted=0
                                         INNER JOIN FFPMatrix fm on w.matrixId = fm.id  and  (fm.inspector RLIKE '(^|,){0}(,|$)' or fm.inspectorManager RLIKE '(^|,){0}(,|$)')
                                         WHERE f.isDeleted=0 ", userId);

            if (!string.IsNullOrWhiteSpace(model.Keyword))
            {

            }

            if (model.StatusList != null && model.StatusList.Length > 0)
            {

                sql += string.Format(" AND (f.`status` in({0})) ", string.Join(',', model.StatusList));
            }

            var datas = this.Context.Database.SqlQueryPagedList<FFPFeedbackLisDto>(model.Page, model.Limit, sql, "", this.Context.Database.GetOrderBySql(model.Orders));
            if (datas != null)
            {
                foreach (var item in datas)
                {
                    item.CreatedByName = BasicSO.Decrypt(item.CreatedByName);
                }
            }
            return datas;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feedbackId"></param>
        /// <returns></returns>
        public async Task<FFPFeedbackDto> GetFeedbackDetailAsync(int feedbackId)
        {
            string sql = string.Format(@"SELECT 
                                         f.id,
                                         f.`name`,
                                         f.createdAt,
                                         f.`status`,
                                         f.notifyType,
                                         f.info,
                                         f.reason,
                                         f.resultDescribe,
                                         u.nickName as createdByName,
                                         w.householdId,
                                         w.`name`,
                                         w.`name` as workflowName
                                         FROM FFPFeedback f
                                         LEFT JOIN FFPWorkflow w on f.workflowId= w.id and w.isDeleted=0
                                         LEFT JOIN BasicUser u on f.createdBy = u.id and u.isDeleted=0
                                         WHERE f.isDeleted=0 AND f.id={0}", feedbackId);
            var data = this.Context.Database.SqlQuery<FFPFeedbackDto>(sql).FirstOrDefault();
            if (data != null)
            {
                data.CreatedByName = BasicSO.Decrypt(data.CreatedByName);
            }
            return data;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> FeedbackInfoAsync(FeedbackInfoSubmit model, int userId) {
            int status = (int)FeedbackStatusEnum.Feedbacked;
            if (model.Id <= 0) {
                throw new ValidException("Id不合法");
            }
            if (string.IsNullOrWhiteSpace(model.Info)) {

                throw new ValidException("请输入反馈信息");
            }
            var res = await this.Context.Database.ExecuteSqlRawAsync(" UPDATE FFPFeedback f SET status={0},updatedBy={1},Info={2} WHERE f.id={3} ", status, userId, model.Info, model.Id);
            return res > 0;
        }


        public async Task<WorkflowDataViewDto> GetFeedbackDataViewAsync(string status1,string status2, int userId) {

            string sql = string.Format(@"SELECT * FROM 																						
                                        (SELECT COUNT(f.`status`) as PendingCount FROM FFPFeedback f 
                                        LEFT JOIN FFPWorkflow w on f.workflowId= w.id and w.isDeleted=0
                                        INNER JOIN FFPMatrix fm on w.matrixId = fm.id  and  (fm.inspector RLIKE '(^|,){0}(,|$)' or fm.inspectorManager RLIKE '(^|,){0}(,|$)')
                                        WHERE f.`status` in ({1}) AND f.isDeleted=0
                                        )t1,
                                        (SELECT COUNT(f.`status`) as ProcessedCount FROM FFPFeedback f 
                                        LEFT JOIN FFPWorkflow w on f.workflowId= w.id and w.isDeleted=0
                                        INNER JOIN FFPMatrix fm on w.matrixId = fm.id  and  (fm.inspector RLIKE '(^|,){0}(,|$)' or fm.inspectorManager RLIKE '(^|,){0}(,|$)')
                                        WHERE f.`status` in ({2}) AND f.isDeleted=0
                                        )t2", userId, status1, status2);
            var res = (await this.Context.Database.SqlQueryAsync<WorkflowDataViewDto>(sql)).FirstOrDefault();
            WorkflowDataViewDto dv = new WorkflowDataViewDto() { PendingCount = 0, ProcessedCount = 0 };
            if (res != null) {
                dv = res;
            }
            return dv;
        }

    }
}
