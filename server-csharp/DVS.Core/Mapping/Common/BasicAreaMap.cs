using DVS.Core.Domains.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lychee.EntityFramework;
using Microsoft.EntityFrameworkCore;
using DVS.Common.Mapping;

namespace DVS.Core.Mapping.Common
{
    public class BasicAreaMap : DvsMapBase<BasicArea>
    {
        public override void Configure(EntityTypeBuilder<BasicArea> builder)
        {

            builder.Ignore(a => a.CreatedBy);
            builder.Ignore(a => a.UpdatedBy);
            base.Configure(builder);
        }
    }
}