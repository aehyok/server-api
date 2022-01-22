using System;
using System.Collections.Generic;
using System.Text;
using DVS.Models.Dtos.Common;

namespace DVS.Models.Dtos.GIS
{
    /// <summary>
    /// 大棚
    /// </summary>
    public class GISGreenHouseDto
    {
        /// <summary>
        /// 唯一码id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名称
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
        /// 大棚类型id
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// 大棚类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 大棚类型对象
        /// </summary>
        public BasicDictionaryDto TypeDto { get; set; }

        /// <summary>
		/// 打点类型 1 区域 ,2 企业园区 
		/// </summary>
		public int Category { get; set; }

        /// <summary>
		/// 大棚面积
		/// </summary>
		public decimal Area { get; set; }

        /// <summary>
        /// 面积单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 大棚位置
        /// </summary>
        public string Address { get; set; }

        /// <summary>
		/// 户码Id、园区id
		/// </summary>
		public int ObjectId { get; set; }

        /// <summary>
        /// 大棚所属人/企业
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// 大棚所属人联系电话
        /// </summary>
        public string Phone { get; set; }

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
		/// 图标id
		/// </summary>
		public int IconId { get; set; }

        /// <summary>
        /// 图标完整信息
        /// </summary>
        public List<SunFileInfoDto> IconFiles { get; set; }
    }
}
