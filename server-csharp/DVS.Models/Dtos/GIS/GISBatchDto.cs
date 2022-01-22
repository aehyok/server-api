using System;
using System.Collections.Generic;
using System.Text;
using DVS.Models.Dtos.Common;

namespace DVS.Models.Dtos.GIS
{
    /// <summary>
    /// 批量更新
    /// </summary>
    public class GISBatchDto
    {
        /// <summary>
        /// 设施类型id
        /// </summary>
        public int TypeId { get; set; } = 0;

        /// <summary>
        /// 面积单位
        /// </summary>
        public string Unit { get; set; } = "";

        /// <summary>
        /// 大棚所属人/企业
        /// </summary>
        public int ObjectId { get; set; } = 0;

        /// <summary>
        /// 地块所属
        /// </summary>
        public int HouseholdId { get; set; } = 0;
        
        /// <summary>
        /// 创建人id
        /// </summary>
        public int CreatedBy { get; set; } = 0;

        /// <summary>
        /// 修改人id
        /// </summary>
        public int UpdatedBy { get; set; } = 0;

        /// <summary>
        /// 批量id
        /// </summary>
        public List<int> Ids { get; set; } = new List<int>();
    }
}
