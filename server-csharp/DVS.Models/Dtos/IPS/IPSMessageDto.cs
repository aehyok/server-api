using System;

namespace DVS.Models.Dtos.IPS
{
    /// <summary>
    /// 滚动消息日程表
    /// </summary>
    public class IPSMessageDto
    {
        /// <summary>
        /// 唯一id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 是否24小时全天播放   0 否 1 是
        /// </summary>
        public int AllDay { get; set; }

        /// <summary>
        /// 永不过期  0 否 1 是
        /// </summary>
        public int NeverExpire { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 设备数
        /// </summary>
        public int DeviceCount { get; set; }

        /// <summary>
        ///  状态 1有效（启用），0无效（禁用）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 审核状态  1待审核，2审核中，3审核通过，4拒绝
        /// </summary>
        public int AuditStatus { get; set; }

        /// <summary>
        /// 设备编号,逗号分割
        /// </summary>
        public string Devices { get; set; }

        /// <summary>
        /// IPS的id，同步到IPS后返回的唯一id
        /// </summary>
        public string IpsMessageId { get; set; }

        /// <summary>
        /// 是否同步到IPS  0 否 1 是
        /// </summary>
        public int IsSyncToIPS { get; set; }

        /// <summary>
        /// 创建区域
        /// </summary>
        public int AreaId { get; set; }

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
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }
    }
}
