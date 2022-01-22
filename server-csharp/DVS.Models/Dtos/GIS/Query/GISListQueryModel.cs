using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.GIS.Query
{
    /// <summary>
    /// 查询条件
    /// </summary>
    public class GISListQueryModel : PagedListQueryModel
    {
        /// <summary>
        /// 分类 1摄像头，2传感器，3区域地图，4农户标记，5农田标记，6公共设施，7规划用地，8大棚, 9自定义
        /// </summary>
        public int PlotType { get; set; }

        /// <summary>
        /// 打点类型 1 区域 ,2 园区 
        /// </summary>
        public int Category { get; set; }

        /// <summary>
        /// 区域id
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 类型id
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 地块用途 1 普通用地 ,2 规划用地
        /// </summary>
        public int UseFor { get; set; }

        /// <summary>
        /// 区域id，户码id，摄像头id，土地标记id，公共设施id，规划用地id，大棚id, 自定义id
        /// </summary>
        public int ObjectId { get; set; }

        /// <summary>
        /// 打点服务记录id，多个id用逗号隔开
        /// </summary>
        public string ObjectIds { get; set; }

        /// <summary>
        ///关联id，同一个点有多个户码，楼栋户码打点用
        /// </summary>
        public int RelationId { get; set; }

        /// <summary>
        /// 门牌名称
        /// </summary>
        public string HouseName { get; set; }

        /// <summary>
        /// 唯一id，多个id用逗号隔开
        /// </summary>
        public string Ids { get; set; }

    }
}
