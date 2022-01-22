using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.FFP.Query
{
    public class DetailQueryModel
    {
        public int Id { get; set; }
    }

    public class DeleteModel
    {
        public string Ids { get; set; }
    }

    public class RemoveQueryModel
    {
        public int MatrixId { get; set; }

        public string HouseholdIds { get; set; }
    }

    public class AddQueryModel
    {
        public int MatrixId { get; set; }

        public string HouseholdIds { get; set; }
    }
}
