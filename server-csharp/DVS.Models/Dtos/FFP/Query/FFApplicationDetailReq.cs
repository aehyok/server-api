using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DVS.Models.Dtos.FFP.Query
{
   public  class FFApplicationDetailReq
    {

        /// <summary>
        /// 工作流的id
        /// </summary>
        public int WorkflowId { get; set; }
    }
}
