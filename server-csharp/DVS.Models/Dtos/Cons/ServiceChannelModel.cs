using System;
using System.Collections.Generic;
using System.Text;
using DVS.Models.Dtos.Common;

namespace DVS.Models.Dtos.Cons
{
    public class ServiceChannelModel
    {
    }


    public class CreateServiceChannelModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 介绍
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 补充说明
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// URL地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public int UpdatedBy { get; set; }
    }

    public class ListServiceChannelModel : CreateServiceChannelModel
    {
        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreatedAt { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public string UpdatedAt { get; set; }

        public string CreatedByName { get; set; }

        public string UpdatedByName { get; set; }

        /// <summary>
        /// 图标文件
        /// </summary>
        public List<SunFileInfoDto> IconFiles { get; set; }

    }

    public class EditServiceChannelModel
    {

    }

    public class DetailServiceChannelModel : ListServiceChannelModel
    {
        /// <summary>
        /// 二维码
        /// </summary>
        public string QrCode { get; set; }
    }
}
