using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Household
{
    /// <summary>
    /// 发送范围查询条件实体
    /// </summary>
    public class WechatMessagePopulationQuery
    {
        /// <summary>
        /// 区域编码
        /// </summary>
        public int[] AreaId { get; set; }

        /// <summary>
        /// 门牌名Id
        /// </summary>
        public int[] HouseNameId { get; set; }


        /// <summary>
        /// 接收人员，0所有人，1户主
        /// </summary>
        public int IsHouseholder { get; set; }

        /// <summary>
        /// 年龄段，开始年龄
        /// </summary>
        public int BeginAge { get; set; }


        /// <summary>
        /// 年龄段，结束年龄
        /// </summary>
        public int EndAge { get; set; }


        /// <summary>
        /// 性别 1男，2女
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 学历字典编码
        /// </summary>
        public string[] Education { get; set; }

        /// <summary>
        /// 婚姻字典编码
        /// </summary>
        public string[] Marital { get; set; }

        /// <summary>
        /// 政治面貌字典编码
        /// </summary>
        public string[] Political { get; set; }

        /// <summary>
        /// 主要生活来源字典编码
        /// </summary>
        public string[] Income { get; set; }

        /// <summary>
        /// 人口属性字典编码
        /// </summary>
        public string[] Relationship { get; set; }

       
    }
}
