using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Query
{
   public class YearPageListBody: PagedListQueryModel
    {


        /// <summary>
        /// 户码Id 
        /// </summary>
        public int HouseholdId { get; set; }


        /// <summary>
        /// 年度
        /// </summary>
        public int Year { get; set; }
    }
}
