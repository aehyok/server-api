using DVS.Common.Models;
using DVS.Models.Dtos.Village;
using Lychee.EntityFramework;
using System;

namespace DVS.Core.Domains.Village
{
    /// <summary>
    /// 疫情防控管理
    /// </summary>
    public class VillageEpidemic : DvsEntityBase
    {

        /// <summary>
        /// 行政代码Id
        /// </summary>
        public virtual int AreaId { get; set; }

        /// <summary>
        /// 户码Id
        /// </summary>
        public virtual int HouseholdId { get; set; }

        /// <summary>
        /// 人口Id
        /// </summary>
        public int PopulationId { get; set; }

        /// <summary>
        /// 来源地行政代码
        /// </summary>
        public string SourceOrgCodes { get; set; }

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
        public DateTime RecorDate { get; set; }

        /// <summary>
        /// 体温
        /// </summary>
        public string Temperature { get; set; }

        /// <summary>
        /// 异常情况，健康情况
        /// </summary>
        public string Health { get; set; }

        /// <summary>
        /// 有无发热、乏力、咳嗽等症状 1 有 0 无
        /// </summary>
        public int IsFever { get; set; } = 0;

        /// <summary>
        /// 疫区接触情况 1 有 0 无
        /// </summary>
        public int IsInAreas { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 年度
        /// </summary>
        public int Year { get; set; }

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
        /// 返乡来源地
        /// </summary>
        public PopulationAddressDto SourceAddressInfo { get; set; }
    }
}



