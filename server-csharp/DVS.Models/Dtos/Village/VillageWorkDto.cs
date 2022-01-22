using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village
{
    public class VillageWorkDto
    {


        /// <summary>
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
        /// 最后操作时间
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }


        /// <summary>
        /// 户码用这个排序
        /// </summary>
        public long HouseNameSequence { get; set; }



    }
}
