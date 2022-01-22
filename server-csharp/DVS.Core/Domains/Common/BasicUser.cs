using System;
using System.Collections.Generic;
using System.Text;
using DVS.Common.Models;
using Newtonsoft.Json;

namespace DVS.Core.Domains.Common
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class BasicUser : DvsEntityBase
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 公众用户是否已认证
        /// </summary>
        public DVS.Models.Enum.UserAuthAuditStatusEnum IsAuth { get; set; }

        /// <summary>
        /// 户籍人口表Id
        /// </summary>
        public int PopulationId { get; set; }

        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 用户类型 公众1 村委2 政务3 企业4
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 所属区域
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        public string DepartmentIds { get; set; }

        /// <summary>
        /// 角色id
        /// </summary>
        public string RoleIds { get; set; }


        /// <summary>
        /// 状态 1有效（启用），0无效（禁用）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 用户头像id
        /// </summary>
        public int PortraitFileId { get; set; }

        /// <summary>
        /// 0非网格员 1网格员，2网格长
        /// </summary>
        public int IsGrid { get; set; }
    }
}
