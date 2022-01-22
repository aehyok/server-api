using DVS.Common.Mapping;
using DVS.Core.Domains.IPS;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Mapping.IPS
{
    public class IpsCompanyMap : DvsMapBase<IpsCompany>
    {
        public override void Configure(EntityTypeBuilder<IpsCompany> builder)
        {
            base.Configure(builder);
        }
    }
}
