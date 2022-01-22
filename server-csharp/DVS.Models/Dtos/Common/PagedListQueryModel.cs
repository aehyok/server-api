using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Common
{
    /// <summary>
    /// 分页基础
    /// </summary>
    public class PagedListQueryModel
    {
        /// <summary>
        /// 分页
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 分页大小
        /// </summary>
        public int Limit { get; set; } = 10;

        /// <summary>
        /// 查询关键字
        /// </summary>
        public string Keyword { get; set; }


        /// <summary>
        /// 排序分组
        /// </summary>
        public List<OrderBy> Orders { get; set; } = null;


        /// <summary>
        /// 按X排序 即将弃用
        /// </summary>
        public string Orderby { get; set; } = " CreatedAt ";


        /// <summary>
        /// 降序  即将弃用
        /// </summary>
        public bool Desc { get; set; } = true;


    }
}
