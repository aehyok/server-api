using DVS.Models.Dtos.Village;
using DVS.Models.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Model.Dtos.Village
{
    /// <summary>
    /// 户主信息
    /// </summary>
    public class HouseholderDto
    {
        /// <summary>
        /// 人口Id
        /// </summary>
        public int PopulationId { get; set; } = 0;
        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; } = "";
       
        
      
        /// <summary>
        /// 联系方式
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 户主头像
        /// </summary>
        // public string HeadImageUrl { get; set; } = "";
     
        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; }
       
        /// <summary>
        /// 门牌名
        /// </summary>
        public string HouseName { get; set; }
        /// <summary>
        /// 门牌号
        /// </summary>
        public string HouseNumber { get; set; }
       
        /// <summary>
        /// 区域Id
        /// </summary>
        public int AreaId { get; set; }
        /// <summary>
        /// 社区/村名称
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 全名称，包括村
        /// </summary>
        public string FullName { get; set; } = "";


    }
}
