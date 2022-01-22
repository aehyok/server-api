using System;
using System.Collections.Generic;

namespace DVS.Common.ModelDtos
{
    [Serializable]
    public class ResultModel<T>
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public long Timestamp { get; set; }

        public T Data { get; set; }
    }

    [Serializable]
    public class PagedResultModel<T>
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

        public IEnumerable<T> Docs { get; set; }
    }

    [Serializable]
    public class ResultIPSModel<T>
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public long Timestamp { get; set; }

        public PagedResultIPSModel<T> Data { get; set; }
    }

    [Serializable]
    public class PagedResultIPSModel<T>
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

        public IEnumerable<T> Docs { get; set; }
    }
}