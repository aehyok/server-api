using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.GIS.Query
{
    /// <summary>
    /// 明细查询条件
    /// </summary>
    public class GISDetailQueryModel
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 区域id
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 类型id
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 地块id,户码id,园区id
        /// </summary>
        public int ObjectId { get; set; }

    }
}
