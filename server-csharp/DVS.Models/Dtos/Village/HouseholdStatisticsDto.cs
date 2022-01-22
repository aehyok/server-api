using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village
{
    /// <summary>
    /// 计算户数和人口数
    /// </summary>
   public class HouseholdStatisticsDto
    {
        /// <summary>
        /// 行政区域Id
        /// </summary>
        public int AreaId { get; set; } = 0;

        /// <summary>
        /// 户数
        /// </summary>
        public int HouseholdCount { get; set; } = 0;

        /// <summary>
        /// 人口数量
        /// </summary>
        public int PopulationCount { get; set; } = 0;
    }
}
