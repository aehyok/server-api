using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.Common
{
    public class ModuleDictionaryType : DvsEntityBase
    {
        /// <summary>
        /// 模块，ffp:防返贫
        /// </summary>
        public string Module { get; set; } = "common";
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sequence { get; set; }
    }
}
