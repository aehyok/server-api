using System;
using System.Collections.Generic;
using System.Text;
using DVS.Models.Dtos.Common;

namespace DVS.Models.Dtos.GIS
{
    /// <summary>
    /// 公共设施
    /// </summary>
    public class GISCollectivePropertyDto
    {
        /// <summary>
        /// 唯一码id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 标绘点名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 区域id
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 设施类型id
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// 设施类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 设施类型对象
        /// </summary>
        public BasicDictionaryDto TypeDto { get; set; }

        /// <summary>
		/// 打点类型 1 区域 ,2 企业园区 
		/// </summary>
		public int Category { get; set; }

        /// <summary>
        /// 设施位置
        /// </summary>
        public string Address { get; set; }

        /// <summary>
		/// 媒资类型 1 图片 2 视频
		/// </summary>
		public int MediaType { get; set; }

        /// <summary>
        /// 媒资id
        /// </summary>
        public int MediaId { get; set; }

        /// <summary>
        /// 媒资完整信息
        /// </summary>
        public List<SunFileInfoDto> MediaFiles { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

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
        /// 是否标绘
        /// </summary>
        public bool IsPloted { get; set; } = false;

        /// <summary>
        /// 区域Id、园区id
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// 园区名称
        /// </summary>
        public string ParkName { get; set; } = "";

        /// <summary>
		/// 图标id
		/// </summary>
		public int IconId { get; set; }

        /// <summary>
        /// 图标完整信息
        /// </summary>
        public List<SunFileInfoDto> IconFiles { get; set; }
    }
}
