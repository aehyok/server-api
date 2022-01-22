using DVS.Common.Mapping;
using DVS.Core.Domains.GIS;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVS.Core.Mapping.GIS
{
    public class GISCameraMap : DvsMapBase<GISCamera>
    {
        public override void Configure(EntityTypeBuilder<GISCamera> builder)
        {
            base.Configure(builder);
        }
    }
}
