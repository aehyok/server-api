using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP.Query
{
    /// <summary>
    /// 网格员查询
    /// </summary>
    public class MatrixListQueryModel : PagedListQueryModel
    {
        /// <summary>
        /// 网格员
        /// </summary>
        public int Inspector { get; set; } = 0;

        /// <summary>
        /// 区域id
        /// </summary>
        public int AreaId { get; set; } = 0;

        /// <summary>
        /// 区域网格id
        /// </summary>
        public int MatrixId { get; set; } = 0;

        /// <summary>
        /// 是否添加到了网格，-1 所有， 0 未添加，1 已添加
        /// </summary>
        public int IsUsed { get; set; } = -1;

        /// <summary>
        /// 检测对象类型
        /// </summary>
        public string HouseholdTypes { get; set; } = "";

        /// <summary>
        /// 户码id
        /// </summary>
        public int HouseholdId { get; set; } = 0;
    }
}
