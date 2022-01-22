﻿using DVS.Models.Dtos.Village;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP.Submit
{
    /// <summary>
    /// 录入评议结果
    /// </summary>
    public class ReviewWorkflowBody
    {
        /// <summary>
        /// workflow Id
        /// </summary>
        public int Id { get; set; } = 0;


        /// <summary>
        /// 投票人数
        /// </summary>
        public int VoteCount { get; set; } = 0;

        /// <summary>
        /// 同意票数
        /// </summary>
        public int Agree { get; set; } = 0;

        /// <summary>
        /// 不同意票数
        /// </summary>
        public int Disagree { get; set; } = 0;


        /// <summary>
        /// 评议结果，只填:通过或不通过
        /// </summary>
        public string Result { get; set; } = "";

        /// <summary>
        /// 原因、评议详情
        /// </summary>
        public string Reason { get; set; } = "";


        /// <summary>
        /// 图片Ids 多个用逗号分割
        /// </summary>
        public string Images { get; set; } = "";


    }
}
