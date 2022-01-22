using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Household
{
    /// <summary>
    /// 发送范围查询条件实体
    /// </summary>
    public class WechatMessagePopulationDto
    {
        /// <summary>
        /// 区域编码
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 门牌名Id
        /// </summary>
        public int HouseNameId { get; set; }
        /// <summary>
        /// 门牌名
        /// </summary>
        public string HouseName { get; set; }

        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 门牌号
        /// </summary>
        public string HouseNumber { get; set; }


        /// <summary>
        /// 人口Id
        /// </summary>
        public int PopulationId { get; set; }

        /// <summary>
        /// 接收人员，0所有人，1户主
        /// </summary>
        public int IsHouseholder { get; set; }

        /// <summary>
        /// 出生年月
        /// </summary>
        public DateTime Birthday { get; set; }


        /// <summary>
        /// 性别 1男，2女
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 学历字典编码
        /// </summary>
        public string Education { get; set; }

        /// <summary>
        /// 婚姻字典编码
        /// </summary>
        public string Marital { get; set; }

        /// <summary>
        /// 政治面貌字典编码
        /// </summary>
        public string Political { get; set; }

        /// <summary>
        /// 主要生活来源字典编码
        /// </summary>
        public string Income { get; set; }

        /// <summary>
        /// 人口属性字典编码
        /// </summary>
        public string Relationship { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// OpenId
        /// </summary>
        public string OpenId { get; set; }
    }
}
