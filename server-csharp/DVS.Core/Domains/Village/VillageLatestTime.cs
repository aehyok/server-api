using DVS.Common.Models;
using Lychee.EntityFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.Village
{
    /// <summary>
    /// 最新操作时间记录表
    /// </summary>
    public class VillageLatestTime: DvsEntityBase
    {
        /// <summary>
        /// 
        /// </summary>
        public int HouseholdId { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>

        public int TableType { get; set; } = 0;


        /// <summary>
        /// 
        /// </summary>
        public long PeopleCount { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public long Year { get; set; } = 0;

        

    }
}
