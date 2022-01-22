using DVS.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Domains.FFP
{
    /// <summary>
    /// 区域网格户码
    /// </summary>
    public class FFPMatrixHousehold : DvsEntityBase
    {
        /// <summary>
        /// 网格id
        /// </summary>
        public int MatrixId { get; set; }

        /// <summary>
        /// 户码id
        /// </summary>
        public int HouseholdId { get; set; }

    }
}
