using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Farmland
{
    public class FarmlandImportRes
    {
        /// <summary>
        /// 导入成功数量
        /// </summary>
        public int Ok { get; set; } = 0;

        /// <summary>
        /// 导入失败数量及原因
        /// </summary>
        public List<ImportFailInfo> Fail { get; set; } = new List<ImportFailInfo>();
    }

    public class ImportFailInfo {
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
