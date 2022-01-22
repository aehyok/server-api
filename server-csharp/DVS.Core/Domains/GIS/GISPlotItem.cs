using System;
using System.Collections.Generic;
using System.Text;
using DVS.Common.Models;

namespace DVS.Core.Domains.GIS
{
    /// <summary>
    /// 打点记录表
    /// </summary>
    public class GISPlotItem : DvsEntityBase
    {
        /// <summary>
        /// 分类 1摄像头，2传感器，3区域地图，4农户标记，5土地标记，6公共设施，7规划用地，8大棚, 9自定义
        /// </summary>
        public int PlotType { get; set; }

        /// <summary>
        /// PlotType对应表的唯一id
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// 标绘点名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 打点经纬度,json字符串 
        /// </summary>
        public string Point { get; set; }

        /// <summary>
        /// 打点范围经纬度,json字符串
        /// </summary>
        public string PointItems { get; set; }

        /// <summary>
        /// 同步到数据大屏后返回的唯一id'
        /// </summary>
        public string SyncId { get; set; } = "";

        /// <summary>
        /// 同步操作后返回的description
        /// </summary>
        public string SyncRes { get; set; } = "";

        /// <summary>
        /// 是否已同步, 0 否 1 是
        /// </summary>
        public int IsSync { get; set; } = 0;

        /// <summary>
        /// 同步操作时间
        /// </summary>
        public DateTime SyncDate { get; set; }

        /// <summary>
        ///关联id，同一个点有多个户码，楼栋户码打点用
        /// </summary>
        public int RelationId { get; set; }


        /// <summary>
        ///关联id，同一个点有多个户码，楼栋户码打点用
        /// </summary>
        public virtual List<ObjectItem> ObjectIds { get; set; }
    }

    public class ObjectItem {
        public int ObjectId { get; set; }
        public string Name { get; set; }
    }
}
