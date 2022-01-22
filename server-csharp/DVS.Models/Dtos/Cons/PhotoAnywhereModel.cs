using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Cons
{
    public class CreatePhotoAnywhereModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 微信用户ID
        /// </summary>
        public int UserId { get; set; }

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
        /// 创建部门ID
        /// </summary>
        public int CreatedByDeptId { get; set; }

        /// <summary>
        /// 事发地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public int UpdatedBy { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// 户码id
        /// </summary>
        public int HouseholdId { get; set; }

        /// <summary>
        /// 区域id
        /// </summary>
        public int AreaId { get; set; } = 0;

        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; } = "";
    }

    public class ListPhotoAnywhereModel : CreatePhotoAnywhereModel
    {
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 微信头像
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 用户地址
        /// </summary>
        public string UserAddress { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 是否是户主，1是，0否
        /// </summary>
        public int IsHouseholder { get; set; }

        /// <summary>
        /// 是否回复 1 待回复，2 已回复
        /// </summary>
        public int IsReply { get; set; }

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
        public DateTime ReplyDateTime { get; set; }

        /// <summary>
        /// 回复人员
        /// </summary>
        public string Replyer { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        public string CreatedByName { get; set; }

        public string UpdatedByName { get; set; }

        /// <summary>
        /// 图片文件
        /// </summary>
        public List<SunFileInfoDto> ImageFiles { get; set; }

        /// <summary>
        /// 回复图标文件
        /// </summary>
        public List<SunFileInfoDto> ReplyImageFiles { get; set; }

        /// <summary>
        /// 与户主关系
        /// </summary>
        public string Relationship { get; set; }

        /// <summary>
        /// 类型对象
        /// </summary>
        public BasicDictionaryDto TypeDto { get; set; }
    }

    public class EditPhotoAnywhereModel
    {

    }

    public class ReplyPhotoAnywhereModel
    {
        public int Id { get; set; }

        /// <summary>
        /// 回复文字
        /// </summary>
        public string ReplyDesc { get; set; }

        /// <summary>
        /// 回复图片ID逗号分隔
        /// </summary>
        public string ReplyImageIds { get; set; }

    }

    public class DetailPhotoAnywhereModel : ListPhotoAnywhereModel
    {
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
