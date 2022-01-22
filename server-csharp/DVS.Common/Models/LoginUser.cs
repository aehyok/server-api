using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Common.Models
{
    /// <summary>
    /// 登录用户的信息
    /// </summary>
    public class LoginUser
    {
        /// <summary>
        /// 用户账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        public int AreaId { get; set; }

        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 户籍地详细地址
        /// </summary>
        public string RegisterAddress { get; set; }

        /// <summary>
        /// 现居住地详细地址
        /// </summary>
        public string LiveAddress { get; set; }

        /// <summary>
        /// 1 启用 0停用
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 是否是户主，1是，0否
        /// </summary>
        public int IsHouseholder { get; set; }

        /// <summary>
        /// 部门id
        /// </summary>
        public string DepartmentIds { get; set; }

        /// <summary>
        /// 角色id
        /// </summary>
        public string RoleIds { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }

        public Dictionary<string, List<Permission>> Permissions { get; set; } = new Dictionary<string, List<Permission>>();
        public List<int> AreaIds { get; set; }

        /// <summary>
        /// 户籍Id
        /// </summary>
        public int PopulationId { get; set; }

        /// <summary>
        /// 数据处理权限 all 全部 group 本部门 self 本人
        /// </summary>
        public string DataAcces { get; set; }
        /// <summary>
        /// 0非网格员 1网格员，2网格长
        /// </summary>
        public int IsGrid { get; set; }

    }


}
