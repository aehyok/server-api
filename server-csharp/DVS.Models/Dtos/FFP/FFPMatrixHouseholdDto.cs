using System;
using System.Collections.Generic;
using System.Text;
using DVS.Models.Dtos.Common;

namespace DVS.Models.Dtos.FFP
{
    /// <summary>
    /// 网格户码表
    /// </summary>
    public class FFPMatrixHouseholdDto
    {
        /// <summary>
        /// 网格户码表id
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// 网格id
        /// </summary>
        public int MatrixId { get; set; } = 0;

        /// <summary>
        /// 户码id
        /// </summary>
        public int HouseholdId { get; set; } = 0;

        /// <summary>
        /// 行政区域Id
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 行政区域名称
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 上级行政区域名称
        /// </summary>
        public string ParentAreaName { get; set; }

        /// <summary>
        /// 门牌名
        /// </summary>
        public string HouseName { get; set; }

        /// <summary>
        /// 门牌号
        /// </summary>
        public string HouseNumber { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public long Sex { get; set; } = 1;

        /// <summary>
        /// 人数
        /// </summary>
        public long PeopleCount { get; set; }

        /// <summary>
        /// 户主
        /// </summary>
        public string HouseholdMan { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Mobile { get; set; }

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

        /// <summary>
        /// 是否添加到了网格，0 未添加， 1 已经添加, -1 所有
        /// </summary>
        public int IsUsed { get; set; } = 0;

        /// <summary>
        /// 网格员
        /// </summary>
        public string Inspector { get; set; }

        /// <summary>
        /// 网格长
        /// </summary>
        public string InspectorManager { get; set; }

        /// <summary>
        /// 网格员
        /// </summary>
        public string InspectorName { get; set; }

        /// <summary>
        /// 网格长
        /// </summary>
        public string InspectorManagerName { get; set; }

        /// <summary>
        /// 户码属性
        /// </summary>
        public string HouseholdType { get; set; }

        /// <summary>
        /// 户码属性对象
        /// </summary>
        public ModuleDictionaryDto HouseholdTypeDto { get; set; }

        /// <summary>
        /// 摸排记录
        /// </summary>
        public List<FFPMoPaiLogDto> FFPMoPaiLogs { get; set; } = new List<FFPMoPaiLogDto>();

        /// <summary>
        /// 工作流
        /// </summary>
        public List<FFPWorkflowDto> FFPWorkflows { get; set; } = new List<FFPWorkflowDto>();
    }
}
