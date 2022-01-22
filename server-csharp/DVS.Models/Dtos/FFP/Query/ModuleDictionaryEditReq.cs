using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP.Query
{
    /// <summary>
    /// 修改字典
    /// </summary>
    public class ModuleDictionaryEditReq
    {
        /// <summary>
        /// 字典的id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 字典的编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图标的文件
        /// </summary>
        public int IconFileId { get; set; }
        /// <summary>
        /// 字典前景色
        /// </summary>
        public string FontColor { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark{get;set;}
        /// <summary>
        /// 序号，越小越靠前
        /// </summary>
        public int Sequence { get; set; }
    }
}
