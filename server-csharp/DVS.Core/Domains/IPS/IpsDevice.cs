using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.IPS
{
    /// <summary>
    /// IPS设备表
    /// </summary>
    public class IpsDevice : DvsEntityBase
    {
        /// <summary>
        /// IPS的唯一id
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNo { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }


        /// <summary>
        /// 设备类型 0屏幕,1人脸识别设备
        /// </summary>
        public int Category { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        ///  状态 1有效（启用），0无效（禁用）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 门店Id
        /// </summary>
        public string CompanyId { get; set; }

    }
}
