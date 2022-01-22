using System;
using System.Collections.Generic;
using System.Text;
using DVS.Common.Models;
using Newtonsoft.Json;

namespace DVS.Core.Domains.Common
{
    /// <summary>
    /// 类目信息
    /// </summary>
    public class BasicCategory : DvsEntityBase
    {
        /// <summary>
        /// 上级类目id
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 类目名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 类目详细描述
        /// </summary>
        public string CategoryDetail { get; set; }

        /// <summary>
        /// 类目图片文件id
        /// </summary>
        public int CategoryPicId { get; set; }

        /// <summary>
        /// 类目图片相对地址
        /// </summary>
        public string CategoryPicUrl { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// 发布人id
        /// </summary>
        ///public virtual int CreatedUserId { get; set; }

    }
}
