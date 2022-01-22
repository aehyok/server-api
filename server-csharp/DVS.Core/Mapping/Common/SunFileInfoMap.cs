using DVS.Common.Mapping;
using DVS.Core.Domains.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Mapping.Common
{
    public class SunFileInfoMap: DvsMapBase<SunFileInfo>
    {
        public override void Configure(EntityTypeBuilder<SunFileInfo> builder)
        {
            base.Configure(builder);
        }
    }
}
