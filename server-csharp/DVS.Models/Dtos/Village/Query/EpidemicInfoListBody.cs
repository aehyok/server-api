using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Query
{
    public class EpidemicInfoListBody : PagedListQueryModel
    {
        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 人口Id 可选
        /// </summary>
        public int PopulationId { get; set; }

        /// <summary>
        /// 年度
        /// </summary>
        public int Year { get; set; }


        /// <summary>
        /// 月
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// 查询类型
        /// 1 户籍人口返乡记录，2 14天内异常人员，3 累计异常，4 按年月查询返乡记录
        /// </summary>
        public int SearchType { get; set; } = 0;

        /// <summary>
        /// 区域码id
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 是否正常 0 所有 1 正常 2 异常
        /// </summary>
        public int IsNormal { get; set; }
    }
}
