using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DVS.Core.Domains.Cons;
using DVS.Core.Domains.Common;
using DVS.Common.Mapping;

namespace DVS.Core.Mapping.Cons
{
    public class ConsInfoPublicMap:DvsMapBase<ConsInfoPublic>
    {
        public override void Configure(EntityTypeBuilder<ConsInfoPublic> builder)
        {
            base.Configure(builder);
        }

    }
}
