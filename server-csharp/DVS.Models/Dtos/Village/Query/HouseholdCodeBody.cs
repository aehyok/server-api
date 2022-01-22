using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Query
{
    public class HouseholdCodeBody
    {

        /// <summary>
        /// Id ，0新增，大于0编辑
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 区域ID
        /// </summary>
        public int AreaId { get; set; }
        /// <summary>
        /// 组名，门牌名Id
        /// </summary>
        public int HouseNameId { get; set; }
        /// <summary>
        /// 门牌号
        /// </summary>
        public string HouseNumber { get; set; }
        /// <summary>
        /// 标签ids
        /// </summary>
        public string Tags { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; } = 1;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
