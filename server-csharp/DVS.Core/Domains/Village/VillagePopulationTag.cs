using System; 
using System.Text;
using System.Collections.Generic; 
using System.Data;
using DVS.Common.Models;

namespace DVS.Core.Domains.Village
{
    /// <summary>
    /// 人口信息标签
    /// </summary>
    public class VillagePopulationTag : DvsEntityBase
    {
        /// <summary>
        /// 人口Id
        /// </summary>
        public int PopulationId { get; set; }

        /// <summary>
        /// 标签Id
        /// </summary>
        public int TagId { get; set; }


    }
}

