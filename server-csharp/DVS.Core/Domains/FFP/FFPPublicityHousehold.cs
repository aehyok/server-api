using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.FFP
{
    /// <summary>
    /// 评议公示的户明细，跟评议公示、评议报告关联
    /// </summary>
    public class FFPPublicityHousehold : DvsEntityBase
    {
        /// <summary>
        /// 评议公示\报告管理ID
        /// </summary>
        public int PublicityManageId { get; set; }

        /// <summary>
        /// 区域ID
        /// </summary>
        public int AreaId { get; set; }

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
        /// 性别 1男，2女
        /// </summary>
        public int Sex { get; set; }

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
        /// 是否监测对象  1是2否
        /// </summary>
        public int MonitorObj { get; set; }

        /// <summary>
        /// 公示结果
        /// </summary>
        public string PublicityResult { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
