using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village
{
    /// <summary>
    /// 门牌号
    /// </summary>
    public class HouseNumberDto
    {
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
        /// 户主名字
        /// </summary>
        public string HouseholdMan { get; set; } = "";

    }
}
