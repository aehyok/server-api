using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP.Query
{
    /// <summary>
    /// 分页获取字典列表
    /// </summary>
  public   class ModuleDictionaryPageListReq:PagedListQueryModel
    {
        /// <summary>
        /// 字典类型编码
        /// </summary>
        public string TypeCode { get; set; }
    }
}
