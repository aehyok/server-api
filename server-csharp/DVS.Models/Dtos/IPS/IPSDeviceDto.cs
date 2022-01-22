using System.Collections.Generic;

namespace DVS.Models.Dtos.IPS
{
    /// <summary>
    /// IPS设备表
    /// </summary>
    public class IPSDeviceDto
    {
        /// <summary>
        /// IPS的唯一id
        /// </summary>
        public string Id { get; set; }

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
        /// 状态 1有效（启用），0无效（禁用）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 门店Id
        /// </summary>
        public string CompanyId { get; set; }

        public bool IsDelete { get; set; }
    }


    /// <summary>
    /// 信发组织树下的设备
    /// </summary>
    public class IPSDeviceTreeDto: IPSCompanyDto
    {
        /// <summary>
        /// 下级门店
        /// </summary>
        public List<IPSCompanyDto> Children { get; set; } = new List<IPSCompanyDto>();

        /// <summary>
        /// 信发组织所属设备
        /// </summary>
        public List<IPSDeviceDto> Devices { get; set; } = new List<IPSDeviceDto>();
    }
}
