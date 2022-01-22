using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.Warning
{
    /// <summary>
    /// 告警操作日志
    /// </summary>
    public class WarningOperationLog: DvsEntityBase
    {
        /// <summary>
        /// 告警Id
        /// </summary>
        public int WarningMessageId { get; set; } = 0;

        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator { get; set; } = "";

       
        /// <summary>
        /// 操作说明
        /// </summary>
        public string Description { get; set; } = "";


        /// <summary>
        /// 操作类型描述
        /// </summary>
        public string Operation { get; set; } = "";


    }
}
