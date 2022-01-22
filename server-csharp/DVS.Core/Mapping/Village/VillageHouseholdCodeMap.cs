using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DVS.Core.Domains.Village;
using DVS.Core.Domains.Common;
using DVS.Common.Mapping;

namespace DVS.Core.Mapping.Village
{
    public class VillageHouseholdCodeMap : DvsMapBase<VillageHouseholdCode>
    {
        public override void Configure(EntityTypeBuilder<VillageHouseholdCode> builder)
        {
            builder.Ignore(a => a.AreaName);
            builder.Ignore(a => a.HouseholdMan);
            builder.Ignore(a => a.TagNames);
            builder.Ignore(a => a.Mobile);
            builder.Ignore(a => a.HeadImageUrl);
            base.Configure(builder);
        }
    }
}
