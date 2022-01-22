using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.FFP
{
    /// <summary>
    /// 网格管理
    /// </summary>
    public class FFPMatrix : DvsEntityBase
    {
        /// <summary>
        /// 网格名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 排序序号
        /// </summary>
        public int Sequence { get; set; }

       /// <summary>
       /// 备注
       /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 户码数量
        /// </summary>
        public int HouseCount { get; set; }

        /// <summary>
        /// 网格员
        /// </summary>
        public string Inspector { get; set; }

        /// <summary>
        /// 网格长
        /// </summary>
        public string InspectorManager { get; set; }

        /// <summary>
        /// 网格打点数据，备用
        /// </summary>
        public string PointItems { get; set; } = "";
    }
}
