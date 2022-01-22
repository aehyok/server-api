using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.Cons
{
    /// <summary>
    /// 办事指南实体表
    /// </summary>
    public class ConsServiceGuide : DvsEntityBase
    {
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



    }
}
