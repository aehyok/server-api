using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village
{
    public class VillageHouseholdCodeDto
    {
       
       



        /// <summary>
        /// 行政代码Id
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 门牌名
        /// </summary>
        public string HouseName { get; set; }

        /// <summary>
        /// 门牌号
        /// </summary>
        public string HouseNumber { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 户人数
        /// </summary>
        public int PeopleCount { get; set; }

        /// <summary>
        /// 住宅图片
        /// </summary>
        public string ImageIds { get; set; }

        /// <summary>
        /// 住宅图片
        /// </summary>
        public string ImageUrls { get; set; }

        /// <summary>
        /// 标签数组
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// 状态 1正常，0禁用
        /// </summary>
        public int Status { get; set; }


        public string AreaName { get; set; }

        public string TagNames { get; set; }

        public string HouseholdMan { get; set; }

    }
}
