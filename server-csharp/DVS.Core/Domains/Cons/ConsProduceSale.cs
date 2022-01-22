using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.Cons
{
    /// <summary>
    /// 产品销售实体表
    /// </summary>
    public class ConsProduceSale : DvsEntityBase
    {
        /// <summary>
        /// 产品id
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 农产品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Number { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 截止日期
        /// </summary>
        public DateTime ExpDate { get; set; }

        /// <summary>
        /// 发布人id
        /// </summary>
        public int PublishId { get; set; }

        /// <summary>
        /// 发布人员
        /// </summary>
        public string Publisher { get; set; }


        /// <summary>
        /// 创建部门ID
        /// </summary>
        public int CreatedByDeptId { get; set; }

        /// <summary>
        /// 浏览次数
        /// </summary>
        public int Viewcnt { get; set; }

        /// <summary>
        /// 创建用户类型 1:公众, 2:村委, 3:政务, 4:企业
        /// </summary>
        public int CreatedUserType { get; set; }

        /// <summary>
        /// 园区id
        /// </summary>
        public int ParkAreaId { get; set; }
    }
}
