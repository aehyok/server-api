using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Export
{
    /// <summary>
    /// 
    /// </summary>
   public class ExportPopulationDto
    {

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public string IdCard { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public DVS.Models.Enum.PopulationGender Sex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Mobile { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string RealName { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>

        public DateTime Birthday { get; set; }

        /// <summary>
        /// 
        /// </summary>

        public string RegisterAddress { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>

        public string LiveAddress { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>

        public string Remark { get; set; } = "";

    }
}
