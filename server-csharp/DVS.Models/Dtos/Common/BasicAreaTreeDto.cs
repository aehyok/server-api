using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Common
{
    public class BasicAreaTreeDto
    {
        public int Id { get; set; }    
        public int PId { get; set; }
        public string Name { get; set; }
        public int Level { get; set; } // 区域层级，1:省份 2:地市 3:区县 4:乡镇 5:社区/村
        public int Sequence { get; set; }   // 顺序值

        /// <summary>
        /// 本级及所有下级的id
        /// </summary>
        public List<int> Ids { get; set; } = new List<int>();
        public List<BasicAreaTreeDto> Children { get; set; }
    }
}
