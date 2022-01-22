using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Common
{
    public class OrderBy
    {
        public string FieldName { get; set; } = "";
        public string Sort { get; set; } = "asc";
    }
}
