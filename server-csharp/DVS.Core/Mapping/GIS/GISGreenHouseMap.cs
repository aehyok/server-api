using DVS.Common.Mapping;
using DVS.Core.Domains.GIS;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVS.Core.Mapping.GIS
{
    public class GISGreenHouseMap : DvsMapBase<GISGreenHouse>
    {
        public override void Configure(EntityTypeBuilder<GISGreenHouse> builder)
        {
            base.Configure(builder);
        }
    }
}
