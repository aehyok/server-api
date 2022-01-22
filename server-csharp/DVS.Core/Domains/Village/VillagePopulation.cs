using DVS.Common.Models;
using DVS.Models.Dtos.Village;
using DVS.Models.Enum;
using System;
using System.Collections.Generic;

namespace DVS.Core.Domains.Village
{
    /// <summary>
    /// 户籍人口管理
    /// </summary>
    public class VillagePopulation : DvsEntityBase
    {
        /// <summary>
        /// 行政代码Id
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 户码Id
        /// </summary>
        // public virtual int HouseholdId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 性别 1男，2女
        /// </summary>
        public PopulationGender Sex { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        public string Nation { get; set; } = "";

        /// <summary>
        /// 是否是户主 1是，0不是
        /// </summary>
        // public virtual int IsHouseholder { get; set; } = 0;

        /// <summary>
        /// 出生年月日 1990-04-02
        /// </summary>
        public DateTime Birthday { get; set; }

        /// <summary>
        /// 与户主关系
        /// </summary>
        public string Relationship { get; set; } = "";

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdCard { get; set; } = "";

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Mobile { get; set; } = "";

        /// <summary>
        /// 政治面貌
        /// </summary>
        public string Political { get; set; } = "";

        /// <summary>
        /// 学历
        /// </summary>
        public string Education { get; set; } = "";

        /// <summary>
        /// 婚姻状态
        /// </summary>
        public string Marital { get; set; } = "";

        /// <summary>
        /// 宗教
        /// </summary>
        public string Religion { get; set; } = "";

        /// <summary>
        /// 收入来源
        /// </summary>
        public string Income { get; set; } = "";

        /// <summary>
        /// 头像Id
        /// </summary>
        public string HeadImageId { get; set; } = "";

        /// <summary>
        /// 头像图片路径
        /// </summary>
        public string HeadImageUrl { get; set; } = "";

        /// <summary>
        /// 标签数组
        /// </summary>
        public string Tags { get; set; } = "";

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";

        /// <summary>
        /// 1 启用 0停用
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 移除原因
        /// </summary>
        public string DeleteReason { get; set; } = "";

        /// <summary>
        /// 标签名称列表
        /// </summary>
        // public string tagNames { get; set; } = "";

        /// <summary>
        /// 人口类型
        /// </summary>
        public PopulationType PopulationType { get; set; } = PopulationType.户籍人口;

        /// <summary>
        /// 标签名称列表
        /// </summary>
        public IEnumerable<VillageTagDto> tagNames { get; set; } = new List<VillageTagDto>();

        /// <summary>
        /// 同步到数据大屏后返回的唯一id'
        /// </summary>
        public string SyncId { get; set; } = "";

        /// <summary>
        /// 同步操作后返回的description
        /// </summary>
        public string SyncRes { get; set; } = "";

        /// <summary>
        /// 是否已同步, 0 否 1 是
        /// </summary>
        public int IsSync { get; set; } = 0;

        /// <summary>
        /// 同步操作时间
        /// </summary>
        public DateTime SyncDate { get; set; }

        /// <summary>
        /// 联系方式简式
        /// </summary>
        public string MobileShort { get; set; }
    }

    /// <summary>
    /// 人口类型
    /// </summary>
    public enum PopulationType
    {
        户籍人口 = 1,
        流动人口,
        外籍人口
    }


}