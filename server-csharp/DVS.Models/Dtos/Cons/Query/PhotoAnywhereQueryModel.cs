using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Cons.Query
{
    public class PhotoAnywhereListQueryModel : PagedListQueryModel
    {
        /// <summary>
        /// 是否回复 1 待回复，2 已回复
        /// </summary>
        public int IsReply { get; set; }

        /// <summary>
        /// 登录用户id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 随手拍类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 区域ID
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 村民户籍id
        /// </summary>
        public int PopulationId { get; set; }

        /// <summary>
        /// 户码id
        /// </summary>
        public int HouseHoldId { get; set; }
    }
}
