using DVS.Models.Dtos.Village;
using DVS.Models.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Model.Dtos.Village
{
    /// <summary>
    /// 户主门牌信息
    /// </summary>
    public class HouseholderAndHouseNumberDto
    {
        /// <summary>
        /// 人口Id
        /// </summary>
        public int PopulationId { get; set; } = 0;
        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; } = "";
        /// <summary>
        /// 性别 1男，2女
        /// </summary>
        public PopulationGender Sex { get; set; } = PopulationGender.男;
        /// <summary>
        /// 是否是户主1 是 0 否
        /// </summary>
        public int IsHouseholder { get; set; }
        /// <summary>
        /// 与户主关系
        /// </summary>
        public string Relationship { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 户主头像
        /// </summary>
        public string HeadImageUrl { get; set; } = "";
        /// <summary>
        /// 人口属性
        /// </summary>
        public IEnumerable<VillageTagDto> PopulationTagNames { get; set; }
        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; }
        /// <summary>
        /// 户码所属区域
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 门牌名
        /// </summary>
        public string HouseName { get; set; }
        /// <summary>
        /// 门牌号
        /// </summary>
        public string HouseNumber { get; set; }
        /// <summary>
        /// 住宅图片
        /// </summary>
        public string ImageUrls { get; set; }
        /// <summary>
        /// 户码标签
        /// </summary>
        public IEnumerable<VillageTagDto> HouseholdTagNames { get; set; }
        /// <summary>
        /// 区域Id
        /// </summary>
        public int AreaId { get; set; }
        /// <summary>
        /// 社区/村名称
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdCard { get; set; } = "";

        /// <summary>
        /// 删除标记
        /// </summary>
        public int IsDeleted { get; set; }


        /// <summary>
        /// 门牌名id
        /// </summary>
        public int HouseNameId { get; set; }
    }
}
