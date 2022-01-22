using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Statistics
{
    /// <summary>
    /// 土地信息汇总
    /// </summary>
    public class StatisticsFarmlandDto
    {
        /// <summary>
        /// 户码的Id
        /// </summary>
        public int HouseholdId { get; set; } = 0;

        /// <summary>
        /// 土地面积
        /// </summary>
        public decimal Area { get; set; } = 0;

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; } = "";

        /// <summary>
        /// 类型Id
        /// </summary>
        public int TypeId { get; set; } = 0;
    }
}
