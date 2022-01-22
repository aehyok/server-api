using DVS.Common.Mapping;
using DVS.Core.Domains.IPS;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Mapping.IPS
{
    public class IpsMessageMap : DvsMapBase<IpsMessage>
    {
        public override void Configure(EntityTypeBuilder<IpsMessage> builder)
        {
            base.Configure(builder);
        }
    }
}
