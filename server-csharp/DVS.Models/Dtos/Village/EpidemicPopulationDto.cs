using DVS.Model.Dtos.Village;
using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village
{
   public class EpidemicPopulationDto
    {

        /// <summary>
        /// 返乡记录Id
        /// </summary>
        public int Id { get; set; } = 0;


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
        public int Sex { get; set; } = 1;
        /// <summary>
        /// 行政代码Id
        /// </summary>
        public int AreaId { get; set; } = 0;

        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; } = 0;

       

        /// <summary>
        /// 来源地行政代码
        /// </summary>
        public string SourceOrgCodes { get; set; } = "";

        /// <summary>
        /// 来源地地详细地址
        /// </summary>
        public string SourceAddress { get; set; } = "";

        /// <summary>
        /// 反乡日期
        /// </summary>
        public DateTime? ReversalDate { get; set; }

        /// <summary>
        /// 登记日期
        /// </summary>
        public DateTime? RecorDate { get; set; }

        /// <summary>
        /// 体温
        /// </summary>
        public string Temperature { get; set; } = "";

        /// <summary>
        /// 异常情况，健康情况
        /// </summary>
        public string Health { get; set; } = "";

        /// <summary>
        /// 有无发热、乏力、咳嗽等症状 1 有 0 无
        /// </summary>
        public int IsFever { get; set; } = 0;

        /// <summary>
        /// 疫区接触情况 1 有 0 无
        /// </summary>
        public int IsInAreas { get; set; } = 0;


        /// <summary>
        /// 
        /// </summary>
        public BasicDictionaryDto HealthDto { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public BasicDictionaryDto IsInAreasDto { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";

        /// <summary>
        /// 与户主关系
        /// </summary>
        public string Relationship { get; set; } = "";

        /// <summary>
        /// 头像图片路径
        /// </summary>
        public string HeadImageUrl { get; set; } = "";

        /// <summary>
        /// 标签数组
        /// </summary>
        public string Tags { get; set; } = "";

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Mobile { get; set; } = "";

        /// <summary>
        /// 民族
        /// </summary>
        public string Nation { get; set; } = "";

        /// <summary>
        /// 标签列表
        /// </summary>
        public IEnumerable<VillageTagDto> TagNames { get; set; } = new List<VillageTagDto>();

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
        /// 
        /// </summary>
        public int Year { get; set; } = 0;

        /// <summary>
        /// 返乡来源地
        /// </summary>
        public PopulationAddressDto SourceAddressInfo { get; set; }

        /// <summary>
        /// 所属户码表
        /// </summary>
        public List<HouseholderAndHouseNumberDto> HouseholdList { get; set; } = new List<HouseholderAndHouseNumberDto>();
    }
}
