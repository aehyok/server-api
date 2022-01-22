using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Query
{
    public class SetFromHouseholdBody
    {    /// <summary>
         /// 0 移除户码，1移除人口，2加入
         /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// 户籍人口Id
        /// </summary>
        public int PopulationId { get; set; }


        /// <summary>
        /// 户码Id,在户码移除的时候，这个参数必传
        /// </summary>
        public int HouseholdId { get; set; } = 0;


        /// <summary>
        /// 移除原因
        /// </summary>
        public string DeleteReason { get; set; }
    }
}
