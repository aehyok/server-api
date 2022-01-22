using DVS.Common.Mapping;
using DVS.Core.Domains.Village;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Mapping.Village
{
    public class VillageHouseholdCodeGrenTaskMap : DvsMapBase<VillageHouseholdCodeGrenTask>
    {
        public override void Configure(EntityTypeBuilder<VillageHouseholdCodeGrenTask> builder)
        {
            builder.Ignore(task => task.Template);
            base.Configure(builder);
        }
    }
}
