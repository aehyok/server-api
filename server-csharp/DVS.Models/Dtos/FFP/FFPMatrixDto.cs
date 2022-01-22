using System;
using System.Collections.Generic;
using System.Text;
using DVS.Models.Dtos.Common;

namespace DVS.Models.Dtos.FFP
{
    /// <summary>
    /// 网格表
    /// </summary>
    public class FFPMatrixDto
    {
        /// <summary>
        /// 网格id
        /// </summary>
        public int Id { get; set; } = 0;

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
        /// 网格员
        /// </summary>
        public string InspectorName { get; set; }

        /// <summary>
        /// 网格长
        /// </summary>
        public string InspectorManagerName { get; set; }

        /// <summary>
        /// 创建人id
        /// </summary>
        public int CreatedBy { get; set; } = 0;

        /// <summary>
        /// 修改人id
        /// </summary>
        public int UpdatedBy { get; set; } = 0;

        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDeleted { get; set; } = 0;

        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreatedByName { get; set; }

        /// <summary>
        /// 修改人姓名
        /// </summary>
        public string UpdatedByName { get; set; }

    }
}
