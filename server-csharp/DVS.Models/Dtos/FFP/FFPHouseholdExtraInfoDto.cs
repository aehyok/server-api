using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{
   public  class FFPHouseholdExtraInfoDto
    {
        /// <summary>
        /// 户码的id（保存的时候不用传）
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
