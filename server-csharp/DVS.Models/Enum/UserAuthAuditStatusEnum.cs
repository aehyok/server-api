using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DVS.Models.Enum
{

    /// <summary>
    /// 村民认证审核状态枚举 0未申请， 1待审核，2审核通过，3审核失败
    /// </summary>
    public enum UserAuthAuditStatusEnum
    {
        /// <summary>
        /// 0未申请
        /// </summary>
        [Description("未申请")]
        Not = 0,
        /// <summary>
        /// 1待审核
        /// </summary>
        [Description("待审核")]
        Waiting = 1,
        /// <summary>
        /// 2审核通过
        /// </summary>
        [Description("审核通过")]
        Passed = 2,
        /// <summary>
        /// 3审核失败
        /// </summary>
        [Description("审核失败")]
        Failed = 3,
    }
}
