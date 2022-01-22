using System.Collections.Generic;

namespace DVS.Models.Dtos.IPS
{
    /// <summary>
    ///  IPS组织架构、企业表
    /// </summary>
    public class IPSCompanyDto
    {
        /// <summary>
        /// IPS的唯一id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// IPS的唯一id
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// 企业编号
        /// </summary>
        public string CompanyNo { get; set; }

        /// <summary>
        /// 顶级单位编号
        /// </summary>
        public string TopCompanyId { get; set; }

        /// <summary>
        /// 类别，0商户，1组，2门店
        /// </summary>
        public int Category { get; set; }

        /// <summary>
        /// 企业名
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 简称
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        ///  状态 1有效（启用），0无效（禁用）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
    }

    /// <summary>
    /// IPS组织架构子表
    /// </summary>
    public class IPSCompanyTreeDto: IPSCompanyDto
    {
        /// <summary>
        /// 下级门店
        /// </summary>
        public List<IPSCompanyDto> Children { get; set; } = new List<IPSCompanyDto>();
    }
}
