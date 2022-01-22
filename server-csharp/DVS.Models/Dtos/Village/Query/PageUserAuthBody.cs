using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;
using DVS.Models.Enum;

namespace DVS.Models.Dtos.Village.Query
{
    public class PageUserAuthBody: PagedListQueryModel
    {
       
        /// <summary>
        /// 行政区域Id
        /// </summary>
        public int AreaId { get; set; }



        /// <summary>
        /// 审核  // 1待审核，2审核通过
        /// </summary>
        public UserAuthAuditStatusEnum AuditStatus { get; set; }
    }
}
