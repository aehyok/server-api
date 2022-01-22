using DVS.Common.Mapping;
using DVS.Core.Domains.GIS;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVS.Core.Mapping.GIS
{
    public class GISCustomMap : DvsMapBase<GISCustom>
    {
        public override void Configure(EntityTypeBuilder<GISCustom> builder)
        {
            base.Configure(builder);
        }
    }
}
