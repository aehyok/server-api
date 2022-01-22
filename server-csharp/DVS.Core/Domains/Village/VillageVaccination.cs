using DVS.Common.Models;
using DVS.Models.Dtos.Village;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.Village
{
    /// <summary>
    /// 人口疫苗接种表
    /// </summary>
    public class VillageVaccination : DvsEntityBase
    {
        /// <summary>
        /// 人口Id
        /// </summary>
        public int PopulationId { get; set; } = 0;

        /// <summary>
        /// 接种日期
        /// </summary>
        public DateTime VaccinationDatetime { get; set; }

        /// <summary>
        /// 是否接种 1已 0 未
        /// </summary>
        public int IsVaccination { get; set; } = 0;
        /// <summary>
        /// 接种针剂,1第一针，2第二针
        /// </summary>
        public int NumberStitch { get; set; } = 0;

        /// <summary>
        /// 接种地点，1本地，2异地
        /// </summary>
        public int AddressType { get; set; } = 0;

        /// <summary>
        /// 接种地点
        /// </summary>
        public string Address { get; set; } = "";

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";

        /// <summary>
        /// 未接种原因
        /// </summary>
        public string NotReason { get; set; } = "";

        /// <summary>
        /// 年度
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 接种地
        /// </summary>
        public virtual PopulationAddressDto AddressInfo { get; set; }
    }
}
