using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class ImportResultMessage
    { 
    
        /// <summary>
        /// 第几行
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DetailError { get; set; }

    }

    /// <summary>
    /// 导入返回结果
    /// </summary>
    public class ImportResultDto
    {

        /// <summary>
        /// 成功数量
        /// </summary>
        public long SuccessCount { get; set; } = 0;
        /// <summary>
        /// 失败数量
        /// </summary>
        public long FailCount { get; set; } = 0;

        /// <summary>
        /// 错误列表
        /// </summary>

        public List<ImportResultMessage> Errors { get; set; }

    }
}
