using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village
{
    /// <summary>
    /// 用户认证状态
    /// </summary>
    public class UserAuthStatusDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public string DepartmentId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 审核状态 0未申请， 1待审核，2审核通过，3审核失败
        /// </summary>
        public DVS.Models.Enum.UserAuthAuditStatusEnum AuditStatus { get; set; }
        /// <summary>
        /// 人口Id
        /// </summary>
        public int PopulationId { get; set; }
        /// <summary>
        /// 行政区域Id
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 行政区域编码
        /// </summary>
        public long AreaCode { get; set; }
        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; }


        /// <summary>
        /// 行政区域名称
        /// </summary>
        public string AreaName { get; set; } = "";

        /// <summary>
        /// 门牌名
        /// </summary>
        public string HouseName { get; set; } = "";
        /// <summary>
        /// 门牌号
        /// </summary>
        public string HouseNumber { get; set; } = "";

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; } = "";

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdCard { get; set; }
    }
}
