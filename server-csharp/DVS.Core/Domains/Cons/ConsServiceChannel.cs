using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.Cons
{
    /// <summary>
    /// 便民渠道查询实体表
    /// </summary>
    public class ConsServiceChannel : DvsEntityBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 介绍
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 补充说明
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// URL地址
        /// </summary>
        public string Url { get; set; }
              



    }
}
