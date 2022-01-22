using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP.Query
{
    public class ModuleDictionaryStatusReq
    {
        /// <summary>
        /// 字典编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 状态，1启用，0停用
        /// </summary>
        public int Status { get; set; }
    }
}
