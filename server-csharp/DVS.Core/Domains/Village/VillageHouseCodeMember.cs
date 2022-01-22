using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.Village
{
    /// <summary>
    /// 户码成员表
    /// </summary>
    public class VillageHouseCodeMember : DvsEntityBase
    {

        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; } = 0;

        /// <summary>
        /// 人口Id
        /// </summary>
        public int PopulationId { get; set; } = 0;
        /// <summary>
        /// 是否是户主 1 是 0 否
        /// </summary>
        public int IsHouseholder { get; set; } = 0;

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";

    }
}
