using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class BasicDictionaryHouseTagBody {

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 图标文件id
        /// </summary>
        public int IconFileId { get; set; }

        /// <summary>
        /// iconFileUrl
        /// </summary>
        public string IconFileUrl { get; set; }

    }

}

