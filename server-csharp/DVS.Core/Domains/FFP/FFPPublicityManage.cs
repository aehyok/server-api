using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.FFP
{
    /// <summary>
    /// 评议公示/报告管理
    /// </summary>
    public class FFPPublicityManage : DvsEntityBase
    {
        /// <summary>
        /// 类型  1评议公示2评议报告
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 区域ID
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 发布状态   0未发布 1已发布
        /// </summary>
        public int PublishStatus {get;set;}

        /// <summary>
        /// 发布日期
        /// </summary>
        public DateTime PublishDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 发布人员
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// 上报状态  0未上报 1已上报
        /// </summary>
        public int ReportStatus { get; set; }

        /// <summary>
        /// 上报日期
        /// </summary>
        public DateTime ReportDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 上报人员
        /// </summary>
        public string Reporter { get; set; }

        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreatedUser { get; set; }


    }
}
