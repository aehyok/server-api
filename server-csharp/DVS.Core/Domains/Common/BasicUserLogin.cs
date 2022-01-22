using System;
using System.Collections.Generic;
using System.Text;
using DVS.Common.Models;
using Newtonsoft.Json;

namespace DVS.Core.Domains.Common
{
    /// <summary>
    /// 用户登录信息
    /// </summary>
    public class BasicUserLogin : DvsEntityBase
    {
       


        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; } = 0;

        /// <summary>
        /// 极光推动别名
        /// </summary>
        public string PushId { get; set; } = "";

        /// <summary>
        /// 手机厂商名称
        /// </summary>
        public string Manufacturer { get; set; } = "";


      

    }
}
