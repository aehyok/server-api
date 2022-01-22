using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.FFP
{
    /// <summary>
    /// 防返贫户码拓展表
    /// </summary>
    public class FFPHouseholdCode : DvsEntityBase
    {
        
        /// <summary>
        /// VillageHouseholdCode 户码Id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Mobile { get; set; } = "";

        /// <summary>
        /// 联系方式简式
        /// </summary>
        public string MobileShort { get; set; } = "";

        /// <summary>
        /// 监测对象，被监测的类型
        /// </summary>
        public string HouseholdType { get; set; } = "";

        /// <summary>
        /// 脱贫户 1是 0否
        /// </summary>
        public int IsWithoutPoverty { get; set; } = 0;

        /// <summary>
        /// 是否在安置区 1是 0否
        /// </summary>
        public int IsInPlaceArea { get; set; } = 0;

        /// <summary>
        /// 状态 1正常，0禁用
        /// </summary>
        public int Status { get; set; } = 0;

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";

    }
}
