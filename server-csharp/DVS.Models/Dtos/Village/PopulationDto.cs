using DVS.Models.Dtos.Common;
using DVS.Models.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village
{
    public class PopulationDto
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
        public long IsHouseholder { get; set; } = 0;



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
        public string NationCode { get; set; } = "";



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
        /// 学历
        /// </summary>
        public string Education { get; set; } = "";




        /// <summary>
        /// 头像图片路径
        /// </summary>
        public string HeadImageUrl { get; set; } = "";

        /// <summary>
        /// 户籍地详细地址
        /// </summary>
        public string RegisterAddress { get; set; } = "";

        /// <summary>
        /// 现居住地详细地址
        /// </summary>
        public string LiveAddress { get; set; } = "";

        /// <summary>
        /// 标签名称列表 添加或编辑时不需要传入
        /// </summary>
        public IEnumerable<VillageTagDto> TagNames { get; set; } = new List<VillageTagDto>();

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public BasicDictionaryDto NationDto { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public BasicDictionaryDto EducationDto { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public BasicDictionaryDto RelationshipDto { get; set; }

     
    }                            
}
