using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{
    /// <summary>
    /// 会议评议列表
    /// </summary>
    public class ReviewListDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }


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
        /// 户主的联系电话
        /// </summary>
        public string Mobile { get; set; } = "";

        /// <summary>
        /// 人数
        /// </summary>
        public long PeopleCount { get; set; } = 0;

        /// <summary>
        /// 户贫困标签(户属性)
        /// </summary>
        public string HouseholdType { get; set; } = "";

        /// <summary>
        /// 核查日期
        /// </summary>
        public DateTime CheckDate { get; set; }

        /// <summary>
        /// 评议类型
        /// </summary>
        public string ReviewType { get; set; }

        /// <summary>
        /// 信息采集
        /// </summary>
        public string[] InfoCollect { get; set; } = { "申请书", "信息采集" };

    }
}
