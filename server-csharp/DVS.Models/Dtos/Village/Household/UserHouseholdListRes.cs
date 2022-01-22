using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Household
{
    public class UserHouseholdListRes
    {
        public int Id { get; set; }
        public string HouseNumber { get; set; }
        public string HouseName { get; set; }
        public bool IsCurrent { get; set; }
    }
}
