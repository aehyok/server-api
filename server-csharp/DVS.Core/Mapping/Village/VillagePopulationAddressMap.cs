
using DVS.Common.Mapping;
using DVS.Core.Domains.Village;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVS.Core.Mapping.Village
{
    public class VillagePopulationAddressMap : DvsMapBase<VillagePopulationAddress>
    {
        public override void Configure(EntityTypeBuilder<VillagePopulationAddress> builder)
        {
            builder.Ignore(a => a.CreatedBy);
            builder.Ignore(a => a.UpdatedBy);
            base.Configure(builder);
        }
    }
}
