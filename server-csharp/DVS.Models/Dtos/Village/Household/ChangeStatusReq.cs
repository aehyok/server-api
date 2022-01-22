using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Household
{
    public class ChangeStatusReq
    {
        public int Id { get; set; }
        public int Status { get; set; }
    }
}
