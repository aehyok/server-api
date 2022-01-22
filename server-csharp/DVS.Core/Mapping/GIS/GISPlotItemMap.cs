using DVS.Common.Mapping;
using DVS.Core.Domains.GIS;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVS.Core.Mapping.GIS
{
    public class GISPlotItemMap : DvsMapBase<GISPlotItem>
    {
        public override void Configure(EntityTypeBuilder<GISPlotItem> builder)
        {
            builder.Ignore(a => a.ObjectIds);
            base.Configure(builder);
        }
    }
}
