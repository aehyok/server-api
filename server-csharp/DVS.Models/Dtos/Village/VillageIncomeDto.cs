using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village
{
    public class VillageIncomeDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; } = 0;
        /// <summary>
        /// 门牌名
        /// </summary>
        public string HouseName { get; set; } = "";
        /// <summary>
        /// 门牌号
        /// </summary>
        public string HouseNumber { get; set; } = "";
        /// <summary>
        /// 年度
        /// </summary>
        public long Year { get; set; } = 0;
        /// <summary>
        /// 户主
        /// </summary>
        public string HouseholdMan { get; set; } = "";
        /// <summary>
        /// 家庭总收入
        /// </summary>
        public double TotalIncome { get; set; } = 0;

        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; } = "";
        /// <summary>
        /// 区域Id
        /// </summary>
        public int AreaId { get; set; } = 0;

        /// <summary>
        /// 门牌排序
        /// </summary>
        public long HouseNameSequence { get; set; } = 0;

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; } = "";

        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; } = 0;

    }
}
