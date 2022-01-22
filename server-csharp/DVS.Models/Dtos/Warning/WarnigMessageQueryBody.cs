using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Warning
{
    /// <summary>
    /// 告警信息查询body
    /// </summary>
    public class WarnigMessageQueryBody : PagedListQueryModel
    {
        /// <summary>
        /// 是否已解除，0否，1是
        /// </summary>
        public int IsFinish { get; set; } = 0;
    }
}
