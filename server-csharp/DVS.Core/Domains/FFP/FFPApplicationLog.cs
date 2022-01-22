using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.FFP
{
   public  class FFPApplicationLog : DvsEntityBase
    {
        /// <summary>
        /// 申请书的id
        /// </summary>
        public int ApplicationId { get; set; }
        /// <summary>
        /// 修改内容
        /// </summary>

        public string Message { get; set; }
    }
}
