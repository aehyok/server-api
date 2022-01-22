using DVS.Common.Models;
using Lychee.EntityFramework;
using System;

namespace DVS.Core.Domains.Village
{
    /// <summary>
    /// 门牌名表
    /// </summary>
    public class VillageHouseName : DvsEntityBase
    {

        /// <summary>
        /// 行政代码Id
        /// </summary>
        public int AreaId { get; set; } = 0;
        /// <summary>
        /// 门牌名
        /// </summary>
        public string HouseName { get; set; } = "";

        /// <summary>
        /// 排序
        /// </summary>
        public int Sequence { get; set; } = 0;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";

    }
}
