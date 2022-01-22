using DVS.Common.Mapping;
using DVS.Core.Domains.IPS;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Mapping.IPS
{
    public class IpsScheduleMap : DvsMapBase<IpsSchedule>
    {
        public override void Configure(EntityTypeBuilder<IpsSchedule> builder)
        {
            base.Configure(builder);
        }
    }
}
