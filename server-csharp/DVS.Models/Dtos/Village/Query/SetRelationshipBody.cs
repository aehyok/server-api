using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Query
{
    public class SetRelationshipBody
    {
        /// <summary>
        /// 与户主关系
        /// </summary>
        // public string Relationship { get; set; } // 与户主关系,

        /// <summary>
        /// 户籍人口Id
        /// </summary>
        public int PopulationId { get; set; }

        /// <summary>
        /// 户码id
        /// </summary>
        public int HouseholdId { get; set; } = 0;
    }
}
