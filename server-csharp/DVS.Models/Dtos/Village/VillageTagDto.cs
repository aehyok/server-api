using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village
{
    public class VillageTagDto
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = "";

        /// <summary>
        /// 户籍id
        /// </summary>
        public int Pid { get; set; } = 0;
        public string IconFileUrl { get; set; } = "";

        /// <summary>
        /// 颜色
        /// </summary>
        public string FontColor { get; set; } = "";
    }
}
