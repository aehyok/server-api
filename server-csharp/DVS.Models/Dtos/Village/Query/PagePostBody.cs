using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Query
{
    public class PagePostBody: PagedListQueryModel
    {
       
        /// <summary>
        /// 行政区域Id
        /// </summary>
        public int AreaId { get; set; }



        /// <summary>
        /// 年份,可选
        /// </summary>
        public int Year { get; set; } = 0;

        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 人口Id 可选
        /// </summary>
        public int PopulationId { get; set; }

        /// <summary>
        /// 批量查询
        /// </summary>
        public string Ids { get; set; }

    }
}
