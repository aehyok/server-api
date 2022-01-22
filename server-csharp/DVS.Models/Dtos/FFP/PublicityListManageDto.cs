using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{
    /// <summary>
    /// 公示名单列表实体
    /// </summary>
    public class PublicityListManageDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// VillageHouseholdCode 户码Id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 门牌名
        /// </summary>
        public string HouseName { get; set; }

        /// <summary>
        /// 门牌号
        /// </summary>
        public string HouseNumber { get; set; }

        /// <summary>
        /// 户主
        /// </summary>
        public string HouseholdMan { get; set; }

        /// <summary>
        /// 户主的联系电话
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 人数
        /// </summary>
        public long PeopleCount { get; set; }

        /// <summary>
        /// 户贫困标签(户属性)
        /// </summary>
        public string HouseholdType { get; set; }

        /// <summary>
        /// 核查日期
        /// </summary>
        public DateTime CheckDate { get; set; }

        /// <summary>
        /// 评议类型
        /// </summary>
        public string ReviewType { get; set; }

        /// <summary>
        /// 公示结果
        /// </summary>
        public string PublicityResult { get; set; }


    }

    /// <summary>
    /// 公示名单详情
    /// </summary>
    public class PublicityListDetailDto
    {

        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 户码id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// FFPHouseholdCode 表的ID ,取地址用
        /// </summary>
        public int Hid { get; set; }

        /// <summary>
        /// 社区/村
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 门牌名
        /// </summary>
        public string HouseName { get; set; } = "";

        /// <summary>
        /// 门牌号
        /// </summary>
        public string HouseNumber { get; set; } = "";


        /// <summary>
        /// 户主
        /// </summary>
        public string HouseholdMan { get; set; } = "";

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; } = "";

        /// <summary>
        /// 户贫困标签(户属性)
        /// </summary>
        public string HouseholdType { get; set; } = "";

        /// <summary>
        /// 户主的联系电话
        /// </summary>
        public string Mobile { get; set; } = "";

        /// <summary>
        /// 门牌标签
        /// </summary>
        public string HouseholdTags { get; set; } = "";

        /// <summary>
        /// 人数
        /// </summary>
        public long PeopleCount { get; set; } = 0;

        /// <summary>
        /// 核查日期
        /// </summary>
        public DateTime CheckDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 审核结果
        /// </summary>
        public string AuditResult { get; set; }

        /// <summary>
        /// 审核日期
        /// </summary>
        public DateTime AuditDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 审核人员
        /// </summary>
        public string Auditer { get; set; }
         
        /// <summary>
        /// 审核人电话
        /// </summary>
        public string AuditMobile { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }


    }
}
