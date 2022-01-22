using System;
using System.Collections.Generic;
using System.Text;
using DVS.Common.Models;

namespace DVS.Core.Domains.GIS
{
	/// <summary>
	/// 公共设施
	/// </summary>
	public class GISCollectiveProperty : DvsEntityBase
	{
		/// <summary>
		/// 名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 区域id
		/// </summary>
		public int AreaId { get; set; }

		/// <summary>
		/// 设施类型id
		/// </summary>
		public int TypeId { get; set; }

		/// <summary>
		/// 打点类型 1 区域 ,2 企业园区 
		/// </summary>
		public int Category { get; set; }

		/// <summary>
		/// 区域Id、园区id
		/// </summary>
		public int ObjectId { get; set; }

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
		/// 备注
		/// </summary>
		public string Remark { get; set; }

		/// <summary>
		/// 图标id
		/// </summary>
		public int IconId { get; set; }
	}
}
