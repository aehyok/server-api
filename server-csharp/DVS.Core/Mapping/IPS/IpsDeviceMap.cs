using DVS.Common.Mapping;
using DVS.Core.Domains.IPS;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Mapping.IPS
{
    public class IpsDeviceMap : DvsMapBase<IpsDevice>
    {
        public override void Configure(EntityTypeBuilder<IpsDevice> builder)
        {
            base.Configure(builder);
        }
    }
}
