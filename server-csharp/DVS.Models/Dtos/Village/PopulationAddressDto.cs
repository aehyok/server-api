using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village
{

    public class PopulationAddressDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public string Province { get; set; } = "";

        /// <summary>
        /// 市
        /// </summary>
        public string City { get; set; } = "";

        /// <summary>
        /// 区
        /// </summary>
        public string District { get; set; } = "";

        /// <summary>
        /// 省市区对应行政编码
        /// </summary>
        public string MapCode { get; set; } = "";

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; } = "";

    }
}
