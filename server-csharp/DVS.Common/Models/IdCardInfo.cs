using System;
using System.Collections.Generic;
using System.Text;
using DVS.Models.Enum;

namespace DVS.Common.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class IdCardInfo
    {
        /// <summary>
        /// 性别 1男2女
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime Birthday { get; set; }
    }
}
