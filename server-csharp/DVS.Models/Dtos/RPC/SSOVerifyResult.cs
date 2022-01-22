using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.RPC
{
   public  class SSOVerifyResult
    {
        /// <summary>
        /// 过期时间
        /// </summary>
        public int ExpiresAt { get; set; }
        /// <summary>
        /// token有效的模块
        /// </summary>
        public List<string> Module { get; set; }

        public long CreateAt { get; set; }
        /// <summary>
        /// 账号（openId)
        /// </summary>
        public string Account { get; set; }
        public string ClientIp { get; set; }
        public PermissionInfo Permissions { get; set; }
        public int Uid { get; set; }
    }

    public class PermissionInfo {
        public List<Permission> Console { get; set; }
        public List<Permission> App { get; set; }
    }

    public class SSOVerifyWithMaxPermissionResult {
        public SSOVerifyResult MaxPermissions { get; set; }
    }
}
