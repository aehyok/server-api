using DVS.Models.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village
{
    public class PopulationDetailDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 行政代码Id
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 是否是户主 1是，0不是
        /// </summary>
        public int IsHouseholder { get; set; } = 0;

        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 性别 1男，2女
        /// </summary>
        public PopulationGender Sex { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        public string Nation { get; set; } = "";

        /// <summary>
        /// 出生年月日 1990-04-02
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// 与户主关系
        /// </summary>
        public string Relationship { get; set; } = "";

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdCard { get; set; } = "";

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Mobile { get; set; } = "";

       

        /// <summary>
        /// 政治面貌
        /// </summary>
        public string Political { get; set; } = "";

        /// <summary>
        /// 学历
        /// </summary>
        public string Education { get; set; } = "";

        /// <summary>
        /// 婚姻状态
        /// </summary>
        public string Marital { get; set; } = "";

        /// <summary>
        /// 宗教
        /// </summary>
        public string Religion { get; set; } = "";

        /// <summary>
        /// 收入来源
        /// </summary>
        public string Income { get; set; } = "";

        /// <summary>
        /// 头像Id
        /// </summary>
        public string HeadImageId { get; set; } = "";

        /// <summary>
        /// 头像图片路径
        /// </summary>
        public string HeadImageUrl { get; set; } = "";

        /// <summary>
        /// 标签数组
        /// </summary>
        public string Tags { get; set; } = "";

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";

        /// <summary>
        /// 1 启用 0停用
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 移除原因
        /// </summary>
        public string DeleteReason { get; set; } = "";

        /// <summary>
        /// 门牌名
        /// </summary>
        public string HouseName { get; set; } = "";
        /// <summary>
        /// 门牌号
        /// </summary>
        public string HouseNumber { get; set; } = "";

        /// <summary>
        /// 户籍地
        /// </summary>
        public PopulationAddressDto RegisterAddressInfo { get; set; }
        /// <summary>
        /// 居住地
        /// </summary>
        public PopulationAddressDto LiveAddressInfo { get; set; }
        /// <summary>
        /// 籍贯
        /// </summary>
        public PopulationAddressDto NativePlaceInfo { get; set; }

        /// <summary>
        /// 标签名称列表 添加或编辑时不需要传入
        /// </summary>
        public IEnumerable<VillageTagDto> TagNames { get; set; } = new List<VillageTagDto>();

        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDeleted { get; set; } = 0;

        /// <summary>
        /// 同步到数据大屏后返回的唯一id'
        /// </summary>
        public string SyncId { get; set; } = "";

        /// <summary>
        /// 同步操作后返回的description
        /// </summary>
        public string SyncRes { get; set; } = "";

        /// <summary>
        /// 是否已同步, 0 否 1 是
        /// </summary>
        public int IsSync { get; set; } = 0;

        /// <summary>
        /// 同步操作时间
        /// </summary>
        public DateTime SyncDate { get; set; }
    }

    
}
