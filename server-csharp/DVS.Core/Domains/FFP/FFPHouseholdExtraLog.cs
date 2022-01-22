using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.FFP
{
    /// <summary>
    /// 用户的额外信息修改日志
    /// </summary>
    public  class FFPHouseholdExtraLog: DvsEntityBase
    {
        /// <summary>
        /// 户的id
        /// </summary>
        public int HouseholdId { get; set; }
        /// <summary>
        /// 修改内容
        /// </summary>

        public string Message { get; set; }
    }
}
