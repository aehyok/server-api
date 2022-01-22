using DVS.Application.Services.FFP;
using DVS.Common.Models;
using DVS.Core.Domains.FFP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DVS.FFP.Api.Controllers
{
    /// <summary>
    /// 反馈操作
    /// </summary>
    [ApiController]
    [Route("api/ffp")]
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
