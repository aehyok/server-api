using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{
    /// <summary>
    /// 
    /// </summary>
    public class ModuleDictionaryDto
    {

        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 模块，ffp:防返贫
        /// </summary>
        public string Module { get; set; } = "common";

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 类型编码
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 顺序值，值越小越靠前
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        public string FontColor { get; set; } = "";
    }
}
