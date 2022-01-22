using DVS.Models.Dtos.Common;
using DVS.Models.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Query
{



    public class PopulationListBody : PagedListQueryModel
    {

        /// <summary>
        /// 行政区域Id
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// 户码id
        /// </summary>
        public int HouseholdId { get; set; } = -1;


        /// <summary>
        /// 人口Id
        /// </summary>
        public int PopulationId { get; set; } = 0;

        /// <summary>
        /// Id组，例如：1,2,3....
        /// </summary>
        public string Ids { get; set; } = "";


        /// <summary>
        /// 标签Ids   例如1,2,3
        /// </summary>
        public string Tags { get; set; } = "";
    }
}
