using System;
using System.Collections.Generic;
using System.Text;
using DVS.Models.Dtos.Common;

namespace DVS.Models.Dtos.GIS
{
    /// <summary>
    /// 自定义标记
    /// </summary>
    public class GISCustomDto
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
		/// 打点类型 1 区域 ,2 企业园区 
		/// </summary>
		public int Category { get; set; }

        /// <summary>
		/// 媒资类型 1 图片 2 视频 3 url
		/// </summary>
		public int MediaType { get; set; }

        /// <summary>
        /// 媒资id
        /// </summary>
        public int MediaId { get; set; }

        /// <summary>
        /// 图片信息
        /// </summary>
        public List<SunFileInfoDto> MediaFiles { get; set; }        

        /// <summary>
        /// url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
		/// 图标id
		/// </summary>
		public int IconId { get; set; }

        /// <summary>
        /// 图标完整信息
        /// </summary>
        public List<SunFileInfoDto> IconFiles { get; set; }

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
        /// 图标宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 图标高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 区域Id、园区id
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// 园区名称
        /// </summary>
        public string ParkName { get; set; } = "";
    }
}
