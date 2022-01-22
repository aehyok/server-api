using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Common
{
    public class Permission
    {
        public string ModuleCode { get; set; }
        public int OwnPermission { get; set; }
        public int MaxPermission { get; set; } = 0;
    }
}
