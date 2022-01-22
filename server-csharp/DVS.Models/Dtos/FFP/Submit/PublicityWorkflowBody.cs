using DVS.Models.Dtos.Village;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP.Submit
{
    /// <summary>
    /// 公示名单审核
    /// </summary>
    public class PublicityWorkflow
    {
        /// <summary>
        /// workflow Id
        /// </summary>
        public int Id { get; set; } = 0;

       
        /// <summary>
        /// 评议结果，只填:通过或不通过
        /// </summary>
        public string Result { get; set; } = "";

        /// <summary>
        /// 原因、评议详情
        /// </summary>
        public string Reason { get; set; } = "";


    }
}
