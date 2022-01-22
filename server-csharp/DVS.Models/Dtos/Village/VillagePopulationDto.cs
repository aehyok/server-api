using DVS.Models.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village
{
    public class VillagePopulationDto
    {
        public int Id { get; set; }

        /// <summary>
        /// 行政代码Id
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 户籍地详细地址
        /// </summary>
        public string RegisterAddress { get; set; } 

        /// <summary>
        /// 现居住地详细地址
        /// </summary>
        public string LiveAddress { get; set; }

        /// <summary>
        /// 1 启用 0停用
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 是否是户主，1是，0否
        /// </summary>
        public int IsHouseholder { get; set; }

        /// <summary>
        /// 门牌名
        /// </summary>
        public string HouseName { get; set; } = "";
        /// <summary>
        /// 门牌号
        /// </summary>
        public string HouseNumber { get; set; } = "";

        /// <summary>
        /// 头像图片路径
        /// </summary>
        public string HeadImageUrl { get; set; } = "";

        /// <summary>
        /// 与户主关系
        /// </summary>
        public string Relationship { get; set; } = "";

        /// <summary>
        /// 民族
        /// </summary>
        public string Nation { get; set; } = "";

        /// <summary>
        /// 性别 1男，2女
        /// </summary>
        public PopulationGender Sex { get; set; }

        /// <summary>
        /// 用户头像id
        /// </summary>
        public int PortraitFileId { get; set; }
    }
}
