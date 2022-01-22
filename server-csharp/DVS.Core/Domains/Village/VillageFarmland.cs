using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.Village
{
    /// <summary>
    /// 地块信息
    /// </summary>
    public class VillageFarmland:DvsEntityBase
    {
        /// <summary>
        /// 区域ID
        /// </summary>
        public int AreaId { get; set; }
        /// <summary>
        /// 户码的Id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 园区ID
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 土地类型的编码
        /// </summary>
        public int TypeId { get; set; }
        /// <summary>
        /// 土地面积
        /// </summary>
        public decimal Area { get; set; }
        /// <summary>
        /// 面积单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 地块地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 地块名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 地块所属类型 1 区域 ,2 园区
        /// </summary>
        public int Category { get; set; } = 1;

        /// <summary>
        /// 地块用途 1 普通用地 ,2 规划用地
        /// </summary>
        public int UseFor { get; set; } = 1;

    }
}
