
using DVS.Common.Mapping;
using DVS.Core.Domains.Village;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVS.Core.Mapping.Village
{
    /// <summary>
    /// 
    /// </summary>
    public class VillageHouseholdCodeTemplateMap : DvsMapBase<VillageHouseholdCodeTemplate>
    {
        public override void Configure(EntityTypeBuilder<VillageHouseholdCodeTemplate> builder)
        {
            base.Configure(builder);
        }
    }
}
