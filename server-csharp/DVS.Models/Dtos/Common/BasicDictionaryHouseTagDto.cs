using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Common
{
   
    /// <summary>
    /// 字典表
    /// </summary>
    public class BasicDictionaryHouseTagDto

    {
        public int Id { get; set; }

        /// <summary>
        /// 类型编码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 类型编码
        /// </summary>
        public int TypeCode { get; set; }

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


        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 图标文件id
        /// </summary>
        public int IconFileId { get; set; }

        /// <summary>
        /// iconFileUrl
        /// </summary>
        public string IconFileUrl { get; set; }


        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string UpdatedBy { get; set; } = "";

    }
}

