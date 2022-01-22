using DVS.Models.Const;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DVS.Village.Api.Controllers.Console
{
    /// <summary>
    /// 系统基础接口
    /// </summary>
    [Route("api/village/console/system")]
    public class SystemController
    {
        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <returns></returns>
        [HttpGet("version")]
        public string Version() {
            if (System.IO.File.Exists(".version"))
            {
                string versionText = System.IO.File.ReadAllText(".version");
                if (!versionText.IsNullOrWhiteSpace())
                {
                    string[] parts = versionText.Split("=");
                    if (parts.Length > 1)
                    {
                        return parts[1];
                    }
                }
            }
            return "1.0.0.001";
        }
    }
}
