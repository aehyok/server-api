using DVS.Common.Mapping;
using DVS.Core.Domains.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVS.Core.Mapping.Common
{
    public class BasicRoleMap : DvsMapBase<BasicRole>
    {
        public override void Configure(EntityTypeBuilder<BasicRole> builder)
        {

            builder.Ignore(a => a.CreatedBy);
            builder.Ignore(a => a.UpdatedBy);
            base.Configure(builder);
        }
    }
}
