using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP.Submit
{
    /// <summary>
    /// 保存农户的扩展信息请求参数
    /// </summary>
    public class HouseholdeExtraSaveReq
    {
        /// <summary>
        /// 户的id
        /// </summary>
        public int HouseholdId { get; set; }
        /// <summary>
        /// 扩展信息
        /// </summary>
        public List<FFPHouseholdExtraInfoDto> ExtraInfos { get; set; }
    }
}
