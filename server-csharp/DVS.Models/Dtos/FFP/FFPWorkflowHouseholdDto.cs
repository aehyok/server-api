using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{
    /// <summary>
    /// 工作流 待评议详情，待公示详情
    /// </summary>
    public class FFPWorkflowHouseholdDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 户码id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 摸排记录ID
        /// </summary>
        public int MoPaiId { get; set; }

        /// <summary>
        /// 网格id
        /// </summary>
        public int MatrixId { get; set; } = 0;

        /// <summary>
        /// 户主名称
        /// </summary>
        public string WorkflowName { get; set; }

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
        /// 收入 近一年人均收入（元）
        /// </summary>
        public double YearIncome { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 流程状态 1摸排确认 2待评议 3公示 4待上报乡镇 9结束
        /// </summary>
        public int FlowStatus { get; set; }


        /// <summary>
        /// 投票人数
        /// </summary>
        public int VoteCount { get; set; }

        /// <summary>
        /// 同意票数
        /// </summary>
        public int Agree { get; set; }

        /// <summary>
        /// 不同意票数
        /// </summary>
        public int Disagree { get; set; }


        /// <summary>
        /// 评议结果，通过、不通过
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 原因、评议详情
        /// </summary>
        public string Reason { get; set; }


        /// <summary>
        /// 户主名称
        /// </summary>
        public string HouseholdName { get; set; }


        /// <summary>
        /// 户主人数
        /// </summary>
        public long HouseholdMemberCount { get; set; }

        /// <summary>
        /// 户属性
        /// </summary>
        public string HouseholdType { get; set; }

        /// <summary>
        /// 户联系方式
        /// </summary>
        public string HouseholdMobile { get; set; }


        /// <summary>
        /// 户码
        /// </summary>
        public string HouseholdNumber { get; set; }

        /// <summary>
        /// 户主头像路径
        /// </summary>
        public string HeadImageUrl { get; set; }

        /// <summary>
        /// 户主头像ID
        /// </summary>
        public string HeadImageId { get; set; }


        /// <summary>
        /// 图片文件
        /// </summary>
        public List<SunFileInfoDto> ImageFiles { get; set; } = new List<SunFileInfoDto>();

    }
}
