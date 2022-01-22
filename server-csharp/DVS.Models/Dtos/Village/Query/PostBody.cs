using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Query
{
    public class PostBody: PagedListQueryModel
    {
        
        /// <summary>
        /// 行政区域Id
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 状态 0禁用，1正常
        /// </summary>
        public int Status { get; set; } = -1;

        /// <summary>
        /// 门牌名称
        /// </summary>
        public string HouseName { get; set; } = "";

        /// <summary>
        /// 批量查询
        /// </summary>
        public List<int> Ids { get; set; }


        /// <summary>
        /// 标签Ids，例如：1,2,3
        /// </summary>
        public string Tags { get; set; } = "";
        
    }
}
