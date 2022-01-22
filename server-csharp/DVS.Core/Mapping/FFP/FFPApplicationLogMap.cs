using DVS.Common.Mapping;
using DVS.Core.Domains.FFP;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Mapping.FFP
{
    public class FFPApplicationLogMap: DvsMapBase<FFPApplicationLog>
    {
        public override void Configure(EntityTypeBuilder<FFPApplicationLog> builder)
        {
            base.Configure(builder);
        }
    }
}
