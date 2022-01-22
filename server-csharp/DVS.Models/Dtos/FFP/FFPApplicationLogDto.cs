using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{
    public class FFPApplicationLogDto
    {
        public int? CreatedBy { get; set; } = 0;
        /// <summary>
        /// 操作用户
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 操作内容
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
