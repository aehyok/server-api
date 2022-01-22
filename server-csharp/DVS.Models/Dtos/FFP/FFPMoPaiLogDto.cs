using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP
{
    /// <summary>
    /// 摸排记录表
    /// </summary>
    public class FFPMoPaiLogDto
    {
        /// <summary>
        /// 网格id
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// 户码id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 网格id
        /// </summary>
        public int MatrixId { get; set; } = 0;

        /// <summary>
        /// 是否存在返贫风险 1是2否
        /// </summary>
        public int ExistRisk { get; set; }

        /// <summary>
        /// 摸排日期
        /// </summary>
        public DateTime MoPaiDate { get; set; } 

        /// <summary>
        /// 图片 多个用逗号分割
        /// </summary>
        public string Images { get; set; } = "";

        /// <summary>
        /// 语音链接地址  多个用逗号分割
        /// </summary>
        public string VoiceUrl { get; set; } = "";

        /// <summary>
        /// 图片文件
        /// </summary>
        public List<SunFileInfoDto> ImageFiles { get; set; } = new List<SunFileInfoDto>();

        /// <summary>
        /// 回复图标文件
        /// </summary>
        public List<SunFileInfoDto> VoiceFiles { get; set; } = new List<SunFileInfoDto>();

        /// <summary>
        /// 描述信息
        /// </summary>
        public string Describe { get; set; } = "";

        /// <summary>
        /// 创建人id
        /// </summary>
        public int CreatedBy { get; set; } = 0;

        /// <summary>
        /// 修改人id
        /// </summary>
        public int UpdatedBy { get; set; } = 0;

        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDeleted { get; set; } = 0;

        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string CreatedByName { get; set; }

        /// <summary>
        /// 修改人姓名
        /// </summary>
        public string UpdatedByName { get; set; }
    }
}
