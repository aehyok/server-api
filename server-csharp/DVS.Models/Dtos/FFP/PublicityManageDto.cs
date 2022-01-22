using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{
    /// <summary>
    /// 评议公示管理列表
    /// </summary>
    public class PublicityManageDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 生成日期
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreatedUser { get; set; }

        /// <summary>
        /// 发布状态
        /// </summary>
        public string PublishStatus { get; set; }

        /// <summary>
        /// 发布日期
        /// </summary>
        public DateTime PublishDate { get; set; }

        /// <summary>
        /// 发布人员
        /// </summary>
        public string Publisher { get; set; }


    }

    public class PublicityManageAddDto
    {
        /// <summary>
        /// 类型 1评议公示2评议报告
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 发布类型，1三务公开、2党建宣传、3精神文明、4便民服务
        /// </summary>
        public int PublishType { get; set; }

        /// <summary>
        /// 户码ID
        /// </summary>
        public List<int> Ids { get; set; }
    }

    public class PublicityHouseholdList
    {
        /// <summary>
        /// 
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// AreaId
        /// </summary>
        public int AreaId { get; set; }



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
        /// 户主的联系电话
        /// </summary>
        public string Mobile { get; set; } = "";

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 人数
        /// </summary>
        public long PeopleCount { get; set; } = 0;



        /// <summary>
        /// 户贫困标签(户属性)
        /// </summary>
        public string HouseholdType { get; set; } = "";

        /// <summary>
        /// 核查日期
        /// </summary>
        public DateTime CheckDate { get; set; }


    }
}
