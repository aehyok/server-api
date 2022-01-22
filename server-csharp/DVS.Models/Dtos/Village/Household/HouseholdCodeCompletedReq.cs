using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Household
{
    public class HouseholdCodeCompletedReq
    {
        public int TaskId { get; set; }
        public int fileId { get; set; }
        public string filePath { get; set; }
    }
}
