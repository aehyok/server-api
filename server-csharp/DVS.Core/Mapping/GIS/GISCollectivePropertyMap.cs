using DVS.Common.Mapping;
using DVS.Core.Domains.GIS;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVS.Core.Mapping.GIS
{
    public class GISCollectivePropertyMap : DvsMapBase<GISCollectiveProperty>
    {
        public override void Configure(EntityTypeBuilder<GISCollectiveProperty> builder)
        {
            base.Configure(builder);
        }
    }
}
