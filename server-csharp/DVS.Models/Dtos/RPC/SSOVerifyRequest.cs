using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.RPC
{
    public class SSOVerifyRequest
    {
        /// <summary>
        /// 需要验证的模块
        /// </summary>
        public string Module { get; set; }
        /// <summary>
        /// Token的值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 延长token的有效期，单位是秒
        /// </summary>
        public int Extend { get; set; }
    }
}
