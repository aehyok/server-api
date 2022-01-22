using System;
using System.Collections.Generic;
using System.Text;
using DVS.Common.Models;
using DVS.Core.Domains.Common;
using Newtonsoft.Json;

namespace DVS.Core.Domains.Cons
{
    public class ConsTypePhotoAnywhere : DvsEntityBase
    {
        public string Type { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public long Cnt { get; set; }
    }
}
