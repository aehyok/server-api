using System;
using System.Collections.Generic;
using System.Text;
using DVS.Common.Models;
using DVS.Core.Domains.Common;
using Newtonsoft.Json;

namespace DVS.Core.Domains.Cons
{
    /// <summary>
    /// 便民服务
    /// </summary>
    public class ConsInfoPublic : DvsEntityBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string MessageName { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string MessageText { get; set; }

        /// <summary>
        /// 类型，1三务公开、2党建宣传、3精神文明、4便民服务
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 主题图片
        /// </summary>
        public string Images { get; set; }

        /// <summary>
        /// 1有效（启用），0无效（禁用）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 浏览次数
        /// </summary>
        public int Viewcnt { get; set; }

        /// <summary>
        /// 创建单位
        /// </summary>
        public int CreatedByDeptId { get; set; }


        /// <summary>
        /// 创建区域
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 置顶截止时间
        /// </summary>
        public DateTime PinTopExpireAt { get; set; }
    }
}
