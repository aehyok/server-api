using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village
{
    public class VillageWorkInfoDto
    {
        /// <summary>
        /// 记录Id
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// 行政代码Id
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 工作所在地行政代码
        /// </summary>
        public string WorkOrgCodes { get; set; }

        /// <summary>
        /// 工作所在地详细地址
        /// </summary>
        public string WorkAddress { get; set; }

        /// <summary>
        /// 人口Id
        /// </summary>
        public int PopulationId { get; set; }

        /// <summary>
        /// 年度
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 行业
        /// </summary>
        public string Industry { get; set; }

        /// <summary>
        /// 行业对象
        /// </summary>
        public BasicDictionaryDto IndustryDto { get; set; }

        /// <summary>
        /// 职业
        /// </summary>
        public string Occupation { get; set; }

        /// <summary>
        /// 职业对象
        /// </summary>
        public BasicDictionaryDto OccupationDto { get; set; }

        /// <summary>
        /// 薪资范围
        /// </summary>
        public string Salary { get; set; }

        /// <summary>
        /// 薪资范围对象
        /// </summary>
        public BasicDictionaryDto SalaryDto { get; set; }

        /// <summary>
        /// 工作单位
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; }


        /// <summary>
        /// 性别 1男，2女,
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 与户主关系
        /// </summary>
        public string Relationship { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 头像路径
        /// </summary>
        public string HeadImageUrl { get; set; }


        /// <summary>
        /// 最后操作时间
        /// </summary>

        public DateTime UpdatedAt { get; set; }


        /// <summary>
        /// 工作地
        /// </summary>
        public PopulationAddressDto WorkAddressInfo { get; set; }
    }
}
