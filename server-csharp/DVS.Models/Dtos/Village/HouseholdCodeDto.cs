using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village
{
    /// <summary>
    /// 户码列表专用
    /// </summary>
    public class HouseholdCodeDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        ///// <summary>
        ///// 行政代码Id
        ///// </summary>
        //public int AreaId { get; set; } = 0;

        /// <summary>
        /// 行政区域名称
        /// </summary>
        public string AreaName { get; set; } = "";

        /// <summary>
        /// 门牌名
        /// </summary>
        public string HouseName { get; set; } = "";

        /// <summary>
        /// 门牌号
        /// </summary>
        public string HouseNumber { get; set; } = "";


        /// <summary>
        /// 户主
        /// </summary>
        public string HouseholdMan { get; set; } = "";
        /// <summary>
        /// 户主的联系电话
        /// </summary>
        public string Mobile { get; set; }="";
        /// <summary>
        /// 性别
        /// </summary>
        public long Sex { get; set; } =1;

        /// <summary>
        /// 人口Id
        /// </summary>
        public long PopulationId { get; set; } = 0;

        /// <summary>
        /// 门牌号
        /// </summary>
        public string Relationship { get; set; } = "";

        /// <summary>
        /// 是否户主
        /// </summary>
        public long IsHouseholder { get; set; } = 0;

        /// <summary>
        /// 人数
        /// </summary>
        public long PeopleCount { get; set; } = 0;

        /// <summary>
        /// 标签列表
        /// </summary>
        public IEnumerable<VillageTagDto> TagNames { get; set; } = new List<VillageTagDto>();


        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 是否标绘
        /// </summary>
        public bool IsPloted { get; set; } = false;

        /// <summary>
        /// 标绘id
        /// </summary>
        public int PlotId { get; set; } = 0;

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";
    
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
        /// 最后操作时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public long HouseNameSequence { get; set; } = 0;
        /// <summary>
        /// 头像的相对地址
        /// </summary>
        public string HeadImageUrl { get; set; }
    }
}
