using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Vaccination
{
    /// <summary>
    /// 
    /// </summary>
    public class VaccinationPagePostBody : PagedListQueryModel
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
        /// 查询类型
        /// 1 需求接种户籍，2 已接种，3 接种第一针，4 接种第一针本地 ，5 接种第一针异地，6 接种第二针，7 接种第二针本地，8 接种第二针异地，9 未登记，10 未接种
        /// </summary>
        public string SearchType { get; set; } = "";

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 高龄、低龄、怀孕、哺乳、因病、外出、失联、其他
        /// </summary>
        public string NotReason { get; set; }

        /// <summary>
        /// 未登记
        /// </summary>
        public string notRegister { get; set; } = "";

        /// <summary>
        /// 户码id
        /// </summary>
        public int HouseholdId { get; set; }
    }
}
