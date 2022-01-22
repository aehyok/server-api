using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP.Query
{
    /// <summary>
    /// 评议公示管理列表请求参数
    /// </summary>
    public class PublicityManageListReq : PagedListQueryModel
    {
        /// <summary>
        /// 区域id
        /// </summary>
        public int AreaId { get; set; } = 0;
    }
}
