using DVS.Models.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Models.Dtos.Village.Query
{
    public class PageHouseNameBody : PagedListQueryModel
    {

        /// <summary>
        /// 行政区域Id
        /// </summary>
        public int AreaId { get; set; }


    }
}
