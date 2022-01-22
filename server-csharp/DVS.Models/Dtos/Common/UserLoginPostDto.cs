using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Common
{
    public class UserLoginPostDto
    {
        ///// <summary>
        ///// 用户Id
        ///// </summary>
        //public int UserId { get; set; } = 0;

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
