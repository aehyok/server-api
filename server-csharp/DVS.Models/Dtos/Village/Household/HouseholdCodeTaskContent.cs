using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Household
{
    public class HouseholdCodeTaskContent
    {
        public string Url { get; set; }

        public int Id { get; set; }

        public string HouseholderName { get; set; }

        public string HouseName { get; set; }

        public string HouseNumber { get; set; }

        public long AreaCode { get; set; }

        public string AreaName { get; set; }

        public bool TemplateImage { get; set; }
    }
}
