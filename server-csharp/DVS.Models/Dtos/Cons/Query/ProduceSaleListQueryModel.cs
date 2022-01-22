using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Cons.Query
{
    public class ProduceSaleListQueryModel : PagedListQueryModel
    {
        /// <summary>
        /// 区域ID
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///  类型 1售卖中 2已截止
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 类目ID
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 发布人id
        /// </summary>
        public int PublishId { get; set; }
    }
}
