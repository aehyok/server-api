using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.FFP
{
    /// <summary>
    /// 工作流表
    /// </summary>
    public class FFPWorkflow : DvsEntityBase
    {
        /// <summary>
        /// 户码id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 摸排记录ID
        /// </summary>
        public int MoPaiId { get; set; } = 0;

        /// <summary>
        /// 网格id
        /// </summary>
        public int MatrixId { get; set; } = 0;

        /// <summary>
        /// 户主名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 图片 多个用逗号分割
        /// </summary>
        public string Images { get; set; }

        /// <summary>
        /// 返贫风险  多个用逗号分割
        /// </summary>
        public string PovertyRisk { get; set; }

        /// <summary>
        /// 致贫原因
        /// </summary>
        public string PovertyReason { get; set; }

        /// <summary>
        /// 收入
        /// </summary>
        public double YearIncome { get; set; } = 0;

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 流程状态 1摸排确认 2待评议 3公示 4待上报乡镇 9结束
        /// </summary>
        public int FlowStatus { get; set; } = 0;


        /// <summary>
        /// 投票人数
        /// </summary>
        public int VoteCount { get; set; } = 0;

        /// <summary>
        /// 同意票数
        /// </summary>
        public int Agree { get; set; } = 0;

        /// <summary>
        /// 不同意票数
        /// </summary>
        public int Disagree { get; set; } = 0;


    }
}
