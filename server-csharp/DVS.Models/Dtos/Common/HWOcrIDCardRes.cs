using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Common
{
    public class HWOcrIDCardResInfo {
        public HWOcrIDCardRes Result { get; set; }
        public string Error_Code{get;set;}
        public string Error_Msg { get; set; }
    }
    public class HWOcrIDCardRes
    {
        public string Name { get; set; }
        public string Sex { get; set; }
        public string Ethnicity { get; set; }

        public string Birth { get; set; }
        public string Address { get; set; }
        public string Number { get; set; }
    }
}
