using System;
using System.Collections.Generic;
using System.Text;
using DVS.Common.Models;

namespace DVS.Core.Domains.GIS
{
	/// <summary>
	/// 大棚
	/// </summary>
	public class GISGreenHouse : DvsEntityBase
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
		/// 大棚类型id
		/// </summary>
		public int TypeId { get; set; }

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

		///// <summary>
		///// 大棚所属人/企业
		///// </summary>
		//public string Owner { get; set; }

		/// <summary>
		/// 大棚所属人联系电话
		/// </summary>
		public string Phone { get; set; }

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
