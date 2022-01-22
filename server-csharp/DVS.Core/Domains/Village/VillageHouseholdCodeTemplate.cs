using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.Village
{
	/// <summary>
	/// 户码门牌模板信息表
	/// </summary>
	public class VillageHouseholdCodeTemplate : DvsEntityBase
	{
		/// <summary>
		/// 行政代码
		/// </summary>
		public int AreaId { get; set; } = 0;

		/// <summary>
		/// 模板名称
		/// </summary>
		public string Name { get; set; } = "";

		/// <summary>
		/// 印刷尺寸宽mm
		/// </summary>
		public int PrintSizeWidth { get; set; }

		/// <summary>
		/// 印刷尺寸高mm
		/// </summary>
		public int PrintSizeHeight { get; set; }

		/// <summary>
		/// 二维码宽px
		/// </summary>
		public int QrCodeWidth { get; set; } = 0;

		/// <summary>
		/// 二维码高px
		/// </summary>
		public int QrCodeHeight { get; set; } = 0;

		/// <summary>
		/// 二维码纵坐标px
		/// </summary>
		public int QrCodeYaxis { get; set; } = 0;

		/// <summary>
		/// 二维码横坐标px
		/// </summary>
		public int QrCodeXaxis { get; set; } = 0;

		/// <summary>
		/// 门牌名字号px
		/// </summary>
		public int HouseNameFontSize { get; set; } = 0;

		/// <summary>
		/// 门牌名纵坐标px
		/// </summary>
		public int HouseNameYaxis { get; set; } = 0;

		/// <summary>
		/// 门牌名横坐标px
		/// </summary>
		public int HouseNameXaxis { get; set; } = 0;

		/// <summary>
		/// 门牌号字号px
		/// </summary>
		public int HouseNumberFontSize { get; set; } = 0;

		/// <summary>
		/// 门牌号纵坐标px
		/// </summary>
		public int HouseNumberYaxis { get; set; } = 0;

		/// <summary>
		/// 门牌号横坐标px
		/// </summary>
		public int HouseNumberXaxis { get; set; } = 0;

		/// <summary>
		/// 是否显示编号 1是，0否
		/// </summary>
		public int QrCodeNoShow { get; set; } = 0;

		/// <summary>
		/// 编号纵坐标px
		/// </summary>
		public int QrCodeNoYaxis { get; set; } = 0;

		/// <summary>
		/// 编号横坐标px
		/// </summary>
		public int QrCodeNoXaxis { get; set; } = 0;

		/// <summary>
		/// 图片路径
		/// </summary>
		public int Background { get; set; } = 0;
		/// <summary>
		/// 编号的字号大小
		/// </summary>
		public int QrCodeNoFontSize { get; set; } = 0;
		/// <summary>
		/// 备注
		/// </summary>
		public string Remark { get; set; } = "";

		public int IsDeleted { get; set; } = 0;

	}
}
