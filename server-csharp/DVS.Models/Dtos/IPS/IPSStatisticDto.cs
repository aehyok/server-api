using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;

namespace DVS.Models.Dtos.IPS
{
    /// <summary>
    /// 信发数据统计
    /// </summary>
    public class IPSStatisticDto
    {
        /// <summary>
        /// 审核通过
        /// </summary>
        public int Passed { get; set; }

        /// <summary>
        /// 提交审核
        /// </summary>
        public int Passing { get; set; }

        /// <summary>
        /// 审核不通过
        /// </summary>
        public int Deny { get; set; }


    }
}
