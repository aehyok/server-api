using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Export
{
    /// <summary>
    /// 
    /// </summary>
   public class ExportHouseHoldCodeDto
    {

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; } = 0;

        
        /// <summary>
        /// 
        /// </summary>
        public string HouseName { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>

        public string HouseNumber { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>

        public string Remark { get; set; } = "";

    }
}
