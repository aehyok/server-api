
using DVS.Common.Mapping;
using DVS.Core.Domains.Village;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DVS.Core.Mapping.Village
{
    /// <summary>
    /// 
    /// </summary>
    public class VillageVaccinationMap : DvsMapBase<VillageVaccination>
    {
        public override void Configure(EntityTypeBuilder<VillageVaccination> builder)
        {
            builder.Ignore(a => a.AddressInfo);
            base.Configure(builder);
        }
    }
}
