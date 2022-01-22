
using System;

namespace DVS.Models.Dtos.Village.Query
{
    /// <summary>
    /// 家庭手来源管理
    /// </summary>
    public class SaveIncomeBody
    {


        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 行政代码Id
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 农产品收入
        /// </summary>
        public double Product { get; set; }

        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 房屋出租收入
        /// </summary>
        public double HouseRental { get; set; }

        /// <summary>
        /// 集体分红收入
        /// </summary>
        public double CollectiveDividend { get; set; }

        /// <summary>
        /// 经销收入
        /// </summary>
        public double Distribution { get; set; }

        /// <summary>
        /// 土地流转收入
        /// </summary>
        public double LandCirculation { get; set; }

        /// <summary>
        /// 务工收入
        /// </summary>
        public double WorkIncome { get; set; }

        /// <summary>
        /// 政府补贴
        /// </summary>
        public double GovSubsidy { get; set; }

        /// <summary>
        /// 其他收入
        /// </summary>
        public double Other { get; set; }

        /// <summary>
        /// 年度
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
}

