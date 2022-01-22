using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{
    /// <summary>
    /// 防返贫住户详情
    /// </summary>
    public class FFPHouseholdCodeDetailDto
    {
        /// <summary>
        /// 户码信息
        /// </summary>
        public FFPHouseholdCodeDto FFPHouseholdCodeInfo { get; set; }

        /// <summary>
        /// 家庭成员信息
        /// </summary>
        public List<FFPPopulationDto> FFPPopulationList { get; set; }
    }
}
