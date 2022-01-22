using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{
    /// <summary>
    /// 评议报告管理列表
    /// </summary>
    public class ReviewReportManageDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 生成日期
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreatedByName { get; set; }

        /// <summary>
        /// 上报状态
        /// </summary>
        public string ReportStatus { get; set; }

        /// <summary>
        /// 上报日期
        /// </summary>
        public DateTime ReportDate { get; set; }

        /// <summary>
        /// 上报人员
        /// </summary>
        public string Reporter { get; set; }




    }
}
