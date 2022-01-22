using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village
{
    /// <summary>
    /// 键值对
    /// </summary>
   public class KeyValueDto
    {
        /// <summary>
        /// 值
        /// </summary>
        public int Value { get; set; } = 0;

        /// <summary>
        /// 名
        /// </summary>
        public string Name { get; set; } = "";
    }
}
