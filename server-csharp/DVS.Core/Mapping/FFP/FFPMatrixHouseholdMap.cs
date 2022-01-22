using DVS.Common.Mapping;
using DVS.Core.Domains.FFP;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVS.Core.Mapping.FFP
{
    public class FFPMatrixHouseholdMap : DvsMapBase<FFPMatrixHousehold>
    {
        public override void Configure(EntityTypeBuilder<FFPMatrixHousehold> builder)
        {
            base.Configure(builder);
        }
    }
}
