using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.Village
{
    /// <summary>
    /// 户码标签管理表
    /// </summary>
    public class VillageHouseCodeTagManage : DvsEntityBase
    {
        /// <summary>
        /// 标签名
        /// </summary>
        public string TagName { get; set; } = "";

        /// <summary>
        /// 排序
        /// </summary>
        public int Sequence { get; set; } = 0;

        /// <summary>
        /// 图片路径
        /// </summary>
        public int Background { get; set; } = 0;

        /// <summary>
        /// 详情
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";

    }
}
