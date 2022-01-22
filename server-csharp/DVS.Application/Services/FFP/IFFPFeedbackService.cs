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
    public interface IFFPFeedbackService : IServiceBase<FFPFeedback>
    {

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> SaveFeedback(FFPFeedback model);

        /// <summary>
        /// 更新状态
        /// </summary>        
        /// <param name="id"></param>
        /// <param name="status">1待反馈2已反馈3已确认</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<int> UpdateStatus(int id, int status, int userId);

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<FFPFeedback> DetailFeedback(int id);

        /// <summary>
        /// 网格员待处理、已处理列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IPagedList<FFPFeedbackLisDto>> GetFeedbackPageListAsync(FeedbackListQueryModel model, int userId);


        /// <summary>
        /// 通知详情
        /// </summary>
        /// <param name="feedbackId"></param>
        /// <returns></returns>
        Task<FFPFeedbackDto> GetFeedbackDetailAsync(int feedbackId);

        /// <summary>
        /// 网格员回报信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> FeedbackInfoAsync(FeedbackInfoSubmit model,int userId);

        /// <summary>
        /// 处理任务统计
        /// </summary>
        /// <param name="status1"></param>
        /// <param name="status2"></param>
        /// <returns></returns>
        Task<WorkflowDataViewDto> GetFeedbackDataViewAsync(string status1, string status2, int userId);
        

        }
}
