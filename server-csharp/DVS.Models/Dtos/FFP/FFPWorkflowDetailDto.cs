using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{
    /// <summary>
    /// 工作流表
    /// </summary>
    public class FFPWorkflowDetailDto
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
        /// 反馈表Id
        /// </summary>
        public long FeedbackId { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string WorkflowName { get; set; }

        /// <summary>
        /// 图片 多个用逗号分割，附件
        /// </summary>
        public string Images { get; set; }

        /// <summary>
        /// 通知类型
        /// </summary>
        public string NotifyType { get; set; }


        /// <summary>
        /// 描述信息,说明
        /// </summary>
        public string Describe { get; set; } = "";



        /// <summary>
        /// 流程状态 1摸排确认 2待评议 3公示 4待上报乡镇 9结束
        /// </summary>
        public int FlowStatus { get; set; }

        /// <summary>
        /// 创建人姓名，发起人
        /// </summary>
        public string CreatedByName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }


        /// <summary>
        /// 处理结果，通过、不通过
        /// </summary>
        public string Result { get; set; }


        /// <summary>
        /// 通过和不通过的原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 网格员反馈
        /// </summary>
        public string Info { get; set; }


        /// <summary>
        /// 语音 链接地址 多个用,分割
        /// </summary>
        public string VoiceUrl { get; set; }


        /// <summary>
        /// 状态0待审核 1待反馈2已反馈3待确认4已确认
        /// </summary>
        public long Status { get; set; } = 0;

        /// <summary>
        /// 图片文件
        /// </summary>
        public List<SunFileInfoDto> ImageFiles { get; set; } = new List<SunFileInfoDto>();

        /// <summary>
        /// 回复图标文件
        /// </summary>
        public List<SunFileInfoDto> VoiceFiles { get; set; } = new List<SunFileInfoDto>();


    }
}
