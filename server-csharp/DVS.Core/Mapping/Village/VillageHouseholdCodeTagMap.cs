using DVS.Common.Mapping;
using DVS.Core.Domains.Village;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVS.Core.Mapping.Village
{
    public class VillageHouseholdCodeTagMap : DvsMapBase<VillageHouseholdCodeTag>
    {
        public override void Configure(EntityTypeBuilder<VillageHouseholdCodeTag> builder)
        {
            builder.Ignore(a => a.CreatedBy);
            builder.Ignore(a => a.UpdatedBy);

            base.Configure(builder);
        }
    }
}