using System;
using System.Collections.Generic;
using System.Text;
using DVS.Models.Dtos.Common;

namespace DVS.Models.Dtos.GIS
{
    /// <summary>
    /// 打点明细
    /// </summary>
    public class GISPlotItemDto
    {
        /// <summary>
        /// 唯一码id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// PlotType对应表的主键id
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";

        /// <summary>
        /// 打点经纬度,json字符串 
        /// </summary>
        public string Point { get; set; } = "";

        /// <summary>
        /// 打点范围经纬度,json字符串
        /// </summary>
        public string PointItems { get; set; } = "";

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
        /// 创建人姓名
        /// </summary>
        public string CreatedByName { get; set; }

        /// <summary>
        /// 修改人姓名
        /// </summary>
        public string UpdatedByName { get; set; }

        /// <summary>
        /// 关联id
        /// </summary>
        public int RelationId { get; set; }

        /// <summary>
        /// 分类 1摄像头，2传感器，3区域地图，4农户标记，5土地标记，6公共设施，7规划用地，8大棚,9自定义
        /// </summary>
        public int PlotType { get; set; }

        ///// <summary>
        ///// 打点类型 1 区域 ,2 园区 
        ///// </summary>
        //public virtual int Category { get; set; }

        /// <summary>
        /// 户码数
        /// </summary>
        public virtual int Cnt { get; set; } = 0;

        /// <summary>
        /// 同步到数据大屏后返回的唯一id'
        /// </summary>
        public string SyncId { get; set; } = "";

    }
}
