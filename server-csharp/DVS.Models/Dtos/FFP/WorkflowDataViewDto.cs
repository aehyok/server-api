using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{
    /// <summary>
    /// 处理任务统计
    /// </summary>
    public class WorkflowDataViewDto
    {
        /// <summary>
        /// 待处理个数
        /// </summary>
        public long PendingCount { get; set; } = 0;


        /// <summary>
        /// 已处理个数
        /// </summary>
        public long ProcessedCount { get; set; } = 0;
         
    }
}
