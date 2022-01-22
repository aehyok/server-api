using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DVS.Core.Domains.Village;
using DVS.Core.Domains.Common;
using DVS.Common.Mapping;

namespace DVS.Core.Mapping.Village
{
    public class VillagePopulationMap : DvsMapBase<VillagePopulation>
    {
        public override void Configure(EntityTypeBuilder<VillagePopulation> builder)
        {
            builder.Ignore(a => a.tagNames);
            // builder.Ignore(a => a.HouseholdId);
            // builder.Ignore(a => a.IsHouseholder);
            // builder.Ignore(a=>a.TagList);
            base.Configure(builder);
        }
    }
}
