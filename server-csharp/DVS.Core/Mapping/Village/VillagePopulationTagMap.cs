
using DVS.Common.Mapping;
using DVS.Core.Domains.Village;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVS.Core.Mapping.Village
{
    public class VillagePopulationTagMap : DvsMapBase<VillagePopulationTag>
    {
        public override void Configure(EntityTypeBuilder<VillagePopulationTag> builder)
        {
            builder.Ignore(a => a.CreatedBy);
            builder.Ignore(a => a.UpdatedBy);
            base.Configure(builder);
        }
    }
}
