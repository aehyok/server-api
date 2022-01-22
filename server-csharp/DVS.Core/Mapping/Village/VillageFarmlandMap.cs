using DVS.Common.Mapping;
using DVS.Core.Domains.Village;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Mapping.Village
{
    public class VillageFarmlandMap: DvsMapBase<VillageFarmland>
    {
        public override void Configure(EntityTypeBuilder<VillageFarmland> builder)
        {
            builder.HasKey(land => land.Id);
            base.Configure(builder);
        }
    }
}
