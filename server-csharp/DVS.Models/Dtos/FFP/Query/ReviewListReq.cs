using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP.Query
{
    /// <summary>
    /// 评议列表请求参数
    /// </summary>
    public class ReviewListReq : PagedListQueryModel
    {
        /// <summary>
        /// 区域id
        /// </summary>
        public int AreaId { get; set; } = 0;

        /// <summary>
        /// 状态   2待评议3通过9不通过
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 户属性
        /// </summary>
        public string HouseholdType { get; set; }


    }
}
