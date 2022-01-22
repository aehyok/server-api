using System;
using System.Collections.Generic;
using System.Text;
using DVS.Common.Models;
using Newtonsoft.Json;

namespace DVS.Core.Domains.Common
{
    /// <summary>
    /// 行政区域
    /// </summary>
    public class BasicArea : DvsEntityBase
    {
        /// <summary>
        /// 区域名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 行政区域编码
        /// </summary>
        public long AreaCode { get; set; }

        /// <summary>
        /// 父级区域id
        /// </summary>
        public int Pid { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// ips组织或门店id
        /// </summary>
        public string IpsCompanyId { get; set; }
    }
}
