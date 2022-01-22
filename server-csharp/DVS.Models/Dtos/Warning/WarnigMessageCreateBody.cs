using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Warning
{
    public class WarnigMessageCreateBody
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; } = "";


        /// <summary>
        /// 区域Id
        /// </summary>
        public int AreaId { get; set; } = 0;


        /// <summary>
        /// 告警详情
        /// </summary>
        public string Descrition { get; set; } = "";

        /// <summary>
        /// 灾情地点
        /// </summary>
        public string Address { get; set; } = "";


        /// <summary>
        /// 严重程度，1 一般，2严重
        /// </summary>
        public int Level { get; set; } = 1;

        /// <summary>
        /// 告警类别 1 火灾，2 高温，3 水灾，4 虫情
        /// </summary>
        public int Category { get; set; } = 0;

        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImageUrl { get; set; } = "";

        /// <summary>
        /// 视频地址或推流地址
        /// </summary>
        public string VideoUrl { get; set; } = "";

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; } = "";
    }
}
