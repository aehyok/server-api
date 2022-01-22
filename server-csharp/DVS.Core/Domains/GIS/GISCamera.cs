using System;
using System.Collections.Generic;
using System.Text;
using DVS.Common.Models;

namespace DVS.Core.Domains.GIS
{
	/// <summary>
	/// 摄像头
	/// </summary>
	public class GISCamera : DvsEntityBase
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
		/// 摄像头位置
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// 摄像头流地址
		/// </summary>
		public string StreamUrl { get; set; }

		/// <summary>
		/// 直播流地址
		/// </summary>
		public string Url { get; set; }

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
