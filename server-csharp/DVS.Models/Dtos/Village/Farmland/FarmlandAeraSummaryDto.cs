using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Farmland
{
    public class FarmlandAreaSummaryDto
    {
        /// <summary>
        /// 区域ID
        /// </summary>
        public int AreaId { get; set; }
        /// <summary>
        /// 户码的Id
        /// </summary>
        public int HouseholdId { get; set; }
 
        /// <summary>
        /// 土地类型的编码
        /// </summary>
        public int TypeId { get; set; }
        //----------
        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 门牌名
        /// </summary>
        public string HouseName { get; set; }
        /// <summary>
        /// 门牌号
        /// </summary>
        public string HouseNumber { get; set; }
        /// <summary>
        /// 户主
        /// </summary>
        public string Householder { get; set; }
        /// <summary>
        /// 土地类型名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 地块所属类型 1 区域 ,2 园区
        /// </summary>
        public int Category { get; set; }

        /// <summary>
        /// 地块用途 1 普通用地 ,2 规划用地
        /// </summary>
        public int UseFor { get; set; }
        /// <summary>
        /// 统计信息
        /// </summary>
        public List<FarmlandAreaSummary> Summarys { get; set; }


        /// <summary>
        /// 门牌名排序
        /// </summary>
        public long HouseNameSequence { get; set; } = 0;
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
    }
}
