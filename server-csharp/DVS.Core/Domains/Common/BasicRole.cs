using DVS.Common.Models;

namespace DVS.Core.Domains.Common
{
    /// <summary>
    /// 用户角色
    /// </summary>
    public class BasicRole : DvsEntityBase
    {

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        public int Sequence { get; set; }


        /// <summary>
        /// 数据权限all,group,self
        /// </summary>
        public string DataAccess { get; set; }

        /// <summary>
        /// 用户类型 公众1 村委2 政务3 企业4
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 状态 1有效（启用），0无效（禁用）
        /// </summary>
        public int Status { get; set; }

    }
}
