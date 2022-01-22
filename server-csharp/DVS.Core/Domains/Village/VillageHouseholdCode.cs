using DVS.Common.Models;
using DVS.Models.Dtos.Village;
using System;
using System.Collections.Generic;

namespace DVS.Core.Domains.Village
{
    /// <summary>
    /// 户码管理
    /// </summary>
    public class VillageHouseholdCode : DvsEntityBase
    {
        /// <summary>
        /// 行政代码Id
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 门牌名Id
        /// </summary>
        public int HouseNameId { get; set; } = 0;

        /// <summary>
        /// 门牌排序
        /// </summary>
        public long HouseNameSequence { get; set; } = 0;

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

        public virtual string AreaName { get; set; }

        // public virtual string TagNames { get; set; }

        public virtual string HouseholdMan { get; set; } = "";

        public virtual string Mobile { get; set; } = "";

        public virtual string HeadImageUrl { get; set; } = "";

        public virtual IEnumerable<VillageTagDto> TagNames { get; set; } = new List<VillageTagDto>();

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
        /// 是否摸排过 0 否，1 是
        /// </summary>
        public int IsMoPai { get; set; } = 0;
    }
}