using DVS.Common.Models;

namespace DVS.Core.Domains.Village
{
    /// <summary>
    /// 户码标签
    /// </summary>
    public class VillageHouseholdCodeTag : DvsEntityBase
    {
        /// <summary>
        /// 户码编号
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 标签编号
        /// </summary>
        public int TagId { get; set; }
    }
}