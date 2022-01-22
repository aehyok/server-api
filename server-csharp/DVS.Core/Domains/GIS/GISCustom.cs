using System;
using System.Collections.Generic;
using System.Text;
using DVS.Common.Models;

namespace DVS.Core.Domains.GIS
{
	/// <summary>
	/// 自定义标记
	/// </summary>
	public class GISCustom : DvsEntityBase
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
		/// 打点类型 1 区域 ,2 企业园区 
		/// </summary>
		public int Category { get; set; }

		/// <summary>
		/// 区域Id、园区id
		/// </summary>
		public int ObjectId { get; set; }

		/// <summary>
		/// 媒资类型 1 图片 2 视频 3 url
		/// </summary>
		public int MediaType { get; set; }

		/// <summary>
		/// 媒资id
		/// </summary>
		public int MediaId { get; set; }

		/// <summary>
		/// url
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// 图标id
		/// </summary>
		public int IconId { get; set; }

		/// <summary>
		/// 图标宽度
		/// </summary>
		public int Width { get; set; }

		/// <summary>
		/// 图标高度
		/// </summary>
		public int Height { get; set; }

		/// <summary>
		/// 备注
		/// </summary>
		public string Remark { get; set; }
	}
}
