using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DVS.Models.Dtos.Village.Household
{
    public class VillageHouseholdCodeTemplateDto
    {
		public int Id { get; set; }
		public string TemplateImage { get; set; }
		/// <summary>
		/// 模板背景图URL 
		/// </summary>
		public SunFileInfoDto BackgroundInfo { get; set; }
		/// <summary>
		/// 背景的id
		/// </summary>
		[Required(ErrorMessage = "请上传背景图")]
		public int Background { get; set; }

		/// <summary>
		/// 行政区域编码，数据库不存储
		/// </summary>
		public  long AreaCode { get; set; }
		/// <summary>
		/// 行政代码
		/// </summary>
		[Required(ErrorMessage ="无效的区域编码")]
		[Range(1,int.MaxValue,ErrorMessage ="行政区域编码必须大于0")]
		public int AreaId { get; set; } = 0;

		/// <summary>
		/// 模板名称
		/// </summary>
		[Required(ErrorMessage ="请填写名称")]
		[MaxLength(20)]
		public string Name { get; set; } = "";

		/// <summary>
		/// 印刷尺寸宽mm
		/// </summary>
		[Required(ErrorMessage ="请填印刷宽度")]
		[Range(1, int.MaxValue,ErrorMessage = "印刷宽度必须大于0")]
		public int PrintSizeWidth { get; set; }

		/// <summary>
		/// 印刷尺寸高mm
		/// </summary>
		[Required(ErrorMessage ="请填写印刷高度")]
		[Range(1, int.MaxValue, ErrorMessage = "印刷高度必须大于0")]
		public int PrintSizeHeight { get; set; }

		/// <summary>
		/// 二维码宽px
		/// </summary>
		[Required(ErrorMessage ="请填写二维码的宽度")]
		[Range(1, int.MaxValue,ErrorMessage = "二维码的宽度必须大于0")]
		public int QrCodeWidth { get; set; } = 0;

		/// <summary>
		/// 二维码高px
		/// </summary>
		[Required(ErrorMessage ="请填写二维码的高度")]
		[Range(1, int.MaxValue)]
		public int QrCodeHeight { get; set; } = 0;

		/// <summary>
		/// 二维码纵坐标px
		/// </summary>
		[Required(ErrorMessage ="请填写二维码的纵坐标")]
		[Range(1, int.MaxValue,ErrorMessage = "二维码的纵坐标必须大于0")]
		public int QrCodeYaxis { get; set; } = 0;

		/// <summary>
		/// 二维码横坐标px
		/// </summary>
		[Required(ErrorMessage ="请填写二维码的横坐标")]
		[Range(1, int.MaxValue,ErrorMessage = "二维码的横坐标必须大于0")]
		public int QrCodeXaxis { get; set; } = 0;

		/// <summary>
		/// 门牌名字号px
		/// </summary>
		[Required(ErrorMessage ="请填写门牌名的字号")]
		[Range(1, int.MaxValue,ErrorMessage = "门牌名的字号必须大于0")]
		public int HouseNameFontSize { get; set; } = 0;

		/// <summary>
		/// 门牌名纵坐标px
		/// </summary>
		[Required(ErrorMessage ="请填写门牌纵坐标")]
		[Range(1, int.MaxValue,ErrorMessage = "门牌纵坐标必须大于0")]
		public int HouseNameYaxis { get; set; } = 0;

		/// <summary>
		/// 门牌名横坐标px
		/// </summary>
		[Required(ErrorMessage ="请填写门牌名横坐标")]
		[Range(1, int.MaxValue,ErrorMessage ="门牌横坐标必须大于0")]
		public int HouseNameXaxis { get; set; } = 0;

		/// <summary>
		/// 门牌号字号px
		/// </summary>
		[Required(ErrorMessage = "请填写门牌号字号")]
		[Range(1, int.MaxValue,ErrorMessage = "门牌号字号必须大于0")]
		public int HouseNumberFontSize { get; set; } = 0;

		/// <summary>
		/// 门牌号纵坐标px
		/// </summary>
		[Required(ErrorMessage = "请填写门牌纵坐标")]
		[Range(1, int.MaxValue,ErrorMessage = "门牌纵坐标必须大于0")]
		public int HouseNumberYaxis { get; set; } = 0;

		/// <summary>
		/// 门牌号横坐标px
		/// </summary>
		[Required(ErrorMessage = "请填写门牌号横坐标")]
		[Range(1, int.MaxValue,ErrorMessage = "门牌号横坐标必须大于0")]
		public int HouseNumberXaxis { get; set; } = 0;

		/// <summary>
		/// 是否显示编号 1是，0否
		/// </summary>
		[Required(ErrorMessage ="请确认是否显示编号")]
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
		/// 编号的字号大小
		/// </summary>
		public int QrCodeNoFontSize { get; set; }
		/// <summary>
		/// 备注
		/// </summary>
		public string Remark { get; set; } = "";
	}
}
