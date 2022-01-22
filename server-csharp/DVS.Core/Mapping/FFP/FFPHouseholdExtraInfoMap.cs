using DVS.Common.Mapping;
using DVS.Core.Domains.FFP;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Mapping.FFP
{
    class FFPHouseholdExtraInfoMap : DvsMapBase<FFPHouseholdExtraInfo> 
    {
        public override void Configure(EntityTypeBuilder<FFPHouseholdExtraInfo> builder)
        {
            base.Configure(builder);
        }
    }
}
