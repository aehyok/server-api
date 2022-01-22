using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{
    /// <summary>
    /// 评议公示详情实体
    /// </summary>
    public class PublicityManageDetailDto
    {
        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 发布日期
        /// </summary>
        public DateTime PublishDate { get; set; }


        /// <summary>
        /// 公示名单列表
        /// </summary>
        public List<PublicityList> PublicityList { get; set; }

        /// <summary>
        /// 消除风险列表
        /// </summary>
        public List<EliminateRiskList> EliminateRiskList { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class PublicityList
    {
        /// <summary>
        /// 门牌名
        /// </summary>
        public string HouseName { get; set; }

        /// <summary>
        /// 户主
        /// </summary>
        public string HouseholdMan { get; set; }

        /// <summary>
        /// 性别 1男，2女
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 与户主关系
        /// </summary>
        public string Relationship { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// 户主的联系电话
        /// </summary>
        public string Mobile { get; set; }


        /// <summary>
        /// 是否监测对象  1是2否
        /// </summary>
        public string MonitorObj { get; set; }

        /// <summary>
        /// 户贫困标签(户属性)
        /// </summary>
        public string HouseholdType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class EliminateRiskList
    {
        /// <summary>
        /// 门牌名
        /// </summary>
        public string HouseName { get; set; }

        /// <summary>
        /// 户主
        /// </summary>
        public string HouseholdMan { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdCard { get; set; }


        /// <summary>
        /// 人数
        /// </summary>
        public long PeopleCount { get; set; }

        /// <summary>
        /// 风险消除类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }

    /// <summary>
    /// 评议报告管理详情
    /// </summary>
    public class ReviewReportManageDetailDto
    {
        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 报告日期
        /// </summary>
        public DateTime ReportDate { get; set; }


        /// <summary>
        /// 报告列表
        /// </summary>
        public List<PublicityList> ReportList { get; set; }

        /// <summary>
        /// 消除风险列表
        /// </summary>
        public List<EliminateRiskList> EliminateRiskList { get; set; }

    }

    
 

}
