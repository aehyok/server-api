using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Enum
{
    /// <summary>
    /// 打点分类
    /// </summary>
    public enum PlotType
    {
        /// <summary>
        /// 未知
        /// </summary>
        UNKNOWN = 0,
        /// <summary>
        /// 1摄像头
        /// </summary>
        CAMERA = 1,
        /// <summary>
        /// 2传感器
        /// </summary>
        SENSOR = 2,
        /// <summary>
        /// 3区域地图
        /// </summary>
        AREA = 3,
        /// <summary>
        /// 4农户标记
        /// </summary>
        HOUSEHOLD = 4,
        /// <summary>
        /// 5土地标记
        /// </summary>
        FARMLAND = 5,
        /// <summary>
        /// 6公共设施
        /// </summary>
        COLLECTIVEPROPERTY = 6,
        /// <summary>
        /// 7规划用地
        /// </summary>
        PLANLAND = 7,
        /// <summary>
        /// 8大棚
        /// </summary>
        GREENHOUSE = 8,
        /// <summary>
        /// 9自定义
        /// </summary>
        CUSTOM = 9
    }
}
