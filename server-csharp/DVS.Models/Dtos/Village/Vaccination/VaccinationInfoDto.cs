using DVS.Model.Dtos.Village;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Vaccination
{
    /// <summary>
    /// 人口疫苗接种表
    /// </summary>
    public class VaccinationInfoDto
    {
        /// <summary>
        /// 人口Id
        /// </summary>
        public int PopulationId { get; set; } = 0;

        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 接种第一针
        /// </summary>
        public Int64 Vaccinated_first { get; set; } = 0;

        /// <summary>
        /// 接种第二针
        /// </summary>
        public Int64 Vaccinated_second { get; set; } = 0;

        /// <summary>
        /// 接种第三针
        /// </summary>
        public Int64 Vaccinated_third { get; set; } = 0;
    }
}
