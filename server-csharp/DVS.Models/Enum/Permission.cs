using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Enum
{
    /// <summary>
    /// 权限操作码
    /// </summary>
    public class PermissionCodes
    {
        /// <summary>
        /// 新增
        /// </summary>
        public const int Add = 1;
        /// <summary>
        /// 删除
        /// </summary>
        public const int Remove = 2;
        /// <summary>
        /// 编辑
        /// </summary>
        public const int Edit = 4;
        /// <summary>
        /// 查看
        /// </summary>
        public const int View = 8;
        /// <summary>
        /// 导入
        /// </summary>
        public const int Import = 16;
        /// <summary>
        /// 导出
        /// </summary>
        public const int Export = 32;
        /// <summary>
        /// 重置密码
        /// </summary>
        public const int ResetPassword = 64;
        /// <summary>
        /// 可见
        /// </summary>
        public const int List = 128;

    }
}
