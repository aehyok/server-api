using DVS.Core.Domains.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lychee.EntityFramework;
using Microsoft.EntityFrameworkCore;
using DVS.Common.Mapping;

namespace DVS.Core.Mapping.Common
{
    public class ParkAreaMap : DvsMapBase<ParkArea>
    {
        public override void Configure(EntityTypeBuilder<ParkArea> builder)
        {

            builder.Ignore(a => a.CreatedBy);
            builder.Ignore(a => a.UpdatedBy);
            base.Configure(builder);
        }
    }
}