using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{

    /// <summary>
    /// 反馈任务详情
    /// </summary>
    public class FFPFeedbackDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 户码ID
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 主任务Id
        /// </summary>
        /// 

        public int WorkflowId { get; set; }

        /// <summary>
        /// 通知标题
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 发起人
        /// </summary>
        public string CreatedByName { get; set; }

        /// <summary>
        /// 结果描述
        /// </summary>
        public string ResultDescribe { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 状态0待审核 1待反馈2已反馈3待确认4已确认
        /// </summary>
        public int Status { get; set; } = 0;

        /// <summary>
        /// 通知类型
        /// </summary>
        public string NotifyType { get; set; }


        /// <summary>
        /// 主任务名称
        /// </summary>
        public string WorkflowName { get; set; }


        /// <summary>
        /// 网格员反馈，汇报信息
        /// </summary>
        public string Info { get; set; }


        /// <summary>
        /// 不通过原因
        /// </summary>
        public string Reason { get; set; }

    }
}
