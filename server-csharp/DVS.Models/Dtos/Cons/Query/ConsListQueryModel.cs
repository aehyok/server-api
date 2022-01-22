using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Cons.Query
{
    public class ConsListQueryModel : PagedListQueryModel
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        public string Startdate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public string Enddate { get; set; }

        /// <summary>
        /// 信息公开类型
        /// 1 三务公开、2 党建宣传、3 精神文明、4 便民服务
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 区域id
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 类型集合
        /// </summary>
        public List<int> Types { get; set; }
    }
}
