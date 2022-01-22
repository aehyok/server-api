using DVS.Common.Mapping;
using DVS.Core.Domains.FFP;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVS.Core.Mapping.FFP
{
    public class FFPMatrixMap : DvsMapBase<FFPMatrix>
    {
        public override void Configure(EntityTypeBuilder<FFPMatrix> builder)
        {
            base.Configure(builder);
        }
    }
}
