using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{
    /// <summary>
    /// 保存防返贫户码信息
    /// </summary>
    public class FFPHouseholdCodeInfoPostDto
    {
        /// <summary>
        /// 户码信息
        /// </summary>
        public FFPHouseholdCodePostDto FFPHouseholdCodeInfo { get; set; }

        /// <summary>
        /// 家庭成员信息
        /// </summary>
        public List<FFPPopulationPostDto> FFPPopulationList { get; set; }
    }
}
