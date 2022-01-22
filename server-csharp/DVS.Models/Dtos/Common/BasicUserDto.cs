using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Common
{
    public class BasicUserDto
    {
        public int Id { get; set; }

        /// <summary>
        /// 行政代码Id
        /// </summary>
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
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 公众用户是否已认证
        /// </summary>
        public DVS.Models.Enum.UserAuthAuditStatusEnum IsAuth { get; set; }

        /// <summary>
        /// 数据处理权限 all 全部 group 本部门 self 本人
        /// </summary>
        public string DataAcces { get; set; }

        /// <summary>
        /// 用户类型 公众1 村委2 政务3 企业4
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 用户所属区域的本级及所有下级区域
        /// </summary>
        public List<int> AreaIds { get; set; }

        /// <summary>
        /// 0非网格员 1网格员，2网格长
        /// </summary>
        public int IsGrid { get; set; }
    }
}
