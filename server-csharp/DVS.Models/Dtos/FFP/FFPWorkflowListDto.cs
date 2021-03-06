using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{

    /// <summary>
    /// 主任务处理任务列表（网格员长）
    /// </summary>
    public class FFPWorkflowListDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 通知标题
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; } = 0;

        /// <summary>
        /// 反馈表Id
        /// </summary>
        public long FeedbackId { get; set; } = 0;

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedByName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 状态0待审核 1待反馈2已反馈3待确认4已确认
        /// </summary>
        public long Status { get; set; } = 0;

        /// <summary>
        /// 通知类型
        /// </summary>
        public string NotifyType { get; set; }



    }
}
