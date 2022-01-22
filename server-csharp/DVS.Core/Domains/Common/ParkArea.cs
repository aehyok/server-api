using System;
using System.Collections.Generic;
using System.Text;
using DVS.Common.Models;
using Newtonsoft.Json;

namespace DVS.Core.Domains.Common
{
    /// <summary>
    /// 园区
    /// </summary>
    public class ParkArea : DvsEntityBase
    {
        /// <summary>
        /// 园区名称
        /// </summary>
        public string EnterpriseName { get; set; }

    }
}
