using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Cons
{
    public class PhotoAnywhereTypeMonthDto
    {
        /// <summary>
        /// 月份
        /// </summary>
        public string Month { get; set; }
        
        /// <summary>
        /// 数量（总数）
        /// </summary>
        public decimal Cnt1 { get; set; }

        /// <summary>
        /// 数量（已办理）
        /// </summary>
        public decimal Cnt2 { get; set; }
    }
}
