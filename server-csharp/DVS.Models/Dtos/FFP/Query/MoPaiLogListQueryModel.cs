using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP.Query
{
    /// <summary>
    /// 摸排记录查询
    /// </summary>
    public class MoPaiLogListQueryModel : PagedListQueryModel
    {
        /// <summary>
        /// 网格员
        /// </summary>
        public int Inspector { get; set; } = 0;

        /// <summary>
        /// 摸排记录状态，1摸排确认，2待评议，3公示，4待上报乡镇，9结束
        /// </summary>
        public int FlowStatus { get; set; } = 1;

        /// <summary>
        /// 户属性
        /// </summary>
        public string HouseholdType { get; set; } = "";
    }
}
