using System;
using System.Collections.Generic;
using System.Text;
using DVS.Models.Dtos.Common;

namespace DVS.Models.Dtos.Cons
{
    /// <summary>
    /// 便民服务
    /// </summary>
    public class ConsInfoPublicDto
    {
        public int Id { get; set; }
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
        /// 创建单位
        /// </summary>
        public int CreatedByDeptId { get; set; }

        /// <summary>
        /// 创建区域
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 浏览次数
        /// </summary>
        public int Viewcnt { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 创建人id
        /// </summary>
        public int CreatedBy { get; set; } = 0;

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 修改人id
        /// </summary>
        public int UpdatedBy { get; set; } = 0;

        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDeleted { get; set; } = 0;

        /// <summary>
        /// 发布人部门名称
        /// </summary>
        public string CreatedByDeptName { get; set; }

        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreatedByName { get; set; }

        /// <summary>
        /// 修改人姓名
        /// </summary>
        public string UpdatedByName { get; set; }

        /// <summary>
        /// 主题图片
        /// </summary>
        public List<SunFileInfoDto> ImageFiles { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 置顶截止时间
        /// </summary>
        public DateTime PinTopExpireAt { get; set; }
    }
}
