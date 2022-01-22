using DVS.Model.Dtos.Village;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Vaccination
{
    /// <summary>
    /// 人口疫苗接种统计表
    /// </summary>
    public class StatisticsVaccinationDto
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
        public long Household { get; set; }

        /// <summary>
        /// 需接种户籍人口数
        /// </summary>
        public long Population { get; set; } = 0;

        /// <summary>
        /// 已接种数
        /// </summary>
        public decimal Vaccinated { get; set; } = 0;

        /// <summary>
        /// 已接种第一针人数
        /// </summary>
        public decimal Vaccinated_first { get; set; } = 0;

        /// <summary>
        ///  在本地接种第一针人数
        /// </summary>
        public decimal Vaccinated_first_local { get; set; } = 0;

        /// <summary>
        /// 在外地接种第一针人数
        /// </summary>
        public decimal Vaccinated_first_nonlocal { get; set; } = 0;

        /// <summary>
        /// 已接种第二针人数
        /// </summary>
        public decimal Vaccinated_second { get; set; } = 0;

        /// <summary>
        /// 在本地接种第二针人数
        /// </summary>
        public decimal Vaccinated_second_local { get; set; } = 0;

        /// <summary>
        /// 在外地接种第二针人数
        /// </summary>
        public decimal Vaccinated_second_nonlocal { get; set; } = 0;

        /// <summary>
        /// 未登记人数
        /// </summary>
        public decimal NotRegister { get; set; } = 0;

        /// <summary>
        /// 未接种人数
        /// </summary>
        public decimal Notvaccinated { get; set; } = 0;

        /// <summary>
        /// 高龄
        /// </summary>
        public decimal Old { get; set; } = 0;

        /// <summary>
        /// 低龄
        /// </summary>
        public decimal Children { get; set; } = 0;

        /// <summary>
        /// 怀孕
        /// </summary>
        public decimal Pregnant { get; set; } = 0;

        /// <summary>
        /// 哺乳
        /// </summary>
        public decimal Lactation { get; set; } = 0;

        /// <summary>
        /// 因病
        /// </summary>
        public decimal Sick { get; set; } = 0;

        /// <summary>
        /// 外出
        /// </summary>
        public decimal Outwork { get; set; } = 0;

        /// <summary>
        /// 失联
        /// </summary>
        public decimal Missing { get; set; } = 0;

        /// <summary>
        /// 其他
        /// </summary>
        public decimal Other { get; set; } = 0;

        /// <summary>
        /// 已接种第三针人数
        /// </summary>
        public decimal Vaccinated_third { get; set; } = 0;

        /// <summary>
        /// 在本地接种第三针人数
        /// </summary>
        public decimal Vaccinated_third_local { get; set; } = 0;

        /// <summary>
        /// 在外地接种第三针人数
        /// </summary>
        public decimal Vaccinated_third_nonlocal { get; set; } = 0;
    }
}
