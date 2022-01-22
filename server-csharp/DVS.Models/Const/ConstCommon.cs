using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Const
{
    /// <summary>
    /// 基本的常量
    /// </summary>
    public static class ConstCommon
    {
        /// <summary>
        /// 申请用户认证发送短信redis key
        /// </summary>
        public static readonly string USER_AUTH_SMS_CODE_REDIS = "user_auth_sms_code";

    }

    public static class SystemParams {
        public static string VillageVersion = "2.0.0.004";
        public static string ConsVersion = "2.0.0.004";
    }
}
