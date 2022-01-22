using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Query
{
    /// <summary>
    /// 外出务工管理
    /// </summary>
    public class SaveWorkBody
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 行政代码Id
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; }

        ///// <summary>
        ///// 工作所在地行政代码
        ///// </summary>
        //public string WorkOrgCodes { get; set; }

        ///// <summary>
        ///// 工作所在地详细地址
        ///// </summary>
        //public string WorkAddress { get; set; }

        /// <summary>
        /// 人口Id
        /// </summary>
        public int PopulationId { get; set; }

        /// <summary>
        /// 年度
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 行业
        /// </summary>
        public string Industry { get; set; }

        /// <summary>
        /// 职业
        /// </summary>
        public string Occupation { get; set; }

        /// <summary>
        /// 薪资范围
        /// </summary>
        public string Salary { get; set; }

        /// <summary>
        /// 工作单位
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }


        /// <summary>
        /// 工作地
        /// </summary>
        public PopulationAddressDto WorkAddressInfo { get; set; }
    }
}
