using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Query
{
    /// <summary>
    /// 
    /// </summary>
    public class DeletePostBody
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// 删除原因
        /// </summary>
        public string DeleteReason { get; set; } = "";


    }
}
