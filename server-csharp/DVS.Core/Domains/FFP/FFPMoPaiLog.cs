using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.FFP
{
    /// <summary>
    /// 摸排记录表
    /// </summary>
    public class FFPMoPaiLog : DvsEntityBase
    {
        /// <summary>
        /// 户码id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 网格id
        /// </summary>
        public int MatrixId { get; set; } = 0;

        /// <summary>
        /// 是否存在返贫风险 1是2否
        /// </summary>
        public int ExistRisk { get; set; }

        /// <summary>
        /// 摸排日期
        /// </summary>
        public DateTime MoPaiDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 图片 多个用逗号分割
        /// </summary>
        public string Images { get; set; }

        /// <summary>
        /// 语音链接地址  多个用逗号分割
        /// </summary>
        public string VoiceUrl { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Describe { get; set; }


    }
}
