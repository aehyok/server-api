using DVS.Common.Models;
using Lychee.EntityFramework;
using System;
using DVS.Models.Enum;

namespace DVS.Core.Domains.Village
{
    /// <summary>
    /// 村民认证记录表
    /// </summary>
    public class VillageUserAuthRecord : DvsEntityBase
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; } = 0;

        /// <summary>
        /// 微信Id
        /// </summary>
        // public string Account { get; set; } = "";

        /// <summary>
        /// 人口Id
        /// </summary>
        public int PopulationId { get; set; } = 0;

        /// <summary>
        /// 行政区域Id
        /// </summary>
        public int AreaId { get; set; } = 0;

        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; } = "";

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdCard { get; set; } = "";

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Mobile { get; set; } = "";

        /// <summary>
        /// 性别 1男，2女
        /// </summary>
        public PopulationGender Sex { get; set; }

        /// <summary>
        /// 出生年月日
        /// </summary>
        public DateTime Birthday { get; set; }

        /// <summary>
        /// 正面照Id
        /// </summary>
        public string ImageId { get; set; } = "";

        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImageUrls { get; set; } = "";

        /// <summary>
        /// 用户备注
        /// </summary>
        public string Remark { get; set; } = "";

        /// <summary>
        /// 审核状态 0未申请， 1待审核，2审核通过，3审核失败
        /// </summary>
        public UserAuthAuditStatusEnum AuditStatus { get; set; }

        /// <summary>
        /// 审核人
        /// </summary>
        public int Auditor { get; set; } = 0;

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditDateTime { get; set; }

        /// <summary>
        /// 审核备注
        /// </summary>
        public string AuditRemark { get; set; } = "";


        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; } = 0;


    }
}

