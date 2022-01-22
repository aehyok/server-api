using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Common
{
    /// <summary>
    /// 门牌标签分页
    /// </summary>
    public class HouseTagQueryModel : PagedListQueryModel
    {
        /// <summary>
        /// 字典门牌标签父节点Code
        /// </summary>
        public int TypeCode { get; set; } = 2010;
    }
}
