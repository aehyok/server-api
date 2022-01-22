using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DVS.Core.Domains.Village;
using DVS.Core.Domains.Common;
using DVS.Common.Mapping;

namespace DVS.Core.Mapping.Village
{
    public class VillageUserAuthRecordMap : DvsMapBase<VillageUserAuthRecord>
    {
        public override void Configure(EntityTypeBuilder<VillageUserAuthRecord> builder)
        {
            builder.Ignore(a => a.CreatedBy);
            base.Configure(builder);
        }
    }
}
