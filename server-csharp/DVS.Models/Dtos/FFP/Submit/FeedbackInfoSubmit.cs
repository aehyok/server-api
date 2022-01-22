using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP.Submit
{
    /// <summary>
    /// 
    /// </summary>
    public class FeedbackInfoSubmit
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id { get; set; }
        ///// <summary>
        ///// 状态 1待反馈2已反馈3已确认
        ///// </summary>
        //public int Status { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Info { get; set; }
    }
}
