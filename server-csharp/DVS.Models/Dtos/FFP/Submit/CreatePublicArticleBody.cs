using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP.Submit
{
    /// <summary>
    /// 生成公示文章
    /// </summary>
    public class CreatePublicArticleBody
    {
        /// <summary>
        /// workflow Ids
        /// </summary>
        public int[] WorkflowIds { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        public int FlowStatus { get; set; }

        /// <summary>
        /// 1 生产公示文章，2生产公示报告
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// 栏目ID 选择三务公开，便民信息的Id
        /// </summary>
        public string ColumnId { get; set; }
    }
}
