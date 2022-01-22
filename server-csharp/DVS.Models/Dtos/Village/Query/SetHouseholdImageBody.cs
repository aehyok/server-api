
using System;

namespace DVS.Models.Dtos.Village.Query
{
    /// <summary>
    /// 上传住宅图片
    /// </summary>
    public class SetHouseholdImageBody
    {

        /// <summary>
        /// 户码id
        /// </summary>
        public int HouseholdId { get; set; }
        /// <summary>
        /// 图片id串，用英文逗号隔开
        /// </summary>
        public string ImageIds { get; set; }
        /// <summary>
        /// 图片相对路径，用英文逗号隔开
        /// </summary>
        public string ImageUrls { get; set; }

    }
}

