using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Farmland
{
    /// <summary>
    /// 查询户码的土地列表的统计信息
    /// </summary>
    public class FarmlandAreaSummary
    {
        /// <summary>
        /// 面积
        /// </summary>
        public decimal Area { get; set; }
        /// <summary>
        /// 区域id
        /// </summary>
        public int AreaId { get; set; }
        /// <summary>
        /// 类别id
        /// </summary>
        public int TypeId { get; set; }
        /// <summary>
        /// 类别名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 类别名称编码
        /// </summary>
        public string TypeNameCode { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 户码id
        /// </summary>
        public int HouseholdId { get; set; }
    }
}
