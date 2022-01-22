using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP.Query
{
    /// <summary>
    /// 获取列表请求参数
    /// </summary>
   public  class ModuleDictionaryListReq
    {
        /// <summary>
        /// 字典类型编码
        /// </summary>
        public List<string> TypeCodes { get; set; }

        public string Keyword { get; set; }
    }
}
