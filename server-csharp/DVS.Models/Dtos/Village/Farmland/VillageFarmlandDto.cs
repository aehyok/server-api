using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Farmland
{
    public class VillageFarmlandDto
    {
        public int Id { get; set; }
        /// <summary>
        /// 区域ID
        /// </summary>
        public int AreaId { get; set; }
        /// <summary>
        /// 户码的Id
        /// </summary>
        public int HouseholdId { get; set; }
        /// <summary>
        /// 园区的ID
        /// </summary>
        public int ParkId { get; set; }
        /// <summary>
        /// 园区名称
        /// </summary>
        public string ParkName { get; set; }
        /// <summary>
        /// 土地类型的编码
        /// </summary>
        public int TypeId { get; set; }
        /// <summary>
        /// 土地面积
        /// </summary>
        public decimal Area { get; set; }
        /// <summary>
        /// 面积单位
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 地块地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 地块名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

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
        /// 土地类型名称
        /// </summary>
        public string TypeNameCode { get; set; }

        /// <summary>
        /// 土地类型对象
        /// </summary>
        public BasicDictionaryDto TypeDto { get; set; }

        /// <summary>
        /// 地块所属类型 1 区域 ,2 园区
        /// </summary>
        public int Category { get; set; }

        /// <summary>
        /// 地块用途 1 普通用地 ,2 规划用地
        /// </summary>
        public int UseFor { get; set; }
        /// <summary>
        /// 是否已经打点
        /// </summary>
        public bool isPloted { get; set; }

        /// <summary>
        /// 删除标记
        /// </summary>
        public int IsDeleted { get; set; }

        public string Mobile { get; set; }
    }
}
