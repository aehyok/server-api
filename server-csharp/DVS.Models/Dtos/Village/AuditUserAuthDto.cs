using System;
using System.Collections.Generic;
using System.Text;
using DVS.Models.Enum;

namespace DVS.Models.Dtos.Village
{
    public class AuditUserAuthDto
    {
        /// <summary>
        /// 户码Id
        /// </summary>
        public int Id { get; set; } = 0;
        /// <summary>
        /// 行政区域Id
        /// </summary>
        public int AreaId { get; set; } = 0;
        /// <summary>
        /// 审核状态 2审核通过，3审核失败
        /// </summary>
        public UserAuthAuditStatusEnum AuditStatus { get; set; } = UserAuthAuditStatusEnum.Passed;
        /// <summary>
        /// 审核备注
        /// </summary>
        public string AuditRemark { get; set; } = "";
        /// <summary>
        /// 门牌名
        /// </summary>
        public string HouseName { get; set; } = "";
        /// <summary>
        /// 门牌号
        /// </summary>
        public string HouseNumber { get; set; } = "";

        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; } = 0;
    }
}
