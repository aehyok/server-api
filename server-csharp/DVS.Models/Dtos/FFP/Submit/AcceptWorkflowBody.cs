using DVS.Models.Dtos.Village;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP.Submit
{
    /// <summary>
    /// 
    /// </summary>
    public class AcceptWorkflowBody
    {
        /// <summary>
        /// workflow Id
        /// </summary>
        public int Id { get; set; } = 0;


        /// <summary>
        /// 反馈Id
        /// </summary>
        public int FeedbackId { get; set; } = 0;


        /// <summary>
        /// 不通过原因
        /// </summary>
        public string Reason { get; set; } = "";


        /// <summary>
        /// 年人均收入
        /// </summary>
        public double YearIncome { get; set; } = 0;

        /// <summary>
        /// 返贫风险字典Code  用英文逗号分开
        /// </summary>
        public string PovertyRisk { get; set; } = "";

        /// <summary>
        /// 致贫原因
        /// </summary>
        public string PovertyReason { get; set; } = "";

        /// <summary>
        /// 户码ID
        /// </summary>
        public int HouseholdId { get; set; } = 0;

        /// <summary>
        /// 监测对象，被监测的类型
        /// </summary>
        public string HouseholdType { get; set; } = "";

        /// <summary>
        /// 家庭地址
        /// </summary>

        public PopulationAddressDto FamilyAddressInfo { get; set; }

        /// <summary>
        /// 流程状态 1摸排确认2待评议3公示4待上报乡镇9结束
        /// </summary>
        public int FlowStatus { get; set; } = 0;


    }
}
