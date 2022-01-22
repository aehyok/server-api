using DVS.Common.Mapping;
using DVS.Core.Domains.FFP;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVS.Core.Mapping.FFP
{
    public class FFPAutoNumberMap : DvsMapBase<FFPAutoNumber>
    {
        public override void Configure(EntityTypeBuilder<FFPAutoNumber> builder)
        {
            builder.Ignore(a => a.CreatedBy);
            builder.Ignore(a => a.IsDeleted);
            builder.Ignore(a => a.UpdatedAt);
            builder.Ignore(a => a.UpdatedBy);
            base.Configure(builder);
        }
    }
}
