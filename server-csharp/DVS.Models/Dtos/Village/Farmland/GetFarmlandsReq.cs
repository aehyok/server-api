using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Farmland
{
    public class GetFarmlandsReq
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 10;
        public int TypeId { get; set; }

        /// <summary>
        /// 区域的id
        /// </summary>
        public int AreaId { get; set; }
        public string Keyword { get; set; }
        /// <summary>
        /// 户码的ID，如果没指定，那查全部
        /// </summary>
        public int HouseholdId { get; set; }
        public List<OrderBy> OrderBy { get; set; }
    }
}
