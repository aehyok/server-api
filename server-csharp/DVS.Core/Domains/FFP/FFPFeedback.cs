using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.FFP
{
    /// <summary>
    /// 反馈表
    /// </summary>
    public class FFPFeedback : DvsEntityBase
    {
        /// <summary>
        /// 工作流ID
        /// </summary>
        public int WorkflowId { get; set; } = 0;

        /// <summary>
        /// 名称，编号自动生成
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 反馈类型 1摸排确认2待评议3公示4待上报乡镇9结束
        /// </summary>
        public int FlowStatus { get; set; } = 0;

        /// <summary>
        /// 通知类型
        /// </summary>
        public string NotifyType { get; set; } = "";

        /// <summary>
        /// 反馈结果 通过、不通过
        /// </summary>
        public string Result { get; set; } = "";

        /// <summary>
        /// 结果描述
        /// </summary>
        public string ResultDescribe { get; set; } = "";

        /// <summary>
        /// 原因
        /// </summary>
        public string Reason { get; set; } = "";

        /// <summary>
        /// 状态0 待审核 1待反馈2已反馈3待确认4已确认
        /// </summary>
        public int Status { get; set; } = 0;

        /// <summary>
        /// 汇报信息
        /// </summary>
        public string Info { get; set; } = "";



        /// <summary>
        /// 图片 多个用逗号分割
        /// </summary>
        public string Images { get; set; } = "";

    }
}
