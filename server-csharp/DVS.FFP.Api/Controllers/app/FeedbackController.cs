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

namespace DVS.FFP.Api.Controllers.app
{
    /// <summary>
    /// app 处理任务
    /// </summary>
    /// <summary>
    /// 反馈操作
    /// </summary>
    [ApiController]
    [Route("")]
    public class FeedbackController : DvsControllerBase
    {
        IFFPFeedbackService feedbackService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="feedbackService"></param>
        public FeedbackController(IFFPFeedbackService feedbackService)
        {
            this.feedbackService = feedbackService;
        }

        /// <summary>
        /// 处理任务列表。
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/ffp/app/feedback/GetFeedbackPageListAsync")]
        public async Task<IPagedList<FFPFeedbackLisDto>> GetFeedbackPageListAsync(FeedbackListQueryModel model) {
            var userId = LoginUser.UserId;
            var data = await this.feedbackService.GetFeedbackPageListAsync(model, userId);
            return data;
        }

        /// <summary>
        /// 获取任务详情。
        /// </summary>
        /// <param name="feedbackId"></param>
        /// <returns></returns>
        [HttpGet("api/ffp/app/feedback/GetFeedbackDetailAsync")]
        //[AllowAnonymous]
        public async Task<FFPFeedbackDto> GetFeedbackDetailAsync(int feedbackId)
        {
            var data = await this.feedbackService.GetFeedbackDetailAsync(feedbackId);
            return data;
        }

        /// <summary>
        /// 处理任务 网格员汇报信息。
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("api/ffp/app/feedback/FeedbackInfoAsync")]
        //[AllowAnonymous]
        public async Task<bool> FeedbackInfoAsync(FeedbackInfoSubmit model)
        {
            var userId = LoginUser.UserId;
            var res = await this.feedbackService.FeedbackInfoAsync(model, userId);
            return res;
        }


        /// <summary>
        /// 处理任务统计 网格员。
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/ffp/app/feedback/GetFeedbackDataViewAsync")]
        //[AllowAnonymous]
        public async Task<WorkflowDataViewDto> GetFeedbackDataViewAsync()
        {
            int userId = LoginUser.UserId;
            var res = await this.feedbackService.GetFeedbackDataViewAsync(((int)FeedbackStatusEnum.TobeFeedback).ToString(), ((int)FeedbackStatusEnum.Feedbacked).ToString(), userId);
            return res;
        }


        ///// <summary>
        ///// 处理任务 网格长确认信息
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost("api/ffp/app/feedback/FeedbackConfirmAsync")]
        //[AllowAnonymous]
        //public async Task<bool> FeedbackConfirmAsync(int id)
        //{
        //    return true;
        //}

        ///// <summary>
        ///// 保存
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost("app/feedback/Save")]
        ////[AllowAnonymous]
        //public async Task<int> SaveFeedbackAsync(FFPFeedback model)
        //{
        //    LoginUser loginUser = this.LoginUser;
        //    model.CreatedBy = loginUser.UserId;
        //    return await this.feedbackService.SaveFeedback(model);
        //}

        ///// <summary>
        ///// 更新状态
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="status">1待反馈2已反馈3已确认</param>
        ///// <returns></returns>
        //[HttpPost("app/feedback/UpdateStatus")]
        ////[AllowAnonymous]
        //public async Task<int> UpdateStatusAsync(int id, int status)
        //{
        //    LoginUser loginUser = this.LoginUser;
        //    return await this.feedbackService.UpdateStatus(id, status, loginUser.UserId);
        //}

        ///// <summary>
        ///// 获取详情
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpPost("app/feedback/Detail")]
        ////[AllowAnonymous]
        //public async Task<FFPFeedback> DetailFeedbackAsync(int id)
        //{
        //    return await this.feedbackService.DetailFeedback(id);
        //}
    }
}
