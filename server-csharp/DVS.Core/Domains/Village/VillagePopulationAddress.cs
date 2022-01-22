using System; 
using System.Text;
using System.Collections.Generic; 
using System.Data;
using DVS.Common.Models;
using DVS.Models.Enum;

namespace DVS.Core.Domains.Village
{
    /// <summary>
    /// 人口信息地址表
    /// </summary>
    public class VillagePopulationAddress : DvsEntityBase
    { 
        /// <summary>
        /// 人口Id
        /// </summary>
        public int PopulationId { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 区
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// 省市区对应行政编码
        /// </summary>
        public string MapCode { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 1 户籍地，2 居住地，3籍贯
        /// </summary>
        public PopulationAddressTypeEnum Type { get; set; }

    }

    
}

