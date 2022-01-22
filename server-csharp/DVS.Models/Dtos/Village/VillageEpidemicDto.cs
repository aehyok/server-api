using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village
{
   public class VillageEpidemicDto
    {  /// <summary>
       /// 户码Id
       /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 行政区域Id
        /// </summary>
        public int AreaId { get; set; }
        /// <summary>
        /// 行政区域名称
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 门牌名
        /// </summary>
        public string HouseName { get; set; }
        /// <summary>
        /// 门牌号
        /// </summary>
        public string HouseNumber { get; set; }
        /// <summary>
        /// 人数
        /// </summary>
        public long PeopleCount { get; set; }
        /// <summary>
        /// 户主
        /// </summary>
        public string HouseholdMan { get; set; }
        /// <summary>
        /// 性别 1男，2女,
        /// </summary>
        public long Sex { get; set; }
        /// <summary>
        /// 民族,
        /// </summary>
        public string Nation { get; set; }

        /// <summary>
        /// 年度
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Mobile { get; set; }
    }
}
