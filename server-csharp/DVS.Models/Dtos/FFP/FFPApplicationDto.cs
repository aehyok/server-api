using DVS.Models.Dtos.Village;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{
    /// <summary>
    /// 申请书信息
    /// </summary>
    public class FFPApplicationDto
    {
        /// <summary>
        /// 工作流的id
        /// </summary>
        public int WorkflowId { get; set; }
        /// <summary>
        /// 户主的名称
        /// </summary>
        public string HouseholderName { get; set; } = string.Empty;
        /// <summary>
        /// 住址
        /// </summary>
        public PopulationAddressDto LivingAddress {get;set;}

        /// <summary>
        /// 是否已脱贫, 1是 0否
        /// </summary>
        public int IsWithoutPorverty { get; set; }
        /// <summary>
        /// 户码的id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 劳动力数量
        /// </summary>
        public int LabourCount { get; set; }
        /// <summary>
        /// 在校生数量
        /// </summary>
        public int StudentCount { get; set; }
        /// <summary>
        /// 慢性病患者数
        /// </summary>
        public int ChronicCount { get; set; }
        /// <summary>
        /// 家庭成员数
        /// </summary>
        public int MemberCount { get; set; }
        /// <summary>
        /// 重症患者数
        /// </summary>
        public int SeriousDiseaseCount { get; set; }
        /// <summary>
        /// 残疾人数
        /// </summary>
        public int DisabledPeopleCount { get; set; }
        /// <summary>
        /// 享受津贴人数
        /// </summary>
        public int AllowanceCount { get; set; }
        /// <summary>
        /// 年收入
        /// </summary>
        public double YearIncome { get; set; }
        /// <summary>
        /// 无能力解决的问题
        /// </summary>
        public string Difficulty { get; set; } = string.Empty;
        /// <summary>
        /// 选择其他，具体困难的描述
        /// </summary>
        public string DifficultyRemark { get; set; } = string.Empty;
    }
}
