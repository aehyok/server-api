using DVS.Common.Mapping;
using DVS.Core.Domains.FFP;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Mapping.FFP
{
    public class FFPPublicityManageMap : DvsMapBase<FFPPublicityManage>
    {
        public override void Configure(EntityTypeBuilder<FFPPublicityManage> builder)
        {
            base.Configure(builder);
        }
    }
}