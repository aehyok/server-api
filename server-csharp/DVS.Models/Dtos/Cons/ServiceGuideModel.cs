using System;
using System.Collections.Generic;
using System.Text;
using DVS.Models.Dtos.Common;

namespace DVS.Models.Dtos.Cons
{
    public class ServiceGuideModel
    {

    }

    public class CreateServiceGuideModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 受理条件
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// 准备材料
        /// </summary>
        public string Material { get; set; }

        /// <summary>
        /// 办理信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 办理流程
        /// </summary>
        public string Step { get; set; }

        /// <summary>
        /// 办理流程图片ID逗号分隔
        /// </summary>
        public string StepImages { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// URL
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

    public class ListServiceGuideModel : CreateServiceGuideModel
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
        /// 办理流程图片文件
        /// </summary>
        public List<SunFileInfoDto> StepImageFiles { get; set; }

        /// <summary>
        /// 图标文件
        /// </summary>
        public List<SunFileInfoDto> IconFiles { get; set; }

    }

    public class EditServiceGuideModel
    {

    }

    public class DetailServiceGuideModel : ListServiceGuideModel
    {
        /// <summary>
        /// 二维码
        /// </summary>
        public string QrCode { get; set; }
    }
}
