using System;
using System.Collections.Generic;
using System.Text;
using DVS.Common.Models;
using Newtonsoft.Json;

namespace DVS.Core.Domains.Common
{
    /// <summary>
    /// 部门
    /// </summary>
    public class BasicDepartment : DvsEntityBase
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 父级部门id
        /// </summary>
        public int Pid { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        public int Level { get; set; }


    }
}
