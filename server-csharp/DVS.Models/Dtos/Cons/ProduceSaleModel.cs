using System;
using System.Collections.Generic;
using System.Text;
using DVS.Models.Dtos.Common;
using DVS.Models.Dtos.Village;

namespace DVS.Models.Dtos.Cons
{
    public class ListProduceSaleModel : CreateProduceSaleModel
    {
        /// <summary>
        /// 图片URL
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 创建部门名称
        /// </summary>
        public string createdByDeptName { get; set; }

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

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreatedByName { get; set; }

        /// <summary>
        /// 修改人名称
        /// </summary>
        public string UpdatedByName { get; set; }

        /// <summary>
        /// 农产品描述
        /// </summary>
        public string CategoryDetail { get; set; }

        /// <summary>
        /// 创建用户类型 1:公众, 2:村委, 3:政务, 4:企业
        /// </summary>
        public int CreatedUserType { get; set; }

        /// <summary>
        /// 农产品图片
        /// </summary>
        public List<SunFileInfoDto> ImageFiles { get; set; }

        /// <summary>
        /// 是否本人发布
        /// </summary>
        public bool IsSelf { get; set; } = false;
    }

    public class CreateProduceSaleModel
    {
        public int Id { get; set; }

        /// <summary>
        /// 农产品id
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 农产品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Number { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; } = "";

        /// <summary>
        /// 截止日期
        /// </summary>
        public DateTime ExpDate { get; set; }

        /// <summary>
        /// 发布人id
        /// </summary>
        public int PublishId { get; set; }

        /// <summary>
        /// 发布人员
        /// </summary>
        public string Publisher { get; set; } = "";


        /// <summary>
        /// 创建人ID
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// 创建部门ID
        /// </summary>
        public int CreatedByDeptId { get; set; }

        /// <summary>
        /// 浏览次数
        /// </summary>
        public int Viewcnt { get; set; }

        /// <summary>
        /// 修改人ID
        /// </summary>
        public int UpdatedBy { get; set; }

        /// <summary>
        /// 园区id
        /// </summary>
        public int ParkAreaId { get; set; }

        /// <summary>
        /// 创建用户类型 1:公众, 2:村委, 3:政务, 4:企业
        /// </summary>
        public int CreatedUserType { get; set; }
    }

    public class EditProduceSaleModel
    {

    }

    public class DetailProduceSaleModel : ListProduceSaleModel
    {
        public VillagePopulationDto PublishInfo { get; set; }

    }

    public class PublisherModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

    }
}
