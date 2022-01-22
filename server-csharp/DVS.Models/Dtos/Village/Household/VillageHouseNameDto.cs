using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Household
{
    /// <summary>
    /// 
    /// </summary>
    public class VillageHouseNameDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; } = 0;

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
