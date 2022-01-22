using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{

    /// <summary>
    /// 处理任务列表
    /// </summary>
    public class FFPFeedbackLisDto
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
        /// 创建人
        /// </summary>
        public string CreatedByName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 状态 1待反馈2已反馈3已确认
        /// </summary>
        public int Status { get; set; } = 0;

        /// <summary>
        /// 通知类型
        /// </summary>
        public string NotifyType { get; set; }

    }
}
