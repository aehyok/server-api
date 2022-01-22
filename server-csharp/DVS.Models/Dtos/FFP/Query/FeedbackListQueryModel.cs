using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP.Query
{
    /// <summary>
    /// 处理任务列表查询body
    /// </summary>
    public class FeedbackListQueryModel : PagedListQueryModel
    {


        /// <summary>
        /// 区域网格id
        /// </summary>
        // public int MatrixId { get; set; } = 0;

        /// <summary>
        /// 状态0 待审核 1待反馈2已反馈3待确认4已确认
        /// </summary>
        public int[] StatusList { get; set; } = new int[] { };


    }
}
