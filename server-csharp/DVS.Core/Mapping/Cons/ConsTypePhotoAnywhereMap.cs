using DVS.Common.Mapping;
using DVS.Core.Domains.Cons;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Mapping.Cons
{
    public class ConsTypePhotoAnywhereMap : DvsMapBase<ConsTypePhotoAnywhere>
    {
        public override void Configure(EntityTypeBuilder<ConsTypePhotoAnywhere> builder)
        {
            base.Configure(builder);
        }
    }
}
