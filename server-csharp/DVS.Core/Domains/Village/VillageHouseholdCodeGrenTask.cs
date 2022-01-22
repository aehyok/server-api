using DVS.Common.Models;
using DVS.Models.Dtos.Village.Household;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.Village
{
    public class VillageHouseholdCodeGrenTask : DvsEntityBase
    {
        /// <summary>
        /// 0未生成，1已生成
        /// </summary>
        public int Status { get; set; } = 0;
        public int ZipFileId { get; set; }
        public string CodeData { get; set; }
        public int TemplateId { get; set; }
        // 任务类型，1户码，2二维码
        public int TaskType { get; set; } = 0;
        public virtual VillageHouseholdCodeTemplateDto Template { get; set; }

    }
}
