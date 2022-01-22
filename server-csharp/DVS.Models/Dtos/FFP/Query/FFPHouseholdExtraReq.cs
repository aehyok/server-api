using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP.Query
{
    /// <summary>
    /// 获取农户详情的扩展信息
    /// </summary>
    public class FFPHouseholdExtraReq
    {
        /// <summary>
        /// 户码的id
        /// </summary>
        public int HouseholdId { get; set; }
        /// <summary>
        /// 字典类型的编码(选填，如果不填，返回所有)
        /// </summary>
        public List<string> TypeCodes { get; set; }
    }
}
