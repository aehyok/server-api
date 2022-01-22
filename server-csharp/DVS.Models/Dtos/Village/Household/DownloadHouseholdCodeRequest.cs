using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Household
{
    public class DownloadHouseholdCodeRequest
    {
        public int AreaCode { get; set; }

        public List<int> Ids { get; set; }
        public int TemplateId { get; set; }
        public int TaskType { get; set; } = 1;
    }
}
