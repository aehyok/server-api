
using DVS.Common.Mapping;
using DVS.Core.Domains.Village;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVS.Core.Mapping.Village
{
    /// <summary>
    /// 
    /// </summary>
    public class VillageHouseNameMap : DvsMapBase<VillageHouseName>
    {
        public override void Configure(EntityTypeBuilder<VillageHouseName> builder)
        {
            base.Configure(builder);
        }
    }
}
