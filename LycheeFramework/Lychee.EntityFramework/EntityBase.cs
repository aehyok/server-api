using System;

namespace Lychee.EntityFramework
{
    [Serializable]
    public abstract class EntityBase : IEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public object Id { get; set; }
    }
}