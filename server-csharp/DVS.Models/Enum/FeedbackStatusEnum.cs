using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Enum
{
    /// <summary>
    /// 反馈表状态0待审核 1待反馈2已反馈3待确认4已确认
    /// </summary>
    public enum FeedbackStatusEnum
    {

        /// <summary>
        /// 待审核
        /// </summary>
        TobeReview = 0,

        /// <summary>
        /// 待反馈
        /// </summary>
        TobeFeedback = 1,

        /// <summary>
        /// 已反馈
        /// </summary>
        Feedbacked = 2,

        /// <summary>
        /// 待确认
        /// </summary>
        ToBeConfirm = 3,

        /// <summary>
        /// 已确认
        /// </summary>
        Confirmed = 4,
    }
}
