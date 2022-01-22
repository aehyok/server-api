using DVS.Models.Dtos.Village;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{
    /// <summary>
    /// 评议记录详情
    /// </summary>
    public class ReviewDetailDto
    {

        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// FFPHouseholdCode 表的ID ,取地址用
        /// </summary>
        public int Hid { get; set; }

        /// <summary>
        /// 社区/村
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 门牌名
        /// </summary>
        public string HouseName { get; set; } = "";

        /// <summary>
        /// 门牌号
        /// </summary>
        public string HouseNumber { get; set; } = "";


        /// <summary>
        /// 户主
        /// </summary>
        public string HouseholdMan { get; set; } = "";

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; } = "";

        /// <summary>
        /// 户主的联系电话
        /// </summary>
        public string Mobile { get; set; } = "";

        /// <summary>
        /// 门牌标签
        /// </summary>
        public string HouseholdTags { get; set; } = "";

        /// <summary>
        /// 人数
        /// </summary>
        public long PeopleCount { get; set; } = 0;

        /// <summary>
        /// 收入
        /// </summary>
        public double YearIncome { get; set; } = 0;

        /// <summary>
        /// 户贫困标签(户属性)
        /// </summary>
        public string HouseholdType { get; set; } = "";

        /// <summary>
        /// 核查日期
        /// </summary>
        public DateTime CheckDate { get; set; }

        /// <summary>
        /// 核查人员
        /// </summary>
        public string CheckPerson { get; set; }

        /// <summary>
        /// 返贫风险
        /// </summary>
        public string PovertyRisk { get; set; }

        /// <summary>
        /// 致贫原因
        /// </summary>
        public string PovertyReason { get; set; }

        /// <summary>
        /// 评议详情
        /// </summary>
        public string ReviewInfo { get; set; }

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
        /// 结果 通过、不通过
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 图片 多个用逗号分割
        /// </summary>
        public string Images { get; set; }

    }
}
