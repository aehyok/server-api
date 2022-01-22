using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.FFP
{
    /// <summary>
    /// 户码对应的扩展信息
    /// </summary>
   public  class FFPHouseholdExtraInfo:DvsEntityBase
    {
        /// <summary>
        /// 户码的id
        /// </summary>
        public int HouseholdId { get; set; }
        /// <summary>
        /// 属性键
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 属性值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 其他的补充说明
        /// </summary>
        public string Remark { get; set; }
    }
}
