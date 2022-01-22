using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.Cons
{
    /// <summary>
    /// 随后拍实体表
    /// </summary>
    public class ConsPhotoAnywhere : DvsEntityBase
    {
        /// <summary>
        /// 微信用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 户码Id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 类型 垃圾处理、环境污染、饮水问题、住房问题、治安问题、违规违纪、看病问题、教育问题、交通问题、其他
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 文字描述
        /// </summary>
        public string Descript { get; set; }

        /// <summary>
        /// 图片ID逗号分隔
        /// </summary>
        public string ImageIds { get; set; }

        /// <summary>
        /// 视频ID逗号分隔
        /// </summary>
        public string VideoIds { get; set; }

        /// <summary>
        /// 是否回复 1 待回复，2 已回复
        /// </summary>
        public int IsReply { get; set; } = 1;

        /// <summary>
        /// 回复文字
        /// </summary>
        public string ReplyDesc { get; set; }

        /// <summary>
        /// 回复图片ID逗号分隔
        /// </summary>
        public string ReplyImageIds { get; set; }

        /// <summary>
        /// 回复时间
        /// </summary>
        public DateTime ReplyDateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 回复人员
        /// </summary>
        public string Replyer { get; set; }


        /// <summary>
        /// 创建部门ID
        /// </summary>
        public int CreatedByDeptId { get; set; }

        /// <summary>
        /// 事发地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public decimal Latitude { get; set; }

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

        /// <summary>
        /// 区域id
        /// </summary>
        public int AreaId { get; set; } = 0;
    }
}
