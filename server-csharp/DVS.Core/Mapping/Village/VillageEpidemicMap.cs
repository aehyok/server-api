using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DVS.Core.Domains.Village;
using DVS.Core.Domains.Common;
using DVS.Common.Mapping;

namespace DVS.Core.Mapping.Village
{
    public class VillageEpidemicMap : DvsMapBase<VillageEpidemic>
    {


        public override void Configure(EntityTypeBuilder<VillageEpidemic> builder)
        {
            builder.Ignore(a => a.SourceAddressInfo);
            builder.Ignore(a => a.AreaId);
            builder.Ignore(a => a.HouseholdId);
            base.Configure(builder);
        }
    }
}
