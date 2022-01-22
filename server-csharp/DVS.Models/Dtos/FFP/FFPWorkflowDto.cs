using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{
    /// <summary>
    /// 工作流表
    /// </summary>
    public class FFPWorkflowDto 
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// 网格id
        /// </summary>
        public int MatrixId { get; set; } = 0;

        /// <summary>
        /// 户码id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 摸排记录ID
        /// </summary>
        public int MoPaiId { get; set; }

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
        /// 描述信息
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 流程状态 1摸排确认 2待评议 3公示 4待上报乡镇 9结束
        /// </summary>
        public int FlowStatus { get; set; }

        /// <summary>
        /// 创建人id
        /// </summary>
        public int CreatedBy { get; set; } = 0;

        /// <summary>
        /// 修改人id
        /// </summary>
        public int UpdatedBy { get; set; } = 0;

        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDeleted { get; set; } = 0;

        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreatedByName { get; set; }

        /// <summary>
        /// 修改人姓名
        /// </summary>
        public string UpdatedByName { get; set; }
    }
}
