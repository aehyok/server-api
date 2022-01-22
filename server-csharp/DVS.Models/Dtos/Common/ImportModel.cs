using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Common
{
    public class ImportModel
    {
        public List<ImportItem> Data { get; set; }



    }

    public class ImportItem
    {
        public string Id { get; set; }

        public string RealName { get; set; }

        public string Gender { get; set; }

        public string Birthday { get; set; }

        public string IdCard { get; set; }

        public string Nation { get; set; }

        public string Relationship { get; set; }

        public string Address { get; set; }

    }
}
