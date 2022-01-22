using Lychee.EntityFramework;
using System;

namespace DVS.Common.Models
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class DvsEntityBase : EntityBase
    {
        /// <summary>
        /// 主键
        /// </summary>
        public new int Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 创建人id
        /// </summary>
        public int? CreatedBy { get; set; } = 0;

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 修改人id
        /// </summary>
        public int? UpdatedBy { get; set; } = 0;

        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDeleted { get; set; } = 0;
    }
}