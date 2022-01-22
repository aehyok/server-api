using DVS.Model.Dtos.Village;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Statistics
{
    /// <summary>
    /// 返乡统计表
    /// </summary>
    public class StatisticsEpidemicDto
    {
        /// <summary>
        /// 行政代码Id
        /// </summary>
        public int AreaId { get; set; } = 0;

        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 户数
        /// </summary>
        public long Household { get; set; } = 0;

        /// <summary>
        /// 户籍人口数
        /// </summary>
        public long Population { get; set; } = 0;

        /// <summary>
        /// 1月返乡人数
        /// </summary>
        public decimal Normal1 { get; set; } = 0;

        /// <summary>
        /// 2月返乡人数
        /// </summary>
        public decimal Normal2 { get; set; } = 0;

        /// <summary>
        /// 3月返乡人数
        /// </summary>
        public decimal Normal3 { get; set; } = 0;

        /// <summary>
        /// 4月返乡人数
        /// </summary>
        public decimal Normal4 { get; set; } = 0;

        /// <summary>
        /// 5月返乡人数
        /// </summary>
        public decimal Normal5 { get; set; } = 0;

        /// <summary>
        /// 6月返乡人数
        /// </summary>
        public decimal Normal6 { get; set; } = 0;

        /// <summary>
        /// 7月返乡人数
        /// </summary>
        public decimal Normal7 { get; set; } = 0;

        /// <summary>
        /// 8月返乡人数
        /// </summary>
        public decimal Normal8 { get; set; } = 0;

        /// <summary>
        /// 9月返乡人数
        /// </summary>
        public decimal Normal9 { get; set; } = 0;

        /// <summary>
        /// 10月返乡人数
        /// </summary>
        public decimal Normal10 { get; set; } = 0;

        /// <summary>
        /// 11月返乡人数
        /// </summary>
        public decimal Normal11 { get; set; } = 0;

        /// <summary>
        /// 12月返乡人数
        /// </summary>
        public decimal Normal12 { get; set; } = 0;

        /// <summary>
        /// 1月返乡人数
        /// </summary>
        public decimal Unnormal1 { get; set; } = 0;

        /// <summary>
        /// 2月返乡人数
        /// </summary>
        public decimal Unnormal2 { get; set; } = 0;

        /// <summary>
        /// 3月返乡人数
        /// </summary>
        public decimal Unnormal3 { get; set; } = 0;

        /// <summary>
        /// 4月返乡人数
        /// </summary>
        public decimal Unnormal4 { get; set; } = 0;

        /// <summary>
        /// 5月返乡人数
        /// </summary>
        public decimal Unnormal5 { get; set; } = 0;

        /// <summary>
        /// 6月返乡人数
        /// </summary>
        public decimal Unnormal6 { get; set; } = 0;

        /// <summary>
        /// 7月返乡人数
        /// </summary>
        public decimal Unnormal7 { get; set; } = 0;

        /// <summary>
        /// 8月返乡人数
        /// </summary>
        public decimal Unnormal8 { get; set; } = 0;

        /// <summary>
        /// 9月返乡人数
        /// </summary>
        public decimal Unnormal9 { get; set; } = 0;

        /// <summary>
        /// 10月返乡人数
        /// </summary>
        public decimal Unnormal10 { get; set; } = 0;

        /// <summary>
        /// 11月返乡人数
        /// </summary>
        public decimal Unnormal11 { get; set; } = 0;

        /// <summary>
        /// 12月返乡人数
        /// </summary>
        public decimal Unnormal12 { get; set; } = 0;

        /// <summary>
        /// 异常人数总数
        /// </summary>
        public long Unnormaltotal { get; set; } = 0;

        /// <summary>
        /// 14天内异常总数
        /// </summary>
        public long Unnormal14 { get; set; } = 0;

    }
}
