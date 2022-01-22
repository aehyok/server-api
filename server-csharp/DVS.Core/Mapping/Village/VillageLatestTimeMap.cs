
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DVS.Core.Domains.Village;
using DVS.Core.Domains.Common;
using DVS.Common.Mapping;

namespace DVS.Core.Mapping.Village
{
    public class VillageLatestTimeMap : DvsMapBase<VillageLatestTime>
    {
        public override void Configure(EntityTypeBuilder<VillageLatestTime> builder)
        {
            builder.Ignore(a => a.CreatedBy);
            builder.Ignore(a => a.UpdatedBy);
            builder.Ignore(a => a.IsDeleted);
            base.Configure(builder);
        }
    }
}
