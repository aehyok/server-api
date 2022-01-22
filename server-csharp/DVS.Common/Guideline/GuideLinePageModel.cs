using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Common.Guideline
{
    public class GuideLinePageModel 
    {
        /// <summary>
        /// 数据总数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int Pages { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int Limit { get; set; }

        public dynamic Docs { get; set; }
    }
}
