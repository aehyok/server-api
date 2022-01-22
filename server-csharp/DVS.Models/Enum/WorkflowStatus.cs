using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Enum
{
   //  1摸排确认2待评议3公示4待上报乡镇9结束
   public enum WorkflowStatus
    {
        /// <summary>
        /// 摸排确认
        /// </summary>
        Submit = 1,
        /// <summary>
        /// 待评议
        /// </summary>
        MopaiPassed = 2,
        /// <summary>
        /// 公示
        /// </summary>
        Publicity = 3,
        /// <summary>
        /// 上报到乡镇
        /// </summary>
        SubmitDistrict=4,
        /// <summary>
        /// 拒绝
        /// </summary>
        Rejected=9
    }
}
