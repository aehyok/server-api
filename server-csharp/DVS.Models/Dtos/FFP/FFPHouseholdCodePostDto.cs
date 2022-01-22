using DVS.Models.Dtos.Village;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{
    /// <summary>
    /// 防返贫住户详情
    /// </summary>
    public class FFPHouseholdCodePostDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// VillageHouseholdCode 户码Id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Mobile { get; set; }


        /// <summary>
        /// 监测对象，被监测的类型
        /// </summary>
        public string HouseholdType { get; set; }

        /// <summary>
        /// 脱贫户 1是 0否
        /// </summary>
        public int IsWithoutPoverty { get; set; } = 0;

        /// <summary>
        /// 是否在安置区 1是 0否
        /// </summary>
        public int IsInPlaceArea { get; set; } = 0;

        /// <summary>
        /// 家庭地址
        /// </summary>

        public PopulationAddressDto FamilyAddressInfo { get; set; }

        /// <summary>
        /// 安置区
        /// </summary>

        public PopulationAddressDto PlaceAreaAddressInfo { get; set; }
    }
}
