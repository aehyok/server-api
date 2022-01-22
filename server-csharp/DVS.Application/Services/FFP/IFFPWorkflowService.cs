using DVS.Common.Services;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.FFP.Query;
using DVS.Models.Dtos.FFP.Submit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.Application.Services.FFP
{
    public interface IFFPWorkflowService : IServiceBase<FFPWorkflow>
    {

        Task<IPagedList<ReviewListDto>> GetReviewList(ReviewListReq model);

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> SaveWorkflow(FFPWorkflow model);

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status">1摸排确认2待评议3公示4待上报乡镇9结束</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<int> UpdateStatus(int id, int status, int userId);

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ReviewDetailDto> DetailWorkflow(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="type">类型  1评议公示2评议报告</param>
        /// <returns></returns>
        Task<IPagedList<FFPPublicityManage>> GetPublicityManageList(PublicityManageListReq model, int type);


        Task<IPagedList<FFPPublicityHousehold>> GetPublicityListManageList(PublicityManageListReq model);


        /// <summary>
        /// 取公示名单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<PublicityListDetailDto> GetPublicityListDetail(int id);

        /// <summary>
        /// 取评议公示的户明细列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<FFPPublicityHousehold>> GetPublicityHouseholdList(int id);


        /// <summary>
        /// 主任务列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IPagedList<FFPWorkflowListDto>> GetWorkflowPageListAsync(WorkflowListQueryModel model, int userId);

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="workflowId"></param>
        /// <param name="feedbackId"></param>
        /// <returns></returns>
        Task<FFPWorkflowDetailDto> GetWorkflowDetailAsync(int workflowId, int feedbackId);

        /// <summary>
        /// 审核不同通过
        /// </summary>
        /// <param name="body"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> RejectWorkflowAsync(RejectWorkflowBody body, int userId);

        /// <summary>
        /// 确认消息
        /// </summary>
        /// <param name="workflowId"></param>
        /// <param name="feedbackId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> ConfirmWorkflowAsync(ConfirmWorkflowBody body, int userId);

        /// <summary>
        /// 审核通过消息
        /// </summary>
        /// <param name="body"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> AcceptWorkflowAsync(AcceptWorkflowBody body, int userId);

        /// <summary>
        /// 待评议，待公示名单的 详情
        /// </summary>
        /// <param name="workflowId"></param>
        /// <param name="feedbackId"></param>
        /// <returns></returns>
        Task<FFPWorkflowHouseholdDto> GetWorkflowHouseholdDetailAsync(int workflowId, int feedbackId);

        /// <summary>
        /// 待评审名单中，录入评议结果
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        Task<bool> AuditReviewWorkflowAsync(ReviewWorkflowBody body, int userId);


        /// <summary>
        /// 公示名单中，审核
        /// </summary>
        /// <param name="body"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> AuditPublicityWorkflowAsync(PublicityWorkflow body, int userId);
    }
}
