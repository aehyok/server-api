using DVS.Model.Dtos.Village;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Vaccination
{
    /// <summary>
    /// 人口疫苗接种表
    /// </summary>
    public class VaccinationDto
    {
        /// <summary>
        /// 疫苗接种登记记录唯一id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 行政代码Id
        /// </summary>
        public int AreaId { get; set; } = 0;

        /// <summary>
        /// 人口Id
        /// </summary>
        public int PopulationId { get; set; } = 0;

        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 户码id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 与户主关系
        /// </summary>
        public string Relationship { get; set; } = "";

        /// <summary>
        /// 门牌名
        /// </summary>
        public string HouseName { get; set; } = "";
        /// <summary>
        /// 门牌号
        /// </summary>
        public string HouseNumber { get; set; } = "";

        /// <summary>
        /// 接种日期
        /// </summary>
        public DateTime VaccinationDatetime { get; set; }

        /// <summary>
        /// 是否接种 1已 0 未
        /// </summary>
        public int IsVaccination { get; set; } = 0;
        /// <summary>
        /// 接种针剂,1第一针，2第二针
        /// </summary>
        public int NumberStitch { get; set; } = 0;

        /// <summary>
        /// 接种地点，1本地，2异地
        /// </summary>
        public int AddressType { get; set; } = 0;

        /// <summary>
        /// 接种地点
        /// </summary>
        public string Address { get; set; } = "";

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";

        /// <summary>
        /// 未接种原因
        /// </summary>
        public string NotReason { get; set; } = "";

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 创建人id
        /// </summary>
        public int CreatedBy { get; set; } = 0;

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 修改人id
        /// </summary>
        public int UpdatedBy { get; set; } = 0;

        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDeleted { get; set; } = 0;

        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreatedByName { get; set; }

        /// <summary>
        /// 修改人姓名
        /// </summary>
        public string UpdatedByName { get; set; }

        /// <summary>
        /// 年度
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 所属户码表
        /// </summary>
        public List<HouseholderAndHouseNumberDto> HouseholdList { get; set; }

        /// <summary>
        /// 是否是户主 1是，0不是
        /// </summary>
        public int IsHouseholder { get; set; } = 0;

        /// <summary>
        /// 头像图片路径
        /// </summary>
        public string HeadImageUrl { get; set; } = "";

        /// <summary>
        /// 标签名称列表 添加或编辑时不需要传入
        /// </summary>
        public IEnumerable<VillageTagDto> TagNames { get; set; } = new List<VillageTagDto>();

        /// <summary>
        /// 接种地
        /// </summary>
        public PopulationAddressDto AddressInfo { get; set; }
    }
}
