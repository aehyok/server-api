using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP.Submit
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfirmWorkflowBody
    {
        /// <summary>
        /// workflow Id
        /// </summary>
        public int Id { get; set; } = 0;


        /// <summary>
        /// 不通过原因
        /// </summary>
        // public string Reason { get; set; } = "";

        /// <summary>
        /// 反馈Id
        /// </summary>
        public int FeedbackId { get; set; } = 0;
    }
}
