using System;
using System.Collections.Generic;
using System.Text;
using DVS.Models.Dtos.Common;

namespace DVS.Models.Dtos.FFP
{
    /// <summary>
    /// 统计表
    /// </summary>
    public class FFPHouseholdStatisticDto
    {
        /// <summary>
        /// 户码属性
        /// </summary>
        public string HouseholdType { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public long Cnt { get; set; } = 0;


        /// <summary>
        /// 总数
        /// </summary>
        public long Total { get; set; } = 0;

        /// <summary>
        /// 是否摸排过
        /// </summary>
        public int IsMoPai { get; set; }
    }
}
