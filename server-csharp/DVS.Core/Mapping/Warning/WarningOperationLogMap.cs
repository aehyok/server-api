using DVS.Common.Mapping;
using DVS.Core.Domains.Warning;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Mapping.Warning
{
    public class WarningOperationLogMap : DvsMapBase<WarningOperationLog>
    {
        public override void Configure(EntityTypeBuilder<WarningOperationLog> builder)
        {
            base.Configure(builder);
        }
    }
}
