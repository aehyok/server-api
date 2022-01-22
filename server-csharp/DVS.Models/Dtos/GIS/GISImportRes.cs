using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.GIS
{
    /// <summary>
    /// 数据导入返回
    /// </summary>
    public class GISImportRes
    {
        /// <summary>
        /// 导入成功数量
        /// </summary>
        public int Ok { get; set; } = 0;

        /// <summary>
        /// 导入失败数量及原因
        /// </summary>
        public List<GISImportFailInfo> Fail { get; set; } = new List<GISImportFailInfo>();
    }

    /// <summary>
    /// 导入失败详细信息
    /// </summary>
    public class GISImportFailInfo {
        /// <summary>
        /// 失败记录数
        /// </summary>
        public int Index { get; set; } = 0;

        /// <summary>
        /// 失败原因
        /// </summary>
        public string Message { get; set; } = "";
    }
}
