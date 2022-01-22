using DVS.Application.Services.FFP;
using DVS.Common.Models;
using DVS.Core.Domains.FFP;
using DVS.Models.Dtos.FFP;
using DVS.Models.Dtos.FFP.Query;
using DVS.Models.Dtos.FFP.Submit;
using DVS.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace DVS.FFP.Api.Controllers
{
    /// <summary>
    /// 工作流操作，主任务，网格员长操作
    /// </summary>
    [ApiController]
    [Route("api/ffp")]
    public class WorkflowController : DvsControllerBase
    {

        IFFPWorkflowService workflowService;
        IFFPFeedbackService feedbackService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="workflowService"></param>
        /// <param name="feedbackService"></param>
        public WorkflowController(IFFPWorkflowService workflowService, IFFPFeedbackService feedbackService)
        {
            this.workflowService = workflowService;
            this.feedbackService = feedbackService;
        }


        ///// <summary>
        ///// 保存
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost("app/workflow/Save")]
        ////[AllowAnonymous]
        //public async Task<int> SaveWorkflowAsync(FFPWorkflow model)
        //{

        //    LoginUser loginUser = this.LoginUser;
        //    model.CreatedBy = loginUser.UserId;
        //    return await this.workflowService.SaveWorkflow(model);
        //}

        /// <summary>
        /// 更新状态
        /// </summary>
        ///  <param name="id"></param>
        /// <param name="status">1摸排确认2待评议3公示4待上报乡镇9结束</param>
        /// <returns></returns>
        [HttpPost("app/workflow/UpdateStatus")]
        //[AllowAnonymous]
        public async Task<int> UpdateStatusAsync(int id, int status)
        {
            LoginUser loginUser = this.LoginUser;
            return await this.workflowService.UpdateStatus(id, status, loginUser.UserId);
        }

        /// <summary>
        /// 处理任务统计 网格员长。
        /// </summary>
        /// <returns></returns>
        [HttpGet("app/workflow/GetWorkflowDataViewAsync")]
        // [AllowAnonymous]
        public async Task<WorkflowDataViewDto> GetWorkflowDataViewAsync()
        {
            int userId = LoginUser.UserId;
            string status1 = ((int)FeedbackStatusEnum.TobeReview).ToString() + "," + ((int)FeedbackStatusEnum.ToBeConfirm).ToString() + "," + ((int)FeedbackStatusEnum.TobeFeedback).ToString() + "," + ((int)FeedbackStatusEnum.Feedbacked).ToString();
            var res = await this.feedbackService.GetFeedbackDataViewAsync(status1, ((int)FeedbackStatusEnum.Confirmed).ToString(), userId);
            return res;
        }

        /// <summary>
        /// 主任务列表。
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("app/workflow/GetWorkflowPageListAsync")]
        // [AllowAnonymous]
        public async Task<IPagedList<FFPWorkflowListDto>> GetWorkflowPageListAsync(WorkflowListQueryModel model)
        {
            int userId = LoginUser.UserId;
            var res = await this.workflowService.GetWorkflowPageListAsync(model,userId);
            return res;
        }

        /// <summary>
        /// 获取详情。
        /// </summary>
        /// <param name="workflowId"></param>
        /// <param name="feedbackId"></param>
        /// <returns></returns>
        [HttpGet("app/workflow/GetWorkflowDetailAsync")]
        // [AllowAnonymous]
        public async Task<FFPWorkflowDetailDto> GetWorkflowDetailAsync(int workflowId,int feedbackId)
        {
            var res = await this.workflowService.GetWorkflowDetailAsync(workflowId, feedbackId);
            return res;
        }

        /// <summary>
        ///  待评议，待公示名单的 详情。
        /// </summary>
        /// <param name="workflowId"></param>
        /// <param name="feedbackId"></param>
        /// <returns></returns>
        [HttpGet("app/workflow/GetWorkflowHouseholdDetailAsync")]
        // [AllowAnonymous]
        public async Task<FFPWorkflowHouseholdDto> GetWorkflowHouseholdDetailAsync(int workflowId, int feedbackId)
        {
            var res = await this.workflowService.GetWorkflowHouseholdDetailAsync(workflowId, feedbackId);
            return res;
        }


        /// <summary>
        /// 审核通过消息。
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("app/workflow/AcceptWorkflowAsync")]
        //[AllowAnonymous]
        public async Task<bool> AcceptWorkflowAsync(AcceptWorkflowBody body)
        {
            int userId = LoginUser.UserId;
            var res = await this.workflowService.AcceptWorkflowAsync(body, userId);
            return res; ;
        }

        /// <summary>
        /// 审核不同通过。
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("app/workflow/RejectWorkflowAsync")]
        //[AllowAnonymous]
        public async Task<bool> RejectWorkflowAsync(RejectWorkflowBody body)
        {
            int userId = LoginUser.UserId;
            var res = await this.workflowService.RejectWorkflowAsync(body, userId);
            return res; ;
        }



        /// <summary>
        /// 处理任务 网格长确认信息。
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("app/workflow/ConfirmWorkflowAsync")]
        //[AllowAnonymous]
        public async Task<bool> ConfirmWorkflowAsync(ConfirmWorkflowBody body)
        {
            int userId = LoginUser.UserId;
            var res = await this.workflowService.ConfirmWorkflowAsync(body, userId);
            return res;
        }

        /// <summary>
        /// 待评审名单中，录入评议结果。
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("app/workflow/AuditReviewWorkflowAsync")]
        // [AllowAnonymous]
        public async Task<bool> AuditReviewWorkflowAsync(ReviewWorkflowBody body)
        {
            int userId = LoginUser.UserId;
            var res = await this.workflowService.AuditReviewWorkflowAsync(body, userId);
            return res;
        }

        /// <summary>
        /// 公示名单中，审核，
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("app/workflow/AuditPublicityWorkflowAsync")]
        // [AllowAnonymous]
        public async Task<bool> AuditPublicityWorkflowAsync(PublicityWorkflow body)
        {
            int userId = LoginUser.UserId;
            var res = await this.workflowService.AuditPublicityWorkflowAsync(body, userId);
            return res;
        }

        /// <summary>
        /// 1 生产公示文章，2生产公示报告
        /// </summary>
        /// <param name="body"></param>
        /// <returns>返回文章的Id</returns>
        //[HttpPost("app/workflow/CreatePublicArticleAsync")]
        //[AllowAnonymous]
        //public async Task<int> CreatePublicArticleAsync(CreatePublicArticleBody body)
        //{

        //    return 0;
        //}
    }
}
